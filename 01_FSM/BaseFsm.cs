using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using SO.Monster;
using System;
using System.Threading;
using UnityEngine;
using static MonsterInterface;

public struct FsmData
{
    private Fsm _fsm;
    public Transform Target { get; private set; }
    public readonly BaseFsm BaseFsm { get; } // BaseFsm 접근자 추가
    public MonsterMediator Mediator { get; private set; }

    public FsmData(Fsm fsm, BaseFsm baseFsm, Transform player, MonsterMediator mediator)
    {
        _fsm = fsm;
        BaseFsm = baseFsm;
        Target = player;
        Mediator = mediator;
    }

    public readonly bool IsStabilized => Mediator != null && Mediator.IsStabilized;
}

[ShowOdinSerializedPropertiesInInspector]
public abstract class BaseFsm : MonoBehaviour, IUpdateObserver, MonsterInterface.ISetMultiplier, IGetMediator
{
    private Fsm _fsm;

    private Fsm.State _spawnState;
    private Fsm.State _idleState;
    private Fsm.State _statusEffectState;

    protected BaseBahaviorState[] _allStates;
    protected BehaviorStateCondition[] _allConditions;

    [Title("FSM Monitoring")]
    [LabelText("현재 상태명")]
    [ShowInInspector, ReadOnly]
    private string _currentStateName;

    [SerializeField] private ScriptableObject[] _stateConfigs; // 여러 유형의 SO를 넣음

    [Header("Debug Settings")]
    [SerializeField] private bool _showStateTransitionLog = true; // 디버깅용 bool

    public BaseBahaviorState ActiveState { get; private set; }
    [ShowInInspector, ReadOnly] private bool _blockTransition = false;
    [ShowInInspector, ReadOnly] private bool _isSystemPaused = false;  // 시스템(안정화/수면)용 정지

    private CancellationTokenSource _transitionCts;
    [ShowInInspector, ReadOnly] private bool _isWaitingTransition = false;

    private float _transitionDelayMultiplier = 1;
    private Transform _targetPlayer;
    private MonsterMediator _mediator;
    protected Action onSpawn;

    public enum BaseFsmStateType
    {
        Spawn,
        Idle,
        StatusEffect,
    }

    private FsmData GetFsmData()
    {
        FsmData data = new(_fsm, this, _targetPlayer, _mediator);
        return data;
    }

    public virtual void Init()
    {
        UpdatePublisher.SubscribeObserver(this);
        _allStates = GetComponentsInChildren<BaseBahaviorState>(true);
        _allConditions = GetComponentsInChildren<BehaviorStateCondition>(true);

        _targetPlayer = GameObject.FindGameObjectWithTag("Player")?.transform;

        InitFsm();

        _fsm = new Fsm();
        _fsm.Start(_spawnState);
    }

    public void ObserverUpdate(float dt)
    {
        _fsm.OnUpdate();
    }

    protected virtual void InitFsm()
    {
        _spawnState = Fsm_SpawnState;
        _idleState = Fsm_IdleState;
        _statusEffectState = Fsm_StatusEffectState;

        foreach (BaseBahaviorState state in _allStates)
        {
            state.CopyFsmData = GetFsmData;
            state.Init();
        }

        foreach (BehaviorStateCondition condition in _allConditions)
        {
            condition.Init(GetFsmData());
        }
    }

    protected virtual void InitializeStateConfig(BaseBahaviorState baseState) { }

    #region State - Transition

    public virtual void TransitionToStateNonDelay(BaseFsmStateType state)
    {
        if (_isSystemPaused || _blockTransition) { return; }

        if (_showStateTransitionLog)
        {
            Debug.Log($"<color=yellow>[FSM System]</color> {gameObject.name}: {_currentStateName} -> {state}");
        }

        print(state.ToString());
        _currentStateName = state.ToString();

        ActiveState = null; // 시스템 상태로 전이 시 객체 참조 해제
        Fsm.State fsmState = state switch
        {
            BaseFsmStateType.Spawn => _spawnState,
            BaseFsmStateType.Idle => _idleState,
            BaseFsmStateType.StatusEffect => _statusEffectState,
            _ => throw new NotImplementedException(),
        };
        _fsm.TransitionTo(fsmState);

    }

    public void TransitionToStateNonDelay(BaseBahaviorState baseState, string ruleLabel = "")
    {
        if (baseState == null || _blockTransition || _isSystemPaused || baseState is ReturnState) { return; }

        if (_showStateTransitionLog)
        {
            string reason = string.IsNullOrEmpty(ruleLabel) ? "Manual Transition" : ruleLabel;
            Debug.Log($"<color=cyan>[FSM Transition]</color> <b>{gameObject.name}</b>: {reason}");
        }

        print(baseState.GetType().Name.ToString());
        // 인스펙터 표시용 데이터 갱신
        _currentStateName = baseState.GetType().Name;

        ActiveState = baseState; // 현재 활성 상태 객체 저장
        _fsm.TransitionTo(baseState.Fsm_ActionState);
    }

    public async UniTaskVoid TransitionToState(BaseBahaviorState baseState, string ruleLabel = "")
    {
        if (baseState == null || _blockTransition || _isSystemPaused || _isWaitingTransition ||
            ActiveState == baseState || baseState is ReturnState) { return; }

        _isWaitingTransition = true;

        // 기존 상태 종료
        ActiveState?.ForceStateExit();

        // 행동 전환 시간 계산 
        float delay = 0.5f; // 기본값
        MovementConfigSO movConfig = GetConfig<MovementConfigSO>();
        if (movConfig != null) { delay = movConfig.ActionTransitionTime; }
        delay *= _transitionDelayMultiplier;

        if (_showStateTransitionLog)
        {
            string reason = string.IsNullOrEmpty(ruleLabel) ? "Manual" : ruleLabel;
            Debug.Log($"<color=orange>[FSM Delay]</color> {delay}s 대기 중... (사유: {reason})");
        }

        // 지연 시간 동안 대기
        _transitionCts?.Cancel();
        _transitionCts = new CancellationTokenSource();

        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: _transitionCts.Token);
        }
        catch (OperationCanceledException)
        {
            _isWaitingTransition = false;
            return;
        }

        // 상태 전이 수행
        _currentStateName = baseState.GetType().Name;
        ActiveState = baseState;
        _fsm.TransitionTo(baseState.Fsm_ActionState);

        _isWaitingTransition = false;
    }
    #endregion

    #region FSM_Status
    protected virtual void Fsm_SpawnState(Fsm fsm, Fsm.Step step, Fsm.State state)
    {
        onSpawn?.Invoke();
        if (step == Fsm.Step.Enter)
        {
            if (_allStates.Length == 0)
            {
                print("State Count is 0");
                return;
            }

            TransitionToStateNonDelay(BaseFsmStateType.Idle);
            // TransitionToState(_allStates[0]);
        }
    }
    protected virtual void Fsm_IdleState(Fsm fsm, Fsm.Step step, Fsm.State state) { }
    protected virtual void Fsm_StatusEffectState(Fsm fsm, Fsm.Step step, Fsm.State state) { }
    #endregion

    #region Feedbacks
    protected virtual void OnHitFeedback() { }

    #endregion

#if UNITY_EDITOR
    [OnInspectorGUI]
    private void RepaintInspector()
    {
        if (Application.isPlaying)
        {
            // 인스펙터를 강제로 다시 그리게 함
            GUIHelper.RequestRepaint();
        }
    }
#endif

    public void Dispose()
    {
        UpdatePublisher.DiscribeObserver(this);
    }

    // 특정 타입의 데이터를 찾아주는 유틸리티 (중복 방지 및 빠른 참조)
    public T GetConfig<T>() where T : ScriptableObject
    {
        foreach (ScriptableObject config in _stateConfigs)
        {
            if (config is T target) { return target; }
        }
        return null;
    }

    public void SetBlockTransition(bool isBlock)
        => _blockTransition = isBlock;

    public void SetSystemPause(bool pause) =>
        _isSystemPaused = pause;

    public void SetMultiplier(float multiplier)
        => _transitionDelayMultiplier = multiplier;

    public void SetMediator(MonsterMediator mediator)
        => _mediator = mediator;

    public void ResetMultiplier()
        => _transitionDelayMultiplier = 1f;
}