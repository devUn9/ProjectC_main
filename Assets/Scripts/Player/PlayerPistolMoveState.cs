using UnityEngine;

public class PlayerPistolMoveState : PlayerState
{
    public PlayerPistolMoveState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) 
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

        player.attackStateTimer = 0.5f;
        player.attackStatusRemainTime = 0.5f;
        player.isMovingAttack = true;

        player.SetVelocity(stateInputVec.x * player.moveSpeed, stateInputVec.y * player.moveSpeed);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
