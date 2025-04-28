using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class EnemyState
{
    protected Enemy enemy;
    protected EnemyStateMachine stateMachine;
    protected Rigidbody2D rb;

    protected string animBoolName;
    protected float stateTimer;
    protected bool triggerCalled;

    public EnemyState(Enemy _enemy, EnemyStateMachine _stateMachine, string _animBoolName)
    {
        this.enemy = _enemy;
        this.stateMachine = _stateMachine;
        this.animBoolName = _animBoolName;
    }

    public virtual void Enter()
    {
        enemy.anim.SetBool(animBoolName, true);
    }

    public virtual void Update()
    {
        if (enemy.isBattle)
        {
            Vector3 dir = EnemyToPlayerDirection();
            Vector3 velocity = dir * enemy.runSpeed;
            enemy.SetVelocity(velocity.x, velocity.y);
            enemy.anim.SetFloat("VelocityX", dir.x);
            enemy.anim.SetFloat("VelocityY", dir.y);

            if (EnemyToPlayerDistance() > 3.5f)
                stateMachine.ChangeState(enemy.moveState);
            else
                stateMachine.ChangeState(enemy.attackState);
        }

        if (enemy.CheckForPlayerInSight())
        {
            enemy.isBattle = true;
            Debug.Log("전투 진입");
        }
    }

    public virtual void Exit()
    {
        enemy.anim.SetBool(animBoolName, false);
    }

    public virtual void AnimationFinishTrigger()
    {
        triggerCalled = true;
    }

    public float EnemyToPlayerDistance()
    {
        Player player = GameObject.FindAnyObjectByType<Player>();

        float distance = Vector3.Distance(enemy.transform.position, player.transform.position);
        return distance;
    }

    public Vector3 EnemyToPlayerDirection()
    {
        Player player = GameObject.FindAnyObjectByType<Player>();
        Vector3 direction = player.transform.position - enemy.transform.position;
        Vector3 normalizeDir = direction.normalized;
        return normalizeDir;
    }
}
