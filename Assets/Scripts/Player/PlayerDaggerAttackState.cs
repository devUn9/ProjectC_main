using UnityEngine;

public class PlayerDaggerAttackState : PlayerState
{
    public PlayerDaggerAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
        player.beforeState = "daggerAttack";
        player.attackStateTimer = 0.6f;
    }
    public override void Exit()
    {
        base.Exit();
    }

}
