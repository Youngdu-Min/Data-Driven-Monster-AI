using Sirenix.OdinInspector;
using SO.Monster;
using UnityEngine;

/// <summary>
/// 플레이어가 거리 감지 범위를 벗어난 상태가
/// _lostTargetReturnTime 초 이상 지속되면 true 반환.
/// 범위 안으로 재진입 시 카운트 초기화.
/// </summary>
public class LostDistanceTimeCondition : BehaviorStateCondition
{
    [SerializeField][Required] private BaseDistanceCondition _distanceCondition;
    private float _lostTargetReturnTime;
    private float _lostTimer = 0f;

    public override void Init(FsmData fsmData)
    {
        Debug.Assert(_distanceCondition != null);

        DetectionConfigSO config = fsmData.BaseFsm.GetConfig<DetectionConfigSO>();
        Debug.Assert(config != null);

        if (!config) { return; }

        _lostTargetReturnTime = config.LostTargetReturnTime;
    }

    public override bool IsConditionMet(Transform owner, FsmData fsmData, float stateEnterTime)
    {
        if (!_distanceCondition) { return false; }

        // inRange가 false일 때가 '범위 이탈' 상태임
        bool inRange = _distanceCondition.IsConditionMet(owner, fsmData, stateEnterTime);

        if (fsmData.Mediator.IsStabilized || inRange)
        {
            _lostTimer = 0f;
            return false;
        }

        _lostTimer += Time.deltaTime;

        if (_lostTimer >= _lostTargetReturnTime)
        {
            _lostTimer = 0f; // 상태 전환 전 초기화
            return true;
        }

        return false;
    }

    public override string GetConditionExtraInfo()
    {
        return $"미감지 유예({_lostTargetReturnTime}s)";
    }
}