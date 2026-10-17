using Sirenix.OdinInspector;
using SO.Monster;
using UnityEngine;

public class AlertState : BaseBahaviorState
{
    private float _alertRotationSpeed;

    [Required]
    [SerializeField] private DirectionalAimer _aimer;

    public override void Init()
    {
        MovementConfigSO config = CopyFsmData().BaseFsm.GetConfig<MovementConfigSO>();

        Debug.Assert(config);
        if (!config)
        {
            return;
        }

        _alertRotationSpeed = config.AlertRotationSpeed;
    }


    protected override void Fsm_Step_Enter()
    {
        base.Fsm_Step_Enter();
    }

    protected override void Fsm_Step_Update()
    {
        base.Fsm_Step_Update();
        UpdateAimerTarget();
        _aimer.Tick(_alertRotationSpeed); // 회전
    }

    private void UpdateAimerTarget()
    {
        if (!CopyFsmData().Target)
        {
            return;
        }

        _aimer.SetTarget(CopyFsmData().Target.position);
    }
}