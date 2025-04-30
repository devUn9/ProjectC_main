using UnityEngine;

public class GrappleStateMachine
{
    public GrappleState currentState { get; private set; }

    public void Initialize(GrappleState _startState)
    {
        currentState = _startState;
        currentState.Enter();
    }

    public void ChangeState(GrappleState _newState)
    {
        currentState.Exit();
        currentState = _newState;
        currentState.Enter();
    }
}
