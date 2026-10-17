using UnityEngine;

public abstract class BaseTimeCondition : BehaviorStateCondition
{
    protected float _targetTime;

    public override bool IsConditionMet(Transform owner, FsmData fsmData, float stateEnterTime)
    {
        return stateEnterTime >= _targetTime;
    }
}