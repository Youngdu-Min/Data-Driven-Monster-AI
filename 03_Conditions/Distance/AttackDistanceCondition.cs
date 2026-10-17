using SO.Monster;

public class AttackDistanceCondition : BaseDistanceCondition
{
    public override void Init(FsmData fsmData)
        => SetupConfig<AttackConfigSO>(fsmData, config => config.AttackRange);
}
