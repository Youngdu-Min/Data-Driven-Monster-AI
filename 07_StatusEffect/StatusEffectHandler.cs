using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectHandler : MonoBehaviour, IUpdateObserver
{
    private MonsterMediator _mediator;
    private BaseFsm _fsm;
    private UpdateDispoer _updateDisposer; // 기존 시스템 활용

    [Title("Configs")]
    private BindConfigSO _bindConfig;
    private SleepConfigSO _sleepConfig;

    [Title("Status Data")]
    [ShowInInspector, ReadOnly] public float CurrentBindGauge { get; private set; }
    [ShowInInspector, ReadOnly] public float CurrentSleepGauge { get; private set; }

    public bool IsBound => _bindConfig != null && CurrentBindGauge >= _bindConfig.BindGaugeMax;
    public bool IsSleeping => _sleepConfig != null && CurrentSleepGauge >= _sleepConfig.SleepGaugeMax;

    [Title("Bind Visuals")]
    [SerializeField] private RopeRenderer _ropeRendererPrefab;
    private List<RopeRenderer> _activeRopes = new();
    private List<RopeRenderer> _ropePool = new();
    private float _lastRopeLength = 3.0f;

    // 수면 안개 체류 여부 플래그
    private bool _isInSleepArea = false;

    private void Awake()
    {
        _mediator = GetComponent<MonsterMediator>();
        _fsm = GetComponent<BaseFsm>();
    }

    private void Start()
    {
        // BaseFsm이나 중재자를 통해 Config 캐싱 (기존 데이터 중심 설계 적용)
        _bindConfig = _fsm.GetConfig<BindConfigSO>();
        _sleepConfig = _fsm.GetConfig<SleepConfigSO>();
        _updateDisposer = new UpdateDispoer(this);
    }

    private void OnDestroy() => _updateDisposer?.Dispose();

    #region Bind Logic (속박)
    public void ApplyBindHit(float amount, float ropeLength)
    {
        if (_bindConfig == null || IsSleeping) return; // 수면 중 속박 불가 등 기획 예외처리

        bool wasBound = IsBound;
        _lastRopeLength = ropeLength;
        CurrentBindGauge = Mathf.Min(CurrentBindGauge + amount, _bindConfig.BindGaugeMax);

        SyncRopeCount();

        if (!wasBound && IsBound)
        {
            _mediator?.OnNotify(this, "OnBindEntered"); // Action 대신 중재자 호출
        }
    }

    private void SyncRopeCount()
    {
        if (_bindConfig == null) return;
        int targetCount = Mathf.CeilToInt(CurrentBindGauge / _bindConfig.BindRopeUnit);

        while (_activeRopes.Count < targetCount)
        {
            var rope = GetOrCreateRope();
            // 임시 앵커 위치(이후 구체화) 및 로프 연결
            rope.Activate(transform, transform.position + Vector3.up, _lastRopeLength);
            _activeRopes.Add(rope);
        }

        while (_activeRopes.Count > targetCount)
        {
            int last = _activeRopes.Count - 1;
            _activeRopes[last].Deactivate();
            _ropePool.Add(_activeRopes[last]);
            _activeRopes.RemoveAt(last);
        }
    }
    #endregion

    #region Sleep Logic (수면)
    // SleepArea가 진입/이탈 시 호출
    public void SetInSleepArea(bool inArea) => _isInSleepArea = inArea;

    // 수면 강제 해제 (기획상 다른 상태이상 적용 시 등)
    public void ForceWakeUp()
    {
        if (CurrentSleepGauge <= 0 && !IsSleeping) return;
        CurrentSleepGauge = 0f;
        _mediator?.OnNotify(this, "OnSleepExited");
    }
    #endregion

    #region Update Loop (게이지 실시간 증감)
    public void ObserverUpdate(float dt)
    {
        UpdateBindGauge(dt);
        UpdateSleepGauge(dt);
    }

    private void UpdateBindGauge(float dt)
    {
        if (_bindConfig == null || CurrentBindGauge <= 0f) return;

        bool wasBound = IsBound;
        // FSM 상태를 통해 행동 상태 여부 판단 (결합도 낮춤)
        bool isActionState = _fsm.ActiveState is AttackState or MovableState;
        float decayRate = isActionState ? _bindConfig.BindDecayRateInAction : _bindConfig.BindDecayRate;

        CurrentBindGauge = Mathf.Max(0f, CurrentBindGauge - decayRate * dt);
        SyncRopeCount();

        if (wasBound && !IsBound)
        {
            ClearAllRopes();
            _mediator?.OnNotify(this, "OnBindExited");
        }

        foreach (var rope in _activeRopes) rope.UpdatePosition();
    }

    private void UpdateSleepGauge(float dt)
    {
        if (_sleepConfig == null) return;
        bool wasSleeping = IsSleeping;

        if (_isInSleepArea)
        {
            CurrentSleepGauge = Mathf.Min(CurrentSleepGauge + (_sleepConfig.SleepIncreaseRate * dt), _sleepConfig.SleepGaugeMax);
        }
        else if (CurrentSleepGauge > 0)
        {
            CurrentSleepGauge = Mathf.Max(0f, CurrentSleepGauge - (_sleepConfig.SleepDecayRate * dt));
        }

        if (!wasSleeping && IsSleeping) _mediator?.OnNotify(this, "OnSleepEntered");
        else if (wasSleeping && !IsSleeping) _mediator?.OnNotify(this, "OnSleepExited");
    }
    #endregion

    private void ClearAllRopes()
    {
        foreach (var rope in _activeRopes) { rope.Deactivate(); _ropePool.Add(rope); }
        _activeRopes.Clear();
    }

    private RopeRenderer GetOrCreateRope()
    {
        if (_ropePool.Count > 0)
        {
            var rope = _ropePool[^1]; // 최신 C# 문법
            _ropePool.RemoveAt(_ropePool.Count - 1);
            return rope;
        }
        return Instantiate(_ropeRendererPrefab, transform);
    }
}