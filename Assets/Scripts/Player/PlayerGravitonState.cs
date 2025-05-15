using UnityEngine;

public class PlayerGravitonState : PlayerState
{
    public PlayerGravitonState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName)
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
        player.ChargeEffect();
        player.SetVelocity(0, 0);
        player.remainingSkillAnimation = 0.34f;

        if (Input.GetKeyUp(KeyCode.E))
        {
            player.transform.position -= PlayerToMousePosVec().normalized * 0.4f;

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
