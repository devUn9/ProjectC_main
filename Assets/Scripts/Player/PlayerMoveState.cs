using UnityEngine;
using UnityEngine.Windows;

public class PlayerMoveState : PlayerState
{
    protected float velocityX;
    protected float velocityY;

    public PlayerMoveState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) 
        : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        player.SetVelocity(InputVector.x * player.moveSpeed, InputVector.y * player.moveSpeed);

        player.anim.SetFloat("VelocityX", InputVector.x);
        player.anim.SetFloat("VelocityY", InputVector.y);

        if (InputVector.x == 0 && InputVector.y == 0)
            stateMachine.ChangeState(player.idleState);
    }

    public override void Exit()
    {
        base.Exit();
    }

}
