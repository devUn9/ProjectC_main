using UnityEngine;

public class EnemyIdleState : EnemyState
{
    public EnemyIdleState(Enemy _enemy, EnemyStateMachine _stateMachine, string _animBoolName)
        : base(_enemy, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        enemy.loopSaveTimer = enemy.idleTimer;

    }
    public override void Update()
    {
        base.Update();
        enemy.idleTimer -= Time.deltaTime* TimeManager.Instance.timeScale;

        enemy.SetZeroVelocity();

        if (enemy.isBattle)
        {
            if (EnemyToPlayerDistance() > attackRange)
                stateMachine.ChangeState(enemy.moveState);
            else
                stateMachine.ChangeState(enemy.attackState);
        }

        if (enemy.isMoveX)
        {
            enemy.anim.SetFloat("VelocityX", enemy.moveDirection);
        }
        else if (enemy.isMoveY)
        {
            enemy.anim.SetFloat("VelocityY", enemy.moveDirection);
        }

        if (enemy.idleTimer < 0)
            stateMachine.ChangeState(enemy.moveState);
    }

    public override void Exit()
    {
        enemy.idleTimer = enemy.loopSaveTimer;
        enemy.moveDirection = -1 * enemy.moveDirection;
        base.Exit();
    }

}
