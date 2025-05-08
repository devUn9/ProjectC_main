using System.Collections;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Player : MonoBehaviour
{
    [Header("Move Info")]
    public float moveSpeed = 5f;

    public Vector3 finalAttackInputVec;
    private Vector3 inputVector;
    private Vector2 knockbackDir;

    [Header("AttackState Info")]
    public GameObject bulletPrefab;
    public float attackStateTimer = 0.5f;   //공격 애니매이션 발동후 약간의 방향 유지를 위한 시간 차 don't touch plz
    public Vector2 daggerAttackDir;
    public bool isMovingAttack = false;
    public float attackStatusRemainTime;

    public string beforeState;

    [Header("Interaction Info")]
    [SerializeField] protected Transform interactionCheck;   // 상호작용 체크 위치의 기준점
    [SerializeField] private Vector3 interactionDistance;    // 상호작용 거리 설정값
    public float interactionRadius;        // 상호작용 감지 반경
    [SerializeField] private Vector2 raycastDirection;       // 레이캐스트 방향
    [SerializeField] private LayerMask detectionEnemyLayers; // Enemy 레이어 설정
    public Vector2 lastDirection;                           // 마지막으로 이동한 방향 저장
    public Quaternion lastRotation;                         // 마지막으로 이동한 방향의 쿼터니언 저장(근접 공격관련)
    private Vector3 gizmoDirection;                         // 기즈모를 그릴 위치 계산용 변수
    

    [Header("Melee Attack Info")]
    public bool isDaggerAttack = false;
    public Quaternion rotation;
    [SerializeField] private GameObject daggerAttackEffectPrefab;

    public bool invisibility = false;


    public SkillManager skill { get; private set; }

    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public EntityFX fx { get; private set; }
    public SpriteTrail MeshTrailscript { get; private set; }
    public PlayerStats stats { get; private set; }

    public PlayerStateMachine stateMachine { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerPistolMoveState pistolMove { get; private set; }
    public PlayerAttackState attackState { get; private set; }
    public PlayerDaggerAttackState daggerAttack { get; private set; }
    public PlayerGrenadeState grenadeSkill { get; private set; }
    public PlayerLauncherArmState launcherArmSkill { get; private set; }
    public PlayerGravitonState gravitonSurgeSkill { get; private set; }

    protected void Awake()
    {
        stateMachine = new PlayerStateMachine();
        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        pistolMove = new PlayerPistolMoveState(this, stateMachine, "MovingAttack");
        attackState = new PlayerAttackState(this, stateMachine, "Attack");
        daggerAttack = new PlayerDaggerAttackState(this, stateMachine, "daggerAttack");
        grenadeSkill = new PlayerGrenadeState(this, stateMachine, "GrenadeThrow");
        launcherArmSkill = new PlayerLauncherArmState(this, stateMachine, "LauncherArm");
        gravitonSurgeSkill = new PlayerGravitonState(this, stateMachine, "GravitonSurge");
    }

    protected void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        fx = GetComponent<EntityFX>();
        stats = GetComponent<PlayerStats>();
        MeshTrailscript = anim.GetComponent<SpriteTrail>();

        skill = SkillManager.instance;
        skill.Initialize(stats);
        stateMachine.Initialize(idleState);
    }

    protected void Update()
    {
        inputVector.x = Input.GetAxisRaw("Horizontal");
        inputVector.y = Input.GetAxisRaw("Vertical");

        if (inputVector.x != 0 || inputVector.y != 0)
        {
            lastDirection = inputVector.normalized;
        }

        stateMachine.currentState.Update();
        gizmoDirection = SetRaycastDirectionFromInput(inputVector);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Interaction();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            MeshTrailscript.StartTrail();
        }
    }

    public virtual void DamageEffect()
    {
        fx.StartCoroutine("FlashFX");
        //StartCoroutine("HitKnockBack");
    }

    public void ChargeEffect()
    {
        fx.StartCoroutine("ChargeFX");
    }

    public void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger();


    public void SetVelocity(float _velocityX, float _velocityY)
    {
        rb.linearVelocity = new Vector2(_velocityX, _velocityY);
    }

    protected void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(interactionCheck.position + gizmoDirection, interactionRadius);
    }

    // 입력 값 기반으로 레이캐스트 방향 설정
    public Vector3 SetRaycastDirectionFromInput(Vector2 inputVector)
    {
        if (inputVector.magnitude > 0.1f)
        {
            // 입력 벡터의 각도를 계산
            float angle = Mathf.Atan2(inputVector.y, inputVector.x) * Mathf.Rad2Deg;

            // 각도를 0-360 범위로 조정
            if (angle < 0) angle += 360f;

            // 네 방향 중 하나로 반올림
            if (angle >= 315f || angle < 45f)
            {
                // 오른쪽 (1, 0)
                rotation = Quaternion.Euler(0f, 0f, 270f);
                raycastDirection = new Vector2(1f, 0f);
            }
            else if (angle >= 45f && angle < 135f)
            {
                // 위쪽 (0, 1)
                rotation = Quaternion.Euler(0f, 0f, 0f);
                raycastDirection = new Vector2(0f, 1f);
            }
            else if (angle >= 135f && angle < 225f)
            {
                // 왼쪽 (-1, 0)
                rotation = Quaternion.Euler(0f, 0f, 90f);
                raycastDirection = new Vector2(-1f, 0f);
            }
            else // angle >= 225f && angle < 315f
            {
                // 아래쪽 (0, -1)
                rotation = Quaternion.Euler(0f, 0f, 180f);
                raycastDirection = new Vector2(0f, -1f);
            }

            // 마지막 방향 저장
            lastRotation = rotation;
            lastDirection = raycastDirection;
        }
        else if (inputVector.magnitude == 0)
        {
            raycastDirection = lastDirection;
        }

        return raycastDirection * 0.7f;
    }

    

    //상호작용
    public void Interaction()
    {
        Vector3 checkPosition = interactionCheck.position + gizmoDirection;
        Vector2 checkPosition2D = new Vector2(checkPosition.x, checkPosition.y);

        Collider2D colliders = Physics2D.OverlapCircle(checkPosition2D, interactionRadius);
        if (colliders != null)
        {
            Debug.Log("layerCheck:"+ colliders.gameObject.layer);
            if (colliders.GetComponent<Npc>())
                Debug.Log("NPC상호작용");  // NPC와 상호작용
            else if (colliders.GetComponent<Obj>())
                Debug.Log("OBJ상호작용");  // 일반 오브젝트와 상호작용
            else
                Debug.Log("인식할 수 없는 오브젝트입니다.");
        }
        else
        {
            Debug.Log("근처에 오브젝트가 없습니다.");  // 감지된 오브젝트가 없음
        }
    }

    public void MeleeAttack()
    {
        Vector3 checkPosition = interactionCheck.position + gizmoDirection;
        Vector2 checkPosition2D = new Vector2(checkPosition.x, checkPosition.y);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(checkPosition2D, interactionRadius, detectionEnemyLayers);

        foreach(Collider2D collider in colliders)
        {
            if (collider.GetComponent<Enemy>())
            {
                isDaggerAttack = true;
                EffectManager.Instance.PlayEffect(EffectType.SlashEffect, transform.position, 1f, rotation);
                this.stats.DoMeleeDamage(collider.GetComponent<EnemyStats>()); // 적에게 대미지 적용
            }
        }
    }

    private void SetLayerRecursively(string _layer)
    {
        gameObject.layer = LayerMask.NameToLayer(_layer);
        foreach (Transform child in gameObject.transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer(_layer); // child의 레이어 설정
            child.GetComponent<Player>()?.SetLayerRecursively(_layer); // 재귀 호출
        }
    }

    public void Invisibility()
    {
        SetLayerRecursively("InvisablePlayer");
    }
    public void Visiblilty()
    {
        SetLayerRecursively("Player");
    }

    public void SetupKnockbackDir(Transform _damageDirection)
    {
        if (_damageDirection.position.x > transform.position.x && _damageDirection.position.y > transform.position.y)
            knockbackDir = new Vector2(-1, -1);
        else if (_damageDirection.position.x > transform.position.x && _damageDirection.position.y < transform.position.y)
            knockbackDir = new Vector2(-1, 1);
        else if (_damageDirection.position.x < transform.position.x && _damageDirection.position.y > transform.position.y)
            knockbackDir = new Vector2(1, -1);
        else if (_damageDirection.position.x < transform.position.x && _damageDirection.position.y < transform.position.y)
            knockbackDir = new Vector2(1, 1);
    }

    public IEnumerator HitKnockback()
    {
        rb.linearVelocity = knockbackDir * 100f;
        yield return new WaitForSeconds(1f);
    }

}
