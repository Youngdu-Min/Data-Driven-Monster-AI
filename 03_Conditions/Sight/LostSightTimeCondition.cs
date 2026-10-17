using Sirenix.OdinInspector;
using SO.Monster;
using UnityEngine;

/// <summary>
/// 시야 감지를 잃고 거리 범위 안에만 있는 상태가
/// _lostCountDuration 초 이상 지속되면 true 반환.
/// 시야 회복 또는 거리 이탈 시 카운트 초기화.
/// </summary>

public class LostSightTimeCondition : BehaviorStateCondition
{
    [SerializeField][Required] private BaseSightCondition _sightCondition;
    [SerializeField][Required] private BaseDistanceCondition _distanceCondition;
    private float _lostCountDuration;
    private float _lostTimer = 0f;

    public override void Init(FsmData fsmData)
    {
        Debug.Assert(_distanceCondition != null);
        Debug.Assert(_sightCondition != null);

        DetectionConfigSO config = fsmData.BaseFsm.GetConfig<DetectionConfigSO>();
        Debug.Assert(config);

        if (!config) { return; }

        _lostCountDuration = config.ActionHoldTime;
    }

    public override bool IsConditionMet(Transform owner, FsmData fsmData, float stateEnterTime)
    {
        if (!_sightCondition || !_distanceCondition) { return false; }

        bool hasSight = _sightCondition.IsConditionMet(owner, fsmData, stateEnterTime);
        bool inRange = _distanceCondition.IsConditionMet(owner, fsmData, stateEnterTime);

        if (fsmData.Mediator.IsStabilized || hasSight || !inRange)
        {
            _lostTimer = 0f;
            return false;
        }

        _lostTimer += Time.deltaTime;
        return _lostTimer >= _lostCountDuration;
    }

    public override string GetConditionExtraInfo()
    {
        return $"미감지 유예({_lostCountDuration}s)";
    }
}
