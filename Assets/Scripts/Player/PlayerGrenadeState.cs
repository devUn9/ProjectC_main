using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerGrenadeState : PlayerState
{
    public PlayerGrenadeState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) 
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

        player.skill.grenade.RangeActive(true);

        if (Input.GetKeyUp(KeyCode.Alpha1)
            || Input.GetKeyUp(KeyCode.Alpha2)
            || Input.GetKeyUp(KeyCode.Alpha3))
        {
            stateMachine.ChangeState(player.idleState);
        }

        Vector3 dir = PlayerToMousePosVec();
        SetAnimDirection(dir);

    }

    public override void Exit()
    {
        base.Exit();
    }

    
}
