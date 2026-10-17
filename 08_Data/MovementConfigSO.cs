using Sirenix.OdinInspector;
using UnityEngine;

namespace SO.Monster
{
    [CreateAssetMenu(fileName = "MovementConfig", menuName = "SO/Monster/Movement Config")]
    public class MovementConfigSO : ScriptableObject
    {
        [LabelText("기본 이동 속도")]
        public float BaseMoveSpeed = 3f;

        [LabelText("기본 방향 전환 속도")]
        public float BaseRotationSpeed = 10f;

        [LabelText("경계 상태 방향 전환 속도")]
        public float AlertRotationSpeed = 20f;

        [LabelText("행동 전환 시간"), Tooltip("상황 변화 시 다음 행동까지 걸리는 시간")]
        public float ActionTransitionTime = 0.5f;
    }
}