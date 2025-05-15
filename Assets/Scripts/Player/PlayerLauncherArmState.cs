using UnityEngine;

public class PlayerLauncherArmState : PlayerState
{
    public PlayerLauncherArmState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) 
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
        player.SetVelocity(0, 0);

        Vector3 dir = PlayerToMousePosVec();
        SetAnimDirection(dir);
        player.remainingSkillAnimation = 0.625f;

        if (triggerCalled)
        {
            SetFinalAttkInputVec();
            stateMachine.ChangeState(player.idleState);
        }
        
        
    }
    public override void Exit()
    {
        base.Exit();
    }

}
