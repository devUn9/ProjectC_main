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
        enemy.stateTimer = enemy.idleTimer;

    }
    public override void Update()
    {
        base.Update();
        enemy.idleTimer -= Time.deltaTime;

        enemy.SetZeroVelocity();

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
        enemy.idleTimer = enemy.stateTimer;
        enemy.moveDirection = -1 * enemy.moveDirection;
        base.Exit();
    }

}
