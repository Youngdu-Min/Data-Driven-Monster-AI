using PathFind;
using UnityEngine;

public class ReturnState : MovableState
{
    private Vector3 _returnPos;

    public override void Init()
    {
        base.Init();
        _returnPos = transform.position;
    }

    public override void Move()
    {
        _direction = (_returnPos - transform.position).normalized;

        // 경로 결정
        Point start = GetGridPoint(transform.position);
        Point end = GetGridPoint(_returnPos);
        _currentPath = Pathfinding.FindPath(_pathGrid, start, end);

        // 이동 실행 요청
        StartMoving();
    }
}
