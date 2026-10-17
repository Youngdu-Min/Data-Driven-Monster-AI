using PathFind;

public class ChaseState : MovableState
{
    public override void Move()
    {
        _direction = (CopyFsmData().Target.position - transform.position).normalized;

        // 경로 결정 (Pathfinding 사용)
        Point start = GetGridPoint(transform.position);
        Point end = GetGridPoint(CopyFsmData().Target.position);
        _currentPath = Pathfinding.FindPath(_pathGrid, start, end);

        // 부모에게 이동 실행 요청
        StartMoving();
    }
}