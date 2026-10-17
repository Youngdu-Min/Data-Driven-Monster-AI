using UnityEngine;

public class Fsm
{
    public enum Step
    {
        Enter,
        Update,
        Exit
    }

    public delegate void State(Fsm fsm, Step step, State state);

    State _behaviorState;
    private float _enterTime;

    public float EnterTime => _enterTime;

    public void Start(State startState)
    {
        TransitionTo(startState);
    }

    public void OnUpdate()
    {
        _behaviorState.Invoke(this, Step.Update, null);
        _enterTime += Time.deltaTime;
    }

    public void TransitionTo(State state)
    {
        _behaviorState?.Invoke(this, Step.Exit, state);
        var oldState = _behaviorState;
        _behaviorState = state;
        _behaviorState.Invoke(this, Step.Enter, oldState);
        _enterTime = 0;
    }
}
