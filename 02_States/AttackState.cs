using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using SO.Monster;
using System;
using System.Threading;
using UnityEngine;

public class AttackState : BaseBahaviorState, MonsterInterface.ISetMultiplier
{
    [Header("Components")]
    [SerializeField] private DirectionalAimer _aimer;
    [SerializeField] private SpriteFlipper _flipper;

    private float _prebTime;
    private float _attackRange;
    private int _damage;
    private float _cooltime;

    [ShowInInspector, ReadOnly]
    private float _currentAttackPrepTime;
    private float _currentAttackCooltime;
    private float _attackTimeMultiplier = 1;

    private CancellationTokenSource _cts;
    private PlayerController _target;

    public override void Init()
    {
        AttackConfigSO config = CopyFsmData().BaseFsm.GetConfig<AttackConfigSO>();
        Debug.Assert(config);
        if (!config) { return; }

        _prebTime = config.PrepTime;
        _cooltime = config.Cooltime;
        _attackRange = config.AttackRange;
        _damage = config.Damage;
    }

    protected override void Fsm_Step_Enter()
    {
        base.Fsm_Step_Enter();

        FsmData data = CopyFsmData();
        if (data.Target != null)
        {
            _target = data.Target.GetComponent<PlayerController>();
        }

        _currentAttackPrepTime = _prebTime * _attackTimeMultiplier;
        _currentAttackCooltime = _cooltime * _attackTimeMultiplier;
        TriggerAttackLoop().Forget();
    }

    protected override void Fsm_Step_Update()
    {
        base.Fsm_Step_Update();

        if (!_target) { return; }

        Vector3 targetPos = _target.transform.position;
        _aimer?.SetTarget(targetPos);

        if (_flipper != null)
        {
            bool isLeft = targetPos.x < transform.position.x;
            _flipper.SetFlip(isLeft);
        }
    }

    private async UniTaskVoid TriggerAttackLoop()
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        CancellationToken token = _cts.Token;

        try
        {
            while (!token.IsCancellationRequested)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_currentAttackPrepTime), cancellationToken: token);

                if (CopyFsmData().Mediator.IsStabilized) { return; }

                ExecuteAttackQuery();

                CopyFsmData().BaseFsm.SetBlockTransition(true);
                await UniTask.Delay(TimeSpan.FromSeconds(_currentAttackCooltime), cancellationToken: token);
                CopyFsmData().BaseFsm.SetBlockTransition(false);
            }
        }
        catch (OperationCanceledException) { }
    }

    private void ExecuteAttackQuery()
    {
        if (_target == null) { return; }

        float dist = Vector3.Distance(transform.position, _target.transform.position);

        if (dist > _attackRange) { return; }

        _target.TakeDamage(_damage);

    }

    protected override void Fsm_Step_Exit()
    {
        base.Fsm_Step_Exit();
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
        _target = null;
    }

    public void SetMultiplier(float multiplier) => _attackTimeMultiplier *= multiplier;
    public void ResetMultiplier() => _attackTimeMultiplier = 1;

    private void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }
}