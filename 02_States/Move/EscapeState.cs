using PathFind;
using UnityEngine;

public class EscapeState : MovableState
{
    public override void Move()
    {
        _direction = (transform.position - CopyFsmData().Target.position).normalized;

        // 경로 결정 (도망 지점 계산)
        Vector3 escapeTarget = transform.position + (_direction * 5f);
        _currentPath = Pathfinding.FindPath(_pathGrid, GetGridPoint(transform.position), GetGridPoint(escapeTarget));

        // 이동 실행 요청
        StartMoving();
    }
}