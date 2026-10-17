using SO.Monster;

public class DetectionDistanceCondition : BaseDistanceCondition
{
    public override void Init(FsmData fsmData)
        => SetupConfig<DetectionConfigSO>(fsmData, config => config.DistanceRadius);
}
