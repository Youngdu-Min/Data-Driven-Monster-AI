using Sirenix.OdinInspector;

public class StatusEffectStat
{
    // [ShowInInspector] public StatusEffectType;
    [ShowInInspector] public float StatusEffectDuration { get; private set; }
    [ShowInInspector] public float StatusEffectResistance { get; private set; }
    [ShowInInspector] public bool StatusEffectImmuneFlag { get; private set; }
    // [ShowInInspector] public CurrentStatusEffect;
    // [ShowInInspector] public StatusEffectApplyCondition;
    // [ShowInInspector] public StatusEffectRemoveCondition;
}
