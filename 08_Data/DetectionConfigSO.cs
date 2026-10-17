using Sirenix.OdinInspector;
using UnityEngine;

namespace SO.Monster
{
    [CreateAssetMenu(fileName = "DetectionConfig", menuName = "SO/Monster/Detection Config")]
    public class DetectionConfigSO : ScriptableObject
    {
        [TabGroup("감지")][LabelText("거리 감지 반지름")] public float DistanceRadius = 10f;
        [TabGroup("감지")][LabelText("시야 감지 반지름")] public float ViewRadius = 10f;
        [TabGroup("감지")][LabelText("시야 감지 각도"), Range(0, 360)] public float ViewAngle = 75f;

        [TabGroup("상태")][LabelText("경계 상태 유지 시간")] public float AlertHoldTime = 2f;
        [TabGroup("상태")][LabelText("플레이어 시야 미감지 판정 시간")] public float ActionHoldTime = 10f;
        [TabGroup("상태")][LabelText("플레이어 미감지 판정 시간")] public float LostTargetReturnTime = 5f;
        [TabGroup("상태")][LabelText("스폰 위치 도달 판정 반경")] public float ReturnArrivalRadius = 1.0f;
    }
}