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

        if (player.attackStatusRemainTime > 0)
            SetAnimDirection(player.finalAttackInputVec);
        else if (player.beforeState == "daggerAttack")
        {
            Debug.Log("beforeState : " + player.beforeState);
            //SetAnimDirection(player.daggerAttackDir);
            SetAnimDirection(player.lastDirection);
        }
        else
        {
            Debug.Log("move:lastDirection : " + lastDirection);
            SetAnimDirection(lastDirection);
        }

    }

    public override void Update()
    {
        base.Update();
        if(player.beforeState == "daggerAttack")
            //SetAnimDirection(player.daggerAttackDir);
            SetAnimDirection(player.lastDirection);
        player.SetVelocity(0,0);
    }

    public override void Exit()
    {
        base.Exit();
        if (player.beforeState == "daggerAttack")
            player.beforeState = "Idle";
    }
}
