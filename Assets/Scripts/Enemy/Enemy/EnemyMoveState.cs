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
        enemy.stateTimer = enemy.moveTimer;
    }

    public override void Update()
    {
        base.Update();
        enemy.moveTimer -= Time.deltaTime;

        if (enemy.isBattle)
        {
            if (EnemyToPlayerDistance() > enemy.gizmoRadius)
                return;
            stateMachine.ChangeState(enemy.attackState);
        }

        if (enemy.moveTimer < 0)
            stateMachine.ChangeState(enemy.idleState);

        if (enemy.isMoveX)
        {
            enemy.anim.SetFloat("VelocityX", enemy.moveDirection);
            enemy.SetVelocity(enemy.moveSpeed * enemy.moveDirection, 0);
        }
        else if (enemy.isMoveY)
        {
            enemy.anim.SetFloat("VelocityY", enemy.moveDirection);
            enemy.SetVelocity(0, enemy.moveSpeed * enemy.moveDirection);
        }
    }
    public override void Exit()
    {
        enemy.moveTimer = enemy.stateTimer;
        base.Exit();
    }
}
