using SO.Monster;
using UnityEngine;

public class AttackTimeCondition : BaseTimeCondition
{
    public override void Init(FsmData fsmData)
    {
        AttackConfigSO config = fsmData.BaseFsm.GetConfig<AttackConfigSO>();
        Debug.Assert(config is not null);
        if (!config) return;

        _targetTime = config.CancelTime;
    }

}
