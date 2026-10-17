using Sirenix.OdinInspector;

public class SleepEffectStat
{
    [ShowInInspector] public float SleepGauge { get; private set; }
    [ShowInInspector] public float SleepGaugeMax { get; private set; }
    [ShowInInspector] public float SleepIncreaseRate { get; private set; }
    [ShowInInspector] public float SleepDecayRate { get; private set; }
    [ShowInInspector] public float SleepDuration { get; private set; }
    [ShowInInspector] public float SleepResistance { get; private set; }
    [ShowInInspector] public float SleepAreaRadius { get; private set; }
    [ShowInInspector] public float SleepAreaDuration { get; private set; }
}
