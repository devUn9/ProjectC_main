using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class PlayerIdleState : PlayerGroundState
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
        else if (player.beforeState.Equals("daggerAttack"))
        {
            Debug.Log("beforeState : " + player.beforeState);
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
        if (player.beforeState.Equals("daggerAttack"))
            SetAnimDirection(player.lastDirection);

        // 이동
        if (stateInputVec.x != 0 || stateInputVec.y != 0)
        {
            lastDirection = stateInputVec.normalized;
            stateMachine.ChangeState(player.moveState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        if (player.beforeState.Equals("daggerAttack"))
            player.beforeState = "Idle";
    }
}
