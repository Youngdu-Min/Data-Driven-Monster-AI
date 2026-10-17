using UnityEngine;

public abstract class BehaviorStateCondition : MonoBehaviour
{
    public abstract void Init(FsmData fsmData);

    // 조건이 충족되었는지 여부를 리턴
    public abstract bool IsConditionMet(Transform owner, FsmData fsmData, float stateEnterTime);
    public virtual string GetConditionExtraInfo() => "";
}