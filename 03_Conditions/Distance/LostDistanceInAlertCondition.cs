using SO.Monster;
using UnityEngine;

public class LostDistanceInAlertCondition : DistanceDelayCondition
{
    public override void Init(FsmData fsmData)
    {
        DetectionConfigSO config = fsmData.BaseFsm.GetConfig<DetectionConfigSO>();
        Debug.Assert(config);

        if (!config) return;

        _holdTime = config.AlertHoldTime;
    }

    private void Reset()
    {
        _distanceCondition = GetComponent<DetectionDistanceCondition>();
    }

}
