using UnityEngine;

public class PlayerGroundState : PlayerState
{
    public PlayerGroundState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        // 공격
        // 제자리 공격 애니매이션 종료 후 AnimationTrigger 발생 전까지 공격 입력불가능하게 조정
        // AnimationTrigger 발생 전 입력 시 state에서 에러발생 attackStateTimer변수는 player에서 관리
        if (Input.GetKeyDown(KeyCode.Mouse0) && player.attackStateTimer < 0.25)
        {
            if (animBoolName.Equals("Move") && !player.isDaggerAttack)
                stateMachine.ChangeState(player.pistolMove);
            else
                stateMachine.ChangeState(player.attackState);
        }

        if (Input.GetKeyDown(KeyCode.Space) && player.attackStateTimer < 0.25)
        {
            player.MeleeAttack();
            if (player.isDaggerAttack)
            {
                stateMachine.ChangeState(player.daggerAttack);
            }
        }

        // 수류탄 스킬 사용
        if ((Input.GetKeyDown(KeyCode.Alpha1)
            || Input.GetKeyDown(KeyCode.Alpha2)
            || Input.GetKeyDown(KeyCode.Alpha3)) && player.skill.grenade.CanUseBool())
        {
            player.skill.grenade.getKeyLock = false;
            stateMachine.ChangeState(player.grenadeSkill);
        }

        // 런처암 스킬 사용
        if (Input.GetKeyDown(KeyCode.Q) && player.skill.launcherArm.CanUseBool())
        {
            if (!player.skill.isLauncherArmUsable)
                return;
            player.skill.launcherArm.GetInProcessCheck();
            player.transform.position -= PlayerToMousePosVec().normalized * 0.4f;
            stateMachine.ChangeState(player.launcherArmSkill);
        }

        // 중력 자탄 스킬 사용
        if (Input.GetKeyDown(KeyCode.E) && player.skill.gravitonSurge.CanUseBool())
        {
            if (!player.skill.isGravitonUsable)
                return;
            player.skill.gravitonSurge.GetInProcessCheck();
            stateMachine.ChangeState(player.gravitonSurgeSkill);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    
}
