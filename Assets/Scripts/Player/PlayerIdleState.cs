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

        if(player.attackStatusRemainTime > 0)
            SetAnimDirection(player.finalAttackInputVec);
        else
            SetAnimDirection(lastDirection);
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
