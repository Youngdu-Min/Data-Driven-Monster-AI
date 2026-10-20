using Cysharp.Threading.Tasks;
using PathFind;
using Sirenix.OdinInspector;
using SO.Monster;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public abstract class MovableState : BaseBahaviorState, MonsterInterface.ISetMultiplier
{
    private float _speed;
    private float _speedMultiplier = 1f;
    protected Vector3 _direction;
    [ShowInInspector] protected float CurrentSpeed => _speed * _speedMultiplier;

    // Pathfinding 관련 필드
    protected List<Point> _currentPath;
    protected CancellationTokenSource _moveCts;
    protected PathFind.Grid _pathGrid;
    protected PathfindingManager _pathManager;

    [SerializeField] private bool _flipToMovingDirection = true;
    [SerializeField] private bool _aimToMovingDirection = true;
    [SerializeField] private SpriteFlipper _spriteFlipper;
    [SerializeField] private DirectionalAimer _aimer;
    private float _rotationSpeed;

    private float _recalcTimer = 0f;
    private const float RECALC_INTERVAL = 0.5f; // 0.5초마다 경로 재계산

    public Action OnMove;

    public override void Init()
    {
        InitConfig();
        _pathManager = PathfindingManager.Instance;
        if (_pathManager != null)
        {
            _pathGrid = _pathManager.Grid;
        }
        else
        {
            Debug.LogError("씬에 PathfindingManager가 없습니다!");
        }

        OnMove += () =>
        {
            if (_spriteFlipper != null)
            {
                bool shouldFlip = _direction.x < 0;
                if (!_flipToMovingDirection) { shouldFlip = !shouldFlip; }
                _spriteFlipper.SetFlip(shouldFlip);
            }

            if (CopyFsmData().Target != null)
            {
                _aimer?.SetTarget(CopyFsmData().Target.position);
                _aimer?.Tick(_rotationSpeed);
            }
        };
    }

    public virtual void InitConfig()
    {
        MovementConfigSO _config = CopyFsmData().BaseFsm.GetConfig<MovementConfigSO>();
        Debug.Assert(_config);
        if (!_config) { return; }
        _speed = _config.BaseMoveSpeed;
        _rotationSpeed = _config.BaseRotationSpeed;
    }

    protected override void Fsm_Step_Enter()
    {
        base.Fsm_Step_Enter();
        _moveCts = new CancellationTokenSource();
    }

    protected override void Fsm_Step_Update()
    {
        base.Fsm_Step_Update();
        if (!CopyFsmData().Target) { return; }

        _recalcTimer += Time.deltaTime;
        if (_recalcTimer >= RECALC_INTERVAL)
        {
            _recalcTimer = 0f;
            Move(); // 경로 재계산
        }
    }

    protected override void Fsm_Step_Exit()
    {
        base.Fsm_Step_Exit();
        _moveCts?.Cancel();
        _moveCts?.Dispose();
    }

    public void SetMultiplier(float multiplier) => _speedMultiplier *= multiplier;
    public void ResetMultiplier() => _speedMultiplier = 1;

    public abstract void Move();

    // 이동 방향에 따라 속도를 보정할 수 있도록 하위 상태에 열어둔 훅
    protected virtual float GetMoveSpeed(Vector3 moveDir) => CurrentSpeed;

    protected Point GetGridPoint(Vector3 worldPos)
    {
        if (_pathManager == null) return new Point(0, 0);
        return _pathManager.WorldToGrid(worldPos);
    }

    protected void StartMoving()
    {
        if (!_moveCts.IsCancellationRequested)
        {
            _moveCts?.Cancel();
        }
        _moveCts = new CancellationTokenSource();
        FollowPathAsync(_moveCts.Token).Forget();
    }

    private async UniTaskVoid FollowPathAsync(CancellationToken ct)
    {
        // 경로 탐색 실패 시 
        if (_currentPath == null || _currentPath.Count == 0)
        {
            await TryStraightMoveAsync(ct);
            return;
        }

        foreach (Point point in _currentPath)
        {
            Vector3 targetPos = PathfindingManager.Instance.GridToWorld(point);
            while (Vector3.Distance(CopyFsmData().BaseFsm.transform.position, targetPos) > 0.05f)
            {
                Vector3 currentPos = CopyFsmData().BaseFsm.transform.position;
                Vector3 moveDir = (targetPos - currentPos).normalized;
                CopyFsmData().BaseFsm.transform.position = Vector3.MoveTowards(
                    currentPos, targetPos, GetMoveSpeed(moveDir) * Time.deltaTime);

                OnMove?.Invoke();
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }
        }
    }

    private async UniTask<bool> TryStraightMoveAsync(CancellationToken ct)
    {
        Debug.Log($"<color=yellow>[Path] 직선 이동 재시도</color>");

        float straightMoveDuration = 0.5f;
        float timer = 0f;

        Vector3 moveDir = _direction.normalized;

        while (timer < straightMoveDuration)
        {
            timer += Time.deltaTime;

            CopyFsmData().BaseFsm.transform.position += moveDir * (GetMoveSpeed(moveDir) * Time.deltaTime);

            OnMove?.Invoke(); // 회전 및 애니메이션 갱신 유지
            await UniTask.Yield(PlayerLoopTiming.Update, ct);

        }

        return true;
    }
}