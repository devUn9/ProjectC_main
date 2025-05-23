using UnityEngine;

public class EnemyMoveState : EnemyState
{
    public EnemyMoveState(Enemy _enemy, EnemyStateMachine _stateMachine, string _animBoolName)
        : base(_enemy, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        enemy.loopSaveTimer = enemy.moveTimer;
    }

    public override void Update()
    {
        base.Update();

        if (enemy.isBattle)
        {
            if (!enemy.isNavi)
            {
                enemy.SetVelocity(velocity.x, velocity.y);
            }
            else
            {
                enemy.navAgent.speed = enemy.runSpeed * TimeManager.Instance.timeScale * enemy.stats.StatusSpeed;
                Player player = GameObject.FindAnyObjectByType<Player>();
                enemy.navAgent.SetDestination(player.transform.position);
            }
            if (EnemyToPlayerDistance() > attackRange)
                return;
            stateMachine.ChangeState(enemy.attackState);
        }
        enemy.SetVelocity(velocity.x, velocity.y);

        enemy.moveTimer -= Time.deltaTime * TimeManager.Instance.timeScale * enemy.stats.StatusSpeed;

        if (enemy.moveTimer < 0)
            stateMachine.ChangeState(enemy.idleState);

        if (enemy.isMoveX)
        {
            enemy.anim.SetFloat("VelocityX", enemy.moveDirection);
            //순찰 속도 조절
            enemy.SetVelocity(enemy.moveSpeed * enemy.moveDirection * TimeManager.Instance.timeScale * enemy.stats.StatusSpeed, 0);
        }
        else if (enemy.isMoveY)
        {
            enemy.anim.SetFloat("VelocityY", enemy.moveDirection);
            //순찰 속도 조절
            enemy.SetVelocity(0, enemy.moveSpeed * enemy.moveDirection * TimeManager.Instance.timeScale * enemy.stats.StatusSpeed);
        }
    }
    public override void Exit()
    {
        enemy.moveTimer = enemy.loopSaveTimer;
        base.Exit();
    }
}
