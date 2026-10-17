using Sirenix.OdinInspector;
using UnityEngine;

public abstract class DistanceDelayCondition : BehaviorStateCondition
{
    [SerializeField][Required] protected BaseDistanceCondition _distanceCondition;
    protected float _holdTime;
    private float _internalTimer = 0f;

    public override bool IsConditionMet(Transform owner, FsmData fsmData, float stateEnterTime)
    {
        _internalTimer += Time.deltaTime;

        if (_internalTimer < _holdTime) return false;

        bool inRange = _distanceCondition.IsConditionMet(owner, fsmData, stateEnterTime);

        _internalTimer = 0f;

        return !inRange;
    }
}
