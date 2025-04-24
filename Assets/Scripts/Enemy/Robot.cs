using UnityEngine;
[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SpriteRenderer))]
public class Robot : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float moveTime = 2f;
    [SerializeField] private float stopTime = 1f;
    [Header("정찰 방향")]
    [SerializeField] private PatrolDirection startDirection = PatrolDirection.Right;
    public enum PatrolDirection { Up, Down, Left, Right }

    private Rigidbody2D rb;
    //private SpriteRenderer spriteRenderer;
    private Vector2 moveDirection;
    private bool isMoving = true;
    private float stateTimer;
    private bool movingForward = true; // 정방향으로 이동 중인지 표시

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //spriteRenderer = GetComponent<SpriteRenderer>();

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        SetInitialDirection(startDirection);

        stateTimer = moveTime;
    }

    private void Update()
    {
        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0)
        {
            if (isMoving)
            {
                // 이동 중이었다면 멈추기
                isMoving = false;
                stateTimer = stopTime;
            }
            else
            {
                // 멈춰있었다면 방향을 전환하고 이동 시작
                isMoving = true;
                movingForward = !movingForward; // 방향 전환
                UpdateMovementDirection();
                stateTimer = moveTime;
            }
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = isMoving ? moveDirection * moveSpeed : Vector2.zero;
    }

    private void SetInitialDirection(PatrolDirection direction)
    {
        // 초기 방향 설정
        switch (direction)
        {
            case PatrolDirection.Up:
                moveDirection = Vector2.up;
                break;
            case PatrolDirection.Down:
                moveDirection = Vector2.down;
                break;
            case PatrolDirection.Left:
                moveDirection = Vector2.left;
                break;
            case PatrolDirection.Right:
                moveDirection = Vector2.right;
                break;
        }
    }

    private void UpdateMovementDirection()
    {
        // 시작 방향에 기반한 이동 방향 업데이트
        switch (startDirection)
        {
            case PatrolDirection.Up:
            case PatrolDirection.Down:
                // 상하 이동
                moveDirection = movingForward ?
                    (startDirection == PatrolDirection.Up ? Vector2.up : Vector2.down) :
                    (startDirection == PatrolDirection.Up ? Vector2.down : Vector2.up);
                break;

            case PatrolDirection.Left:
            case PatrolDirection.Right:
                // 좌우 이동
                moveDirection = movingForward ?
                    (startDirection == PatrolDirection.Right ? Vector2.right : Vector2.left) :
                    (startDirection == PatrolDirection.Right ? Vector2.left : Vector2.right);
                break;
        }

        //// 스프라이트 방향 설정 (필요하다면)
        //if (moveDirection.x < 0)
        //    spriteRenderer.flipX = true;
        //else if (moveDirection.x > 0)
        //    spriteRenderer.flipX = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 충돌 시 방향 전환 (필요하다면)
        // movingForward = !movingForward;
        // UpdateMovementDirection();
    }
}