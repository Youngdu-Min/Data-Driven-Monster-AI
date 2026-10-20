using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using SO.Monster;
using System;
using System.Threading;
using UnityEngine;
using static MonsterInterface;

public class StabilizeHandler : MonoBehaviour, IGetMediator
{
    [SerializeField] private BaseStabilizeConfigSO _config;

    [ShowInInspector, ReadOnly] private int _stabilizeStack = 0;
    [ShowInInspector, ReadOnly] private int MaxStabilizeStack => _config != null ? _config.MaxStabilizeStack : 0;

    private CancellationTokenSource _decayCts;
    private CancellationTokenSource _stabilizeCts;
    private CancellationTokenSource _immunityCts;

    [field: ShowInInspector, ReadOnly]
    public bool IsStabilized { get; private set; } = false;
    [ShowInInspector, ReadOnly] private bool _restabilizeImmunity = false;
    private ISetMultiplier[] _multipliers;
    private MonsterMediator _mediator;

    private void Init()
    {
        _multipliers = GetComponentsInChildren<ISetMultiplier>();
    }

    private void Awake() => Init();

    private void Reset() => Init();

    private void OnDestroy()
    {
        CancelAndDispose(ref _decayCts);
        CancelAndDispose(ref _stabilizeCts);
        CancelAndDispose(ref _immunityCts);
    }

    [ContextMenu("AddStack")]
    public void AddStack()
    {
        if (IsStabilized || _restabilizeImmunity) { return; }

        SetStabilizeStack(_stabilizeStack + 1);
        CancelAndDispose(ref _decayCts);

        if (_stabilizeStack >= _config.MaxStabilizeStack)
        {
            _stabilizeCts = new CancellationTokenSource();
            StabilizeAsync(_stabilizeCts.Token).Forget();
        }
        else
        {
            _decayCts = new CancellationTokenSource();
            DecayStackAsync(_decayCts.Token).Forget();
        }
    }

    private void SetStabilizeStack(int value)
        => _stabilizeStack = Mathf.Clamp(value, 0, _config.MaxStabilizeStack);

    private async UniTaskVoid DecayStackAsync(CancellationToken ct)
    {
        Debug.Log($"[Stabilize] Decay 대기 중... ({_config.StackDecayDelay}s)");
        await UniTask.WaitForSeconds(_config.StackDecayDelay, cancellationToken: ct);

        while (!ct.IsCancellationRequested)
        {
            SetStabilizeStack(_stabilizeStack - _config.StackDecayAmount);
            Debug.Log($"[Stabilize] 스택 감소 → {_stabilizeStack} / {_config.MaxStabilizeStack}");
            if (_stabilizeStack <= 0)
            {
                Debug.Log("[Stabilize] 스택 소진");
                return;
            }
            await UniTask.WaitForSeconds(_config.StackDecayInterval, cancellationToken: ct);
        }
        Debug.Log("[Stabilize] Decay 취소됨");
    }

    private async UniTaskVoid StabilizeAsync(CancellationToken ct)
    {
        IsStabilized = true;
        _mediator?.OnNotify(this, "OnStabilized");
        OnStabilizeEnter();

        await UniTask.WaitForSeconds(_config.StabilizeDuration, cancellationToken: ct);

        IsStabilized = false;
        SetStabilizeStack(0);
        _mediator?.OnNotify(this, "OnStabilizeExpired");
        OnStabilizeExit();
    }

    private void OnStabilizeEnter()
    {
        CombatStabilizeConfigSO combatConfig = _config as CombatStabilizeConfigSO;

        foreach (MonsterInterface.ISetMultiplier multiplier in _multipliers)
        {
            float targetMultiplier = multiplier switch
            {
                AttackState => combatConfig != null ? combatConfig.StabilizeAttackPrepMultiplier : 1f,
                MovableState => _config.StabilizeMoveMultiplier,
                DirectionalAimer => _config.StabilizeRotationMultiplier,
                BaseFsm => _config.StabilizeReactionMultiplier,
                _ => throw new NotImplementedException()
            };

            multiplier.SetMultiplier(targetMultiplier);
        }
    }

    private void OnStabilizeExit()
    {
        _ = _multipliers.ForEach(multiplier => multiplier.ResetMultiplier());

        _immunityCts = new CancellationTokenSource();
        SetRestabilizeImmunity(_immunityCts.Token).Forget();
    }

    private async UniTaskVoid SetRestabilizeImmunity(CancellationToken ct)
    {
        _restabilizeImmunity = true;
        await UniTask.WaitForSeconds(_config.RestabilizeImmunityTime, cancellationToken: ct);
        _restabilizeImmunity = false;
    }

    private void CancelAndDispose(ref CancellationTokenSource cts)
    {
        cts?.Cancel();
        cts?.Dispose();
        cts = null;
    }

    public void SetMediator(MonsterMediator mediator)
        => _mediator = mediator;
}