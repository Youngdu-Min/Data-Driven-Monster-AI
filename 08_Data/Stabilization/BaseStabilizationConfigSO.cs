using Sirenix.OdinInspector;
using UnityEngine;

namespace SO.Monster
{
    [CreateAssetMenu(fileName = "BaseStabilizationConfig", menuName = "SO/Monster/Stabilization/BaseStabilizationConfig")]
    public class BaseStabilizationConfigSO : SerializedScriptableObject
    {
        [SerializeField] private int _maxStabilizeStack = 10;
        [HideInInspector] public float StackDecayDelay = 3f;
        [HideInInspector] public float StackDecayInterval = 1f;
        [HideInInspector] public int StackDecayAmount = 1;

        public float StabilizeDuration = 3f;
        [HideInInspector] public int RestabilizeImmunityTime = 1;

        [HideInInspector] public float StabilizeMoveMultiplier = 0.5f;
        [HideInInspector] public float StabilizeRotationMultiplier = 0.5f;
        [HideInInspector] public float StabilizeReactionMultiplier = 1.5f;
        [HideInInspector] public readonly bool CanCollectWhileStabilized = true;

        public int MaxStabilizeStack => _maxStabilizeStack;
    }
}