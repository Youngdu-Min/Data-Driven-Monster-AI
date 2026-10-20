using UnityEngine;

namespace SO.Monster
{
    // [CreateAssetMenu(fileName = "CombatStabilizeConfigSO", menuName = "SO/Monster/Stabilize/CombatStabilizeConfigSO")]
    public class CombatStabilizeConfigSO : BaseStabilizeConfigSO
    {
        [HideInInspector] public float StabilizeAttackPrepMultiplier = 1.5f;
    }
}