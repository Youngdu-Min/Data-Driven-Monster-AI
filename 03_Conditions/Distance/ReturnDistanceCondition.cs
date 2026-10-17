using SO.Monster;

public class ReturnDistanceCondition : BaseDistanceCondition
{
    public override void Init(FsmData fsmData)
        => SetupConfig<DetectionConfigSO>(fsmData, config => config.ReturnArrivalRadius);
}
