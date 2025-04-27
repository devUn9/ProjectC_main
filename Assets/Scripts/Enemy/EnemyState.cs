using UnityEngine;

public class EnemyState
{
    protected Enemy enemy;
    protected EnemyStateMachine stateMachine;
    protected Rigidbody2D rb;

    protected string animBoolName;
    protected float stateTimer;

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
        
    }

    public virtual void Exit()
    {
        enemy.anim.SetBool(animBoolName, false);
    }
}
