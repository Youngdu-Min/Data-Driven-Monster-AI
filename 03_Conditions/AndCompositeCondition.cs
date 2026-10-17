using System.Collections.Generic;
using UnityEngine;

// 여러 조건을 하나로 묶는 복합 컨디션
public class AndCompositeCondition : BehaviorStateCondition
{
    [SerializeField] private List<BehaviorStateCondition> _subConditions;

    public override void Init(FsmData fsmData) { }

    public override bool IsConditionMet(Transform owner, FsmData fsmData, float stateEnterTime)
    {
        // 모든 하위 조건이 만족되어야 함 (기획서의 복합 상태 판정 최적화)
        foreach (BehaviorStateCondition condition in _subConditions)
        {
            if (!condition.IsConditionMet(owner, fsmData, stateEnterTime)) { return false; }
        }
        return true;
    }
}