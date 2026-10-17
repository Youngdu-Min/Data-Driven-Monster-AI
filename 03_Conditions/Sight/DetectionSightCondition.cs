using SO.Monster;
using UnityEngine;

public class DetectionSightCondition : BaseSightCondition
{
    public override void Init(FsmData fsmData)
    {
        DetectionConfigSO config = fsmData.BaseFsm.GetConfig<DetectionConfigSO>();
        Debug.Assert(config);
        if (!config) return;

        _viewRadius = config.ViewRadius;
        _viewAngle = config.ViewAngle;

    }
}