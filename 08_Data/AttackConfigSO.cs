using Sirenix.OdinInspector;
using UnityEngine;

namespace SO.Monster
{
    [CreateAssetMenu(fileName = "AttackConfig", menuName = "SO/Monster/AttackConfig")]
    public class AttackConfigSO : ScriptableObject
    {
        [Title("범위 설정")]
        [LabelText("공격 개시 범위"), Tooltip("추적 중 공격을 시작하는 거리 (기본: 1.5)")]
        public float AttackRange = 1.2f;

        [Title("타이밍 설정")]
        [LabelText("공격 전 딜레이 (Prep)")]
        public float PrepTime = 0.8f;

        [LabelText("공격 쿨타임")]
        public float Cooltime = 1.5f;

        [LabelText("범위 벗어난 이후 경직")]
        [ShowInInspector, ReadOnly]
        public float CancelTime = 3f;

        [LabelText("데미지")]
        public int Damage = 1;
    }
}