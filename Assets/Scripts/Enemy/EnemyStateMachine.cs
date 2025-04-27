using UnityEngine;

public class EnemyStateMachine
{
    public EnemyState currentState { get; private set; }

    public void Initialize(EnemyState _startState)
    {
        currentState = _startState;
        currentState.Enter();
    }

    public void ChangeState(EnemyState _changeState)
    {
        currentState.Exit();
        currentState = _changeState;
        currentState.Enter();
    }
}
