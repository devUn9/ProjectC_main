using UnityEngine;
using UnityEngine.SocialPlatforms;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class EnemyState
{
    protected Enemy enemy;
    protected EnemyStateMachine stateMachine;
    protected Rigidbody2D rb;

    protected string animBoolName;
    protected float stateTimer;
    protected bool triggerCalled;

    protected float attackRange;
    protected Vector3 velocity;


    public EnemyState(Enemy _enemy, EnemyStateMachine _stateMachine, string _animBoolName)
    {
        this.enemy = _enemy;
        this.stateMachine = _stateMachine;
        this.animBoolName = _animBoolName;
    }

    public virtual void Enter()
    {
        enemy.anim.SetBool(animBoolName, true);
        if (enemy.isMelee)
        {
            attackRange = enemy.meleeAttackRadius;
        }
        else if (enemy.isBullet)
            attackRange = enemy.gizmoRadius;
        Debug.Log("상태 진입 : " + animBoolName);
        enemy.anim.speed = TimeManager.Instance.timeScale;
    }

    public virtual void Update()
    {
        stateTimer -= Time.deltaTime * TimeManager.Instance.timeScale;

        if (enemy.isBattle)
        {
            enemy.enemyDir = EnemyToPlayerDirection();
            velocity = enemy.enemyDir * enemy.runSpeed * TimeManager.Instance.timeScale;
            enemy.anim.SetFloat("VelocityX", enemy.enemyDir.x);
            enemy.anim.SetFloat("VelocityY", enemy.enemyDir.y);
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
