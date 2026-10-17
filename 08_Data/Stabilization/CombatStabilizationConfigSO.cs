using UnityEngine;

namespace SO.Monster
{
    // [CreateAssetMenu(fileName = "CombatStabilizationConfig", menuName = "SO/Monster/Stabilization/CombatStabilizationConfig")]
    public class CombatStabilizationConfigSO : BaseStabilizationConfigSO
    {
        [HideInInspector] public float StabilizeAttackPrepMultiplier = 1.5f;
    }
}