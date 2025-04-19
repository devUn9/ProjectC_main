using UnityEngine;

public class Player : MonoBehaviour
{

    [Header("Move Info")]
    public float moveSpeed = 5f;

    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }


    public PlayerStateMachine stateMachine { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }

    protected void Awake()
    {
        stateMachine = new PlayerStateMachine();
        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
    }

    protected void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();

        stateMachine.Initialize(idleState);
    }

    protected void Update()
    {
        stateMachine.currentState.Update();
    }

    public void SetVelocity(float _velocityX, float _velocityY)
    {
        rb.linearVelocity = new Vector2(_velocityX, _velocityY);
    }
}
