using PathFind;
using UnityEngine;

public class WanderState : MovableState
{
    [Header("Wander Settings")]
    [SerializeField] private float _wanderRadius = 5f;       // 배회 반경 (원점 기준)
    [SerializeField] private float _minWaitTime = 0.5f;      // 목적지 도착 후 최소 대기 시간
    [SerializeField] private float _maxWaitTime = 2f;        // 목적지 도착 후 최대 대기 시간
    [SerializeField] private int _maxPickAttempts = 10;      // walkable 지점 탐색 최대 시도 횟수

    [Header("Turn Slowdown Settings")]
    [SerializeField, Range(0f, 1f)] private float _turnMinSpeedRatio = 0.3f; // 180도 방향 전환 시 속도 비율
    [SerializeField] private float _turnRecoverSpeed = 3f;                    // 감속 후 원래 속도로 복귀하는 러프 속도

    private Vector3 _wanderOrigin;   // 배회 중심점 (Enter 시점의 위치로 고정)
    private bool _isMoving = false;
    private float _waitTimer = 0f;
    private bool _isWaiting = false;

    private Vector3 _lastMoveDir;          // 직전 프레임 이동 방향
    private float _turnSpeedRatio = 1f;    // 방향 전환 감속 비율 (1 = 감속 없음)

    protected override void Fsm_Step_Enter()
    {
        base.Fsm_Step_Enter();
        _wanderOrigin = transform.position;
        _isMoving = false;
        _isWaiting = false;
        _waitTimer = 0f;
        _lastMoveDir = Vector3.zero;
        _turnSpeedRatio = 1f;

        // 진입 즉시 첫 목적지로 이동 시작
        Move();
    }

    protected override void Fsm_Step_Update()
    {
        base.Fsm_Step_Update();

        // 대기 중이면 타이머 소진 후 다음 목적지 탐색
        if (_isWaiting)
        {
            _waitTimer -= Time.deltaTime;
            if (_waitTimer <= 0f)
            {
                _isWaiting = false;
                Move();
            }
            return;
        }

        // 이동 중이 아니면(경로 완료) 대기 시작
        if (!_isMoving)
        {
            StartWaiting();
        }
    }

    public override void Move()
    {
        Vector3? target = PickWalkablePoint();
        if (target == null)
        {
            // walkable 지점을 못 찾으면 잠시 대기 후 재시도
            StartWaiting();
            return;
        }

        _direction = (target.Value - transform.position).normalized;
        _currentPath = Pathfinding.FindPath(_pathGrid, GetGridPoint(transform.position), GetGridPoint(target.Value));

        if (_currentPath == null || _currentPath.Count == 0)
        {
            // 경로 탐색 실패 시 대기 후 재시도
            StartWaiting();
            return;
        }

        _isMoving = true;
        StartMovingWithCallback();
    }

    // 방향이 꺾인 각도만큼 감속한 뒤 러프로 원래 속도까지 복귀
    protected override float GetMoveSpeed(Vector3 moveDir)
    {
        if (_lastMoveDir != Vector3.zero && moveDir != Vector3.zero)
        {
            float angle = Vector3.Angle(_lastMoveDir, moveDir);
            float targetRatio = Mathf.Lerp(1f, _turnMinSpeedRatio, angle / 180f);
            _turnSpeedRatio = Mathf.Min(_turnSpeedRatio, targetRatio);
        }
        _lastMoveDir = moveDir;

        _turnSpeedRatio = Mathf.Lerp(_turnSpeedRatio, 1f, _turnRecoverSpeed * Time.deltaTime);
        return base.GetMoveSpeed(moveDir) * _turnSpeedRatio;
    }

    // 배회 반경 내에서 walkable한 랜덤 지점을 반환 (없으면 null)
    private Vector3? PickWalkablePoint()
    {
        for (int i = 0; i < _maxPickAttempts; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * _wanderRadius;
            Vector3 candidate = _wanderOrigin + new Vector3(randomCircle.x, randomCircle.y, 0f);

            Point gridPoint = GetGridPoint(candidate);
            if (_pathGrid.nodes[gridPoint.x, gridPoint.y].walkable)
            {
                return candidate;
            }
        }
        return null;
    }

    private void StartWaiting()
    {
        _isMoving = false;
        _isWaiting = true;
        _waitTimer = Random.Range(_minWaitTime, _maxWaitTime);
    }

    // 경로 완료 콜백을 위해 부모의 StartMoving을 래핑
    private void StartMovingWithCallback()
    {
        WaitForPathComplete().Forget();
    }

    private async Cysharp.Threading.Tasks.UniTaskVoid WaitForPathComplete()
    {
        // 경로 이동 시작 (부모 메서드 호출)
        StartMoving();

        // 경로 완료까지 대기: _currentPath 길이 * 예상 소요 시간을 타임아웃으로 활용
        // CancellationToken이 취소되면 자동 종료
        float timeout = (_currentPath != null ? _currentPath.Count : 1) * 10f;
        float elapsed = 0f;

        while (_isMoving && elapsed < timeout)
        {
            elapsed += UnityEngine.Time.deltaTime;

            // 목적지(마지막 waypoint)에 도달했는지 체크
            if (_currentPath != null && _currentPath.Count > 0)
            {
                Vector3 finalTarget = PathfindingManager.Instance.GridToWorld(_currentPath[_currentPath.Count - 1]);
                if (Vector3.Distance(CopyFsmData().BaseFsm.transform.position, finalTarget) <= 0.1f)
                {
                    _isMoving = false;
                    break;
                }
            }

            await Cysharp.Threading.Tasks.UniTask.Yield(
                Cysharp.Threading.Tasks.PlayerLoopTiming.Update,
                _moveCts.Token
            );
        }

        if (_isMoving) _isMoving = false;
    }
}