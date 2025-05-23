using UnityEngine;

public class PlayerMoveState : PlayerGroundState
{
    public PlayerMoveState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) 
        : base(_player, _stateMachine, _animBoolName)
    {
    }


    public override void Enter()
    {
        base.Enter();

        if (stateInputVec.x != 0 || stateInputVec.y != 0)
            player.daggerAttackDir = stateInputVec.normalized;
    }

    public override void Update()
    {
        base.Update();

        player.SetVelocity(stateInputVec.x * player.moveSpeed, stateInputVec.y * player.moveSpeed);

        if (stateInputVec.x == 0 && stateInputVec.y == 0)
            stateMachine.ChangeState(player.idleState);
    }

    public override void Exit()
    {
        base.Exit();
    }

}
