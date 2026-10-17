using SO.Monster;

public class EscapeDistanceCondition : BaseDistanceCondition
{
    public override void Init(FsmData fsmData)
        => SetupConfig<EscapeDetectionConfigSO>(fsmData, config => config.EscapeEndDistance);
}
