using System;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.EventSystems;

public enum EnemyType
{
    Human,
    Robot
}
public class Enemy : MonoBehaviour
{
    [Header("Enemy Type")]
    public EnemyType enemyType;

    [Header("Move Info")]
    public float moveSpeed;
    public float runSpeed;

    public bool isMoveX;
    public bool isMoveY;

    public int moveDirection = 1;

    public float idleTimer;
    public float moveTimer;
    public float stateTimer;
    public float loopSaveTimer;     //이동 및 대기 타이머를 저장하는 변수


    [Header("Attack Info")]
    public GameObject bulletPrefab;
    public GameObject meleeAttackPrefab;

    public Vector3 enemyDir;
    public Quaternion meleeAttackAngle;
    public float attackDelay;      // 근접공격 딜레이

    public bool isBattle;
    public float battleTime;
    public float meleeAttackRadius;
    public bool isMelee;
    public bool isBullet;

    [Header("player recognition")]
    public LayerMask playerLayer;
    public LayerMask invisablePlayerLayer; // 투명화된 플레이어 레이어
    public LayerMask enemyLayer;
    public LayerMask wallLayer;
    public float BattleCheckRadius;

    public float gizmoRadius = 3f;    // 부채꼴의 반지름
    public float gizmoAngle = 90f;    // 부채꼴의 각도 (도 단위)
    public float sightLightAngle;
    public Color gizmoColor = Color.red;

    private float startAngle;
    private float endAngle;

    [Header("Sight Effect")]
    [SerializeField] private bool isSightEffectActive = true; // 시야 이펙트 활성화 여부
    private EffectController sightEffect;

    public Rigidbody2D rb { get; private set; }
    public Animator anim { get; private set; }
    public EntityFX fx { get; private set; }

    public DissolveShaderControl dissolveShader; //디졸브 쉐이더 컨트롤러

    public EnemyStats stats { get; private set; }

    public EnemyStateMachine stateMachine { get; private set; }
    public EnemyIdleState idleState { get; private set; }
    public EnemyMoveState moveState { get; private set; }
    public EnemyAttackState attackState { get; private set; }


    protected void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        fx = GetComponentInChildren<EntityFX>();
        stats = GetComponent<EnemyStats>();
        dissolveShader = GetComponentInChildren<DissolveShaderControl>();

        stateMachine = new EnemyStateMachine();
        idleState = new EnemyIdleState(this, stateMachine, "Idle");
        moveState = new EnemyMoveState(this, stateMachine, "Move");
        attackState = new EnemyAttackState(this, stateMachine, "Attack");

        isBattle = false;
    }

    public virtual void Start()
    {
        if (isSightEffectActive)
            sightEffect = EffectManager.Instance.PlayEffectFollow(EffectType.EnemySightEffect, transform, Quaternion.Euler(0, 0, -180f));
        stats.StatusSpeed = 1f;
        stateMachine.Initialize(idleState);
    }

    public virtual void Update()
    {
        anim.speed = TimeManager.Instance.timeScale;
        Debug.Log(anim.speed);
        stateMachine.currentState.Update();
        SetSightEffectAngle();

        StartCoroutine("battleCheck");

        if (isBattle)
            sightEffect.SetSightColor(Color.red);
        else
            sightEffect.SetSightColor(Color.yellow);
    }

    public void healthCheck()
    {
        if (stats.maxHealth.Equals(stats.currentHealth))
            return;
        isBattle = true;
    }

    private IEnumerator battleCheck()
    {
        Collider2D[] Colliders = Physics2D.OverlapCircleAll(transform.position, BattleCheckRadius, enemyLayer);
        foreach (Collider2D collider in Colliders)
        {
            if (collider != null)
            {
                if (collider.GetComponent<Enemy>().isBattle == true)
                {
                    yield return new WaitForSeconds(0.3f);
                    isBattle = true;
                }
            }
        }
    }

    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        rb.linearVelocity = new Vector2(_xVelocity, _yVelocity);
    }

    public void SetZeroVelocity()
    {
        rb.linearVelocity = Vector2.zero;
    }

    public void SetSightEffectAngle()
    {
        if (isBattle)
        {
            Vector3 EnemyToPlayerDirection = stateMachine.currentState.EnemyToPlayerDirection();

            sightLightAngle = Mathf.Atan2(EnemyToPlayerDirection.y, EnemyToPlayerDirection.x) * Mathf.Rad2Deg;

            sightEffect.SetSightEffect(gizmoRadius, Quaternion.Euler(0, 0, sightLightAngle - 90f), gizmoAngle);
            return;
        }

        if (isMoveX)
            switch (moveDirection)
            {
                case 1:
                    // 시작과 끝 각도 계산
                    sightLightAngle = -90f;
                    break;
                case -1:
                    sightLightAngle = 90f;
                    break;
            }

        if (isMoveY)
            switch (moveDirection)
            {
                case 1:
                    sightLightAngle = 0;
                    break;
                case -1:
                    sightLightAngle = 180f;
                    break;
            }
        //시야 이펙트 설정
        sightEffect.SetSightEffect(gizmoRadius, Quaternion.Euler(0, 0, sightLightAngle), gizmoAngle);
    }

    private bool playerCheck() => Physics2D.Raycast(transform.position, Vector2.zero, 3, playerLayer);

    // 씬 뷰에서 부채꼴 기즈모 그리기
    private void OnDrawGizmos()
    {
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


        Gizmos.DrawWireSphere(enemyDir * 2f + transform.position, meleeAttackRadius);
    }

    public bool CheckForPlayerInSight()
    {
        // 먼저 원형 영역 내에 플레이어가 있는지 확인
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, gizmoRadius, playerLayer | invisablePlayerLayer);

        if (playerCollider == null)
            return false;

        if (playerCollider.gameObject.layer == LayerMask.NameToLayer("InvisablePlayer"))
        {
            isBattle = false;
            return false;
        }



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
                playerLayer | wallLayer  // 장애물 레이어가 필요하면 추가: obstacleLayer | playerLayer
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

    public virtual void DamageEffect()
    {
        fx.StartCoroutine("FlashFX");
        //StartCoroutine("HitKnockBack");
    }

    public virtual void StunEffect()
    {
        fx.StartCoroutine("StunFX");
    }

    public virtual void EmpEffect(float _duration)
    {
        // EMP 이펙트
        StartCoroutine(fx.EmpShockFX(_duration));
    }

    public void DieShader()
    {
        StartCoroutine(dissolveShader.DissolveSequence());
    }

    public static event System.Action OnEnemyRemoved;
    private void OnDestroy()
    {
        // 몬스터가 파괴되거나 비활성화될 때 이벤트 발생
        OnEnemyRemoved?.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (enemyType == EnemyType.Robot)
        {
            if (!collision.GetComponent<Player>())
                return;
            Player player = collision.GetComponent<Player>();
            PlayerStats playerStats = collision.GetComponent<PlayerStats>();
            if (isMelee)
            {
                EffectManager.Instance.PlayEffect(EffectType.GrenadeEffect, transform.position, 2f);
                stats.DoMeleeDamage(playerStats);
                Destroy(gameObject);
            }
        }
    }
}
