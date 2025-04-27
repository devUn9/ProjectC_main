using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Move Info")]
    public float moveSpeed;
    public float runSpeed;
    public float battleTime;

    public bool isMoveX;
    public bool isMoveY;

    public int moveDirection = 1;

    public float idleTimer;
    public float moveTimer;
    public float stateTimer;

    public Rigidbody2D rb { get; private set; }
    public Animator anim { get; private set; }

    public EnemyStateMachine stateMachine { get; private set; }
    public EnemyIdleState idleState { get; private set; }
    public EnemyMoveState moveState { get; private set; }


    protected void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

        stateMachine = new EnemyStateMachine();
        idleState = new EnemyIdleState(this, stateMachine, "Idle");
        moveState = new EnemyMoveState(this, stateMachine, "Move");
    }

    public virtual void Start()
    {
        stateMachine.Initialize(idleState);
    }

    public virtual void Update()
    {
        stateMachine.currentState.Update();
    }

    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        rb.linearVelocity = new Vector2(_xVelocity, _yVelocity);
    }

    public void SetZeroVelocity()
    {
        rb.linearVelocity = Vector2.zero;
    }
}
