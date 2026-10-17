using UnityEngine;

public abstract class BaseSightCondition : BehaviorStateCondition
{
    protected float _viewRadius;
    protected float _viewAngle;
    [SerializeField] protected bool _exitIfLost = false;
    [SerializeField] private LayerMask _targetMask;
    [SerializeField] private LayerMask _obstacleMask;

    public override bool IsConditionMet(Transform owner, FsmData fsmData, float stateEnterTime)
    {
        Transform target = fsmData.Target;
        if (!target) { return false; }

        bool isVisible = CheckSight(owner, target);

        return _exitIfLost ? !isVisible : isVisible;
    }

    private bool CheckSight(Transform owner, Transform target)
    {
        float dist = Vector3.Distance(owner.position, target.position);
        if (dist > _viewRadius) { return false; }

        Vector3 dirToTarget = (target.position - owner.position).normalized;
        return Vector3.Angle(transform.right, dirToTarget) < _viewAngle / 2f;
    }

    public override string GetConditionExtraInfo()
    {
        return _exitIfLost ? "상실" : "포착";
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector3 origin = transform.position;
        Vector3 right = transform.right; // 2D에서 위쪽이 전방

        float halfAngle = _viewAngle / 2f;

        // --- 부채꼴 채우기 ---
        Gizmos.color = new Color(1f, 1f, 0f, 0.08f);
        DrawFilledArc(origin, right, halfAngle, _viewRadius, 36);

        // --- 부채꼴 외곽선 ---
        Gizmos.color = new Color(1f, 1f, 0f, 0.9f);

        // 좌우 경계선
        Vector3 leftDir = Quaternion.Euler(0f, 0f, halfAngle) * right;
        Vector3 rightDir = Quaternion.Euler(0f, 0f, -halfAngle) * right;
        Gizmos.DrawLine(origin, origin + (leftDir * _viewRadius));
        Gizmos.DrawLine(origin, origin + (rightDir * _viewRadius));

        // 호(arc) 외곽선
        DrawArc(origin, right, halfAngle, _viewRadius, 36);

        // --- 전방 방향선 ---
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.8f);
        Gizmos.DrawLine(origin, origin + (right * _viewRadius));

        // --- 플레이 중: 타겟 레이캐스트 시각화 ---
        if (Application.isPlaying)
        {
            // FsmData 참조가 없으면 스킵 (런타임에만 표시)
            Gizmos.color = Color.red;
        }

        // --- 레이블 ---
        UnityEditor.Handles.color = new Color(1f, 1f, 0f, 1f);
        UnityEditor.Handles.Label(
            origin + (right * (_viewRadius + 0.2f)),
            $"시야 {_viewAngle}°  /  {_viewRadius}m"
        );
    }

    private void DrawArc(Vector3 center, Vector3 forward, float halfAngle, float radius, int segments)
    {
        float step = _viewAngle / segments;
        Vector3 prev = center + (Quaternion.Euler(0f, 0f, halfAngle) * forward * radius);
        for (int i = 1; i <= segments; i++)
        {
            float angle = halfAngle - (step * i);
            Vector3 next = center + (Quaternion.Euler(0f, 0f, angle) * forward * radius);
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }

    private void DrawFilledArc(Vector3 center, Vector3 forward, float halfAngle, float radius, int segments)
    {
        float step = _viewAngle / segments;
        for (int i = 0; i < segments; i++)
        {
            float a1 = halfAngle - (step * i);
            float a2 = halfAngle - (step * (i + 1));
            Vector3 p1 = center + (Quaternion.Euler(0f, 0f, a1) * forward * radius);
            Vector3 p2 = center + (Quaternion.Euler(0f, 0f, a2) * forward * radius);
            Gizmos.DrawLine(center, p1);
            Gizmos.DrawLine(p1, p2);
            Gizmos.DrawLine(p2, center);
        }
    }
#endif
}