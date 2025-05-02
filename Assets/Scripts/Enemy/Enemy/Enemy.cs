using UnityEngine;
using UnityEngine.EventSystems;

public class Enemy : MonoBehaviour
{
    [Header("Move Info")]
    public float moveSpeed;
    public float runSpeed;

    public bool isMoveX;
    public bool isMoveY;

    public int moveDirection = 1;

    public float idleTimer;
    public float moveTimer;
    public float stateTimer;


    [Header("Attack Info")]
    public GameObject bulletPrefab;
    public bool isBattle;
    public float battleTime;

    [Header("player recognition")]
    public LayerMask playerLayer;
    public float BattleCheckRadius;

    public float gizmoRadius = 3f;    // 부채꼴의 반지름
    public float gizmoAngle = 90f;    // 부채꼴의 각도 (도 단위)
    public Color gizmoColor = Color.red;

    private float startAngle;
    private float endAngle;

    public Rigidbody2D rb { get; private set; }
    public Animator anim { get; private set; }

    public EnemyStateMachine stateMachine { get; private set; }
    public EnemyIdleState idleState { get; private set; }
    public EnemyMoveState moveState { get; private set; }
    public EnemyAttackState attackState { get; private set; }


    protected void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

        stateMachine = new EnemyStateMachine();
        idleState = new EnemyIdleState(this, stateMachine, "Idle");
        moveState = new EnemyMoveState(this, stateMachine, "Move");
        attackState = new EnemyAttackState(this, stateMachine, "Attack");

        isBattle = false;
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

    private bool playerCheck() => Physics2D.Raycast(transform.position, Vector2.zero,3, playerLayer);


    public void TakeDamage(float damage)
    {
        Debug.Log($"{damage}데미지.");
    }

    // 씬 뷰에서 부채꼴 기즈모 그리기
    private void OnDrawGizmos()
    {
        if (isBattle)
        {
            Gizmos.DrawWireSphere(transform.position, BattleCheckRadius);
            return;
        }

        // 원래 색상 저장하고 기즈모 색상 설정
        Color originalColor = Gizmos.color;
        Gizmos.color = gizmoColor;

        float angleStep = 5f; // 작을수록 더 부드러운 호
        Vector3 prev = transform.position;

        if (isMoveX)
            switch (moveDirection)
            {
                case 1:
                    // 시작과 끝 각도 계산
                    startAngle = -gizmoAngle / 2;
                    endAngle = gizmoAngle / 2;
                    break;
                case -1:
                    startAngle = -gizmoAngle / 2 + 180f;
                    endAngle = gizmoAngle / 2 + 180f;
                    break;
            }

        if (isMoveY)
            switch (moveDirection)
            {
                case 1:
                    startAngle = -gizmoAngle / 2 + 90f;
                    endAngle = gizmoAngle / 2 + 90f;
                    break;
                case -1:
                    startAngle = -gizmoAngle / 2 + 270f;
                    endAngle = gizmoAngle / 2 + 270f;
                    break;
            }

        // 시작선 그리기
        Vector3 startDir = Quaternion.Euler(0, 0, startAngle) * transform.right;
        Gizmos.DrawLine(transform.position, transform.position + startDir * gizmoRadius);

        // 호 그리기
        for (float angle = startAngle; angle <= endAngle; angle += angleStep)
        {
            Vector3 direction = Quaternion.Euler(0, 0, angle) * transform.right;
            Vector3 current = transform.position + direction * gizmoRadius;

            // 호를 형성하는 선분 그리기
            Gizmos.DrawLine(prev, current);
            prev = current;
        }

        // 끝선 그리기
        Vector3 endDir = Quaternion.Euler(0, 0, endAngle) * transform.right;
        Gizmos.DrawLine(transform.position, transform.position + endDir * gizmoRadius);

        // 원래 색상 복원
        Gizmos.color = originalColor;
    }

    public bool CheckForPlayerInSight()
    {
        // 먼저 원형 영역 내에 플레이어가 있는지 확인
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, gizmoRadius, playerLayer);

        if (playerCollider == null)
            return false;

        // 플레이어 방향 벡터 계산
        Vector2 directionToPlayer = (playerCollider.transform.position - transform.position).normalized;

        // 기준 방향 설정 (현재 적의 방향에 따라)
        Vector2 referenceDirection = transform.right;
        float currentAngle = 0f;

        // 이동 방향에 따라 기준 방향 조정
        if (isMoveX)
        {
            if (moveDirection == -1)
                currentAngle = 180f;
        }
        else if (isMoveY)
        {
            if (moveDirection == 1)
                currentAngle = 90f;
            else if (moveDirection == -1)
                currentAngle = 270f;
        }

        // 조정된 기준 방향
        referenceDirection = Quaternion.Euler(0, 0, currentAngle) * Vector2.right;

        // 플레이어와 기준 방향 사이의 각도 계산
        float angleToPlayer = Vector2.Angle(referenceDirection, directionToPlayer);

        // 계산된 각도가 시야각의 절반보다 작은지 확인
        if (angleToPlayer <= gizmoAngle / 2)
        {
            // 플레이어까지의 시야를 차단하는 장애물 확인
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position,
                directionToPlayer,
                gizmoRadius,
                playerLayer  // 장애물 레이어가 필요하면 추가: obstacleLayer | playerLayer
            );

            // 레이캐스트 결과 디버깅
            if (hit.collider != null)
            {
                // 충돌한 오브젝트가 플레이어 레이어에 있는지 확인
                bool isPlayerLayer = (playerLayer & (1 << hit.collider.gameObject.layer)) != 0;

                if (isPlayerLayer)
                {
                    Debug.DrawLine(transform.position, hit.point, Color.green);
                    return true;
                }
                else
                {
                    // 플레이어가 아닌 다른 무언가에 닿음
                    Debug.DrawLine(transform.position, hit.point, Color.yellow);
                }
            }
        }
        return false;
    }

    public void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger();


}
