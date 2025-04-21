using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) 
        : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        SetAnimDirection(GetLastDirection());
    }

    public override void Update()
    {
        base.Update();
        
        if(InputVector.x != 0 || InputVector.y != 0)
        {
            lastDirection = InputVector.normalized;
            stateMachine.ChangeState(player.moveState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
