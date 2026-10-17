using System;
using UnityEngine;

public abstract class BaseDistanceCondition : BehaviorStateCondition
{
    protected float _distanceRadius;
    [SerializeField] private bool _exitIfNear = true; // 가까워지면 종료할지, 멀어지면 종료할지

    public float DistanceRadius => _distanceRadius;

    protected void SetupConfig<T>(FsmData fsmData, Func<T, float> valueSelector) where T : ScriptableObject
    {
        T config = fsmData.BaseFsm.GetConfig<T>();

        Debug.Assert(config != null, $"{name}: {typeof(T).Name}를 찾을 수 없습니다.");

        if (config != null)
        {
            _distanceRadius = valueSelector(config);
        }
    }

    public override string GetConditionExtraInfo()
    {
        return _exitIfNear ? "진입" : "이탈";
    }

    public override bool IsConditionMet(Transform owner, FsmData fsmData, float stateEnterTime)
    {
        Transform target = fsmData.Target;
        if (!target) return false;

        float dist = Vector3.Distance(owner.position, target.position);
        return _exitIfNear ? (dist <= _distanceRadius) : (dist >= _distanceRadius);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // 범위 원
        Gizmos.color = _exitIfNear
            ? new Color(1f, 0.3f, 0.3f, 0.3f)  // 가까워지면 종료: 빨강 (진입 감지)
            : new Color(0.3f, 0.6f, 1f, 0.3f);  // 멀어지면 종료: 파랑 (이탈 감지)
        Gizmos.DrawSphere(transform.position, _distanceRadius);

        // 범위 원 외곽선
        Gizmos.color = _exitIfNear
            ? new Color(1f, 0.3f, 0.3f, 1f)
            : new Color(0.3f, 0.6f, 1f, 1f);
        DrawCircle(transform.position, _distanceRadius, 36);

        // 모드 레이블 (Scene 뷰 텍스트)
        UnityEditor.Handles.color = Gizmos.color;
        string label = _exitIfNear ? $"진입 감지\n(<= {_distanceRadius})" : $"이탈 감지\n(>= {_distanceRadius})";
        UnityEditor.Handles.Label(transform.position + Vector3.up * (_distanceRadius + 0.2f), label);
    }

    private void DrawCircle(Vector3 center, float radius, int segments)
    {
        float angleStep = 360f / segments;
        Vector3 prev = center + new Vector3(radius, 0f, 0f);
        for (int i = 1; i <= segments; i++)
        {
            float rad = Mathf.Deg2Rad * (angleStep * i);
            Vector3 next = center + new Vector3(Mathf.Cos(rad) * radius, Mathf.Sin(rad) * radius, 0f);
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }
#endif
}