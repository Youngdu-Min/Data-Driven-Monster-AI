using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public abstract class BaseBahaviorState : MonoBehaviour
{
    [Header("Transition Settings")]
    // 리스트 제목을 GetRuleLabel 함수에서 실시간으로 생성하도록 설정
    [ListDrawerSettings(ListElementLabelName = "@GetRuleLabel()", ShowIndexLabels = false)]
    [SerializeField] private List<TransitionRule> _transitionRules = new();

    [Serializable]
    public struct TransitionRule
    {
        [LabelText("규칙 용도")]
        public string UsageTag;

        [Required]
        public BehaviorStateCondition Condition;

        [Required]
        public BaseBahaviorState TargetState;

        // 리스트 제목 생성: [규칙 용도] 컨디션 클래스명 -> 타겟 상태명
        public string GetRuleLabel()
        {
            string tag = string.IsNullOrEmpty(UsageTag) ? "No Tag" : UsageTag;
            string condName = Condition != null ? Condition.GetType().Name : "Null";
            string extraInfo = Condition != null ? Condition.GetConditionExtraInfo() : "";
            if (!string.IsNullOrEmpty(extraInfo)) extraInfo = $"({extraInfo})";

            string targetName = TargetState != null ? TargetState.GetType().Name : "Null";

            return $"[{tag}] {condName}{extraInfo} → {targetName}";
        }
    }

    public Action<BaseBahaviorState> StateEnabled;
    public Action<BaseBahaviorState> StateDisabled;
    private CancellationTokenSource _disableCts;

    public Func<FsmData> CopyFsmData;
    private float _stateEnterTime = 0;

    [SerializeField] private BaseBahaviorState _defaultFallbackState; // 아무 조건도 안 맞을 때 갈 곳 (예: Idle)

    private void OnEnable()
    {
        _disableCts = new CancellationTokenSource();

        StateEnabledTriggerAsync(_disableCts.Token).Forget();
    }

    private void OnDisable()
    {
        _disableCts?.Cancel();
        _disableCts?.Dispose();

        StateDisabledTriggerAsync(this.GetCancellationTokenOnDestroy()).Forget();
    }

    public virtual void Init() { }

    private async UniTaskVoid StateEnabledTriggerAsync(CancellationToken ct)
    {
        await UniTask.WaitUntil(() => StateEnabled != null, cancellationToken: ct);

        StateEnabled.Invoke(this);
    }

    private async UniTaskVoid StateDisabledTriggerAsync(CancellationToken ct)
    {
        await UniTask.WaitUntil(() => StateDisabled != null, cancellationToken: ct);

        StateDisabled.Invoke(this);
    }

    public void Fsm_ActionState(Fsm fsm, Fsm.Step step, Fsm.State state)
    {
        if (step == Fsm.Step.Enter)
        {
            Fsm_Step_Enter();
        }
        else if (step == Fsm.Step.Update)
        {
            Fsm_Step_Update();
        }
        else if (step == Fsm.Step.Exit)
        {
            Fsm_Step_Exit();
        }
    }

    protected virtual void Fsm_Step_Enter()
    {
        _stateEnterTime = 0;
    }
    protected virtual void Fsm_Step_Update()
    {
        _stateEnterTime += Time.deltaTime;

        // 등록된 모든 전이 규칙을 검사
        foreach (var rule in _transitionRules)
        {
            if (rule.Condition &&
                rule.Condition.IsConditionMet(transform, CopyFsmData(), _stateEnterTime))
            {
                CopyFsmData().BaseFsm.TransitionToState(rule.TargetState, rule.GetRuleLabel()).Forget();
                return;
            }
        }
    }

    protected void TransitionTo(BaseBahaviorState nextState)
    {
        if (nextState)
        {
            CopyFsmData().BaseFsm.TransitionToState(nextState).Forget();
        }
        else if (_defaultFallbackState)
        {
            CopyFsmData().BaseFsm.TransitionToState(_defaultFallbackState).Forget();
        }
    }

    protected virtual void Fsm_Step_Exit() { }

    public void ForceStateExit()
        => Fsm_Step_Exit();
}