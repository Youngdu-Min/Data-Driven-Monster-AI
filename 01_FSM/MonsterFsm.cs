using UnityEngine;

public class MonsterFsm : BaseFsm
{
    [SerializeField] private BaseBahaviorState _state;

    protected override void Fsm_IdleState(Fsm fsm, Fsm.Step step, Fsm.State state)
    {
        base.Fsm_IdleState(fsm, step, state);
        if (step == Fsm.Step.Enter)
        {
            // _anim.CrossFade(Anim_Idle, 0);
            TransitionToState(_state);
        }
        else if (step == Fsm.Step.Update)
        {

        }
        else if (step == Fsm.Step.Exit)
        {

        }
    }
}
