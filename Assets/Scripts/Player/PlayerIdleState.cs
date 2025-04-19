using UnityEngine;
using UnityEngine.Windows;

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

    public Vector2 GetLastDirection()
    {
        return lastDirection;
    }

    public void SetAnimDirection(Vector2 _Velocity)
    {
        player.anim.SetFloat("VelocityX", _Velocity.x);
        player.anim.SetFloat("VelocityY", _Velocity.y);
    }
}
