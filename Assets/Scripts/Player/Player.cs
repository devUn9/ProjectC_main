using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Move Info")]
    public float moveSpeed = 5f;

    public Vector3 finalAttackInputVec;
    private Vector3 inputVector;
    
    

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
    [SerializeField] private float interactionRadius;        // 상호작용 감지 반경
    [SerializeField] private Vector2 raycastDirection;       // 레이캐스트 방향
    [SerializeField] private LayerMask detectionEnemyLayers; // Enemy 레이어 설정
    public Vector2 lastDirection;                           // 마지막으로 이동한 방향 저장
    private Vector3 gizmoDistance;                           // 기즈모를 그릴 위치 계산용 변수

    [Header("Melee Attack Info")]
    public bool isDaggerAttack = false;
    [SerializeField] private GameObject daggerAttackEffectPrefab;
    private Vector3 AttackDistance;




    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }

    public SpriteTrail MeshTrailscript { get; private set; }


    public PlayerStateMachine stateMachine { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerPistolMoveState pistolMove { get; private set; }
    public PlayerAttackState attackState { get; private set; }
    public PlayerDaggerAttackState daggerAttack { get; private set; }

    protected void Awake()
    {
        stateMachine = new PlayerStateMachine();
        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        pistolMove = new PlayerPistolMoveState(this, stateMachine, "MovingAttack");
        attackState = new PlayerAttackState(this, stateMachine, "Attack");
        daggerAttack = new PlayerDaggerAttackState(this, stateMachine, "daggerAttack");
    }

    protected void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();

        MeshTrailscript = anim.GetComponent<SpriteTrail>();

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
        gizmoDistance = SetRaycastDirectionFromInput(inputVector);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Interaction();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            MeshTrailscript.StartTrail();
        }
    }

    public void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger();


    public void SetVelocity(float _velocityX, float _velocityY)
    {
        rb.linearVelocity = new Vector2(_velocityX, _velocityY);
    }

    protected void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(interactionCheck.position + gizmoDistance, interactionRadius);
    }

    // 입력 값 기반으로 레이캐스트 방향 설정
    public Vector3 SetRaycastDirectionFromInput(Vector2 inputVector)
    {
        if (inputVector.magnitude > 0.1f)
        {
            raycastDirection = inputVector.normalized;
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
        Vector3 checkPosition = interactionCheck.position + gizmoDistance;
        Vector2 checkPosition2D = new Vector2(checkPosition.x, checkPosition.y);

        Collider2D colliders = Physics2D.OverlapCircle(checkPosition2D, interactionRadius);
        if (colliders != null)
        {
            if (colliders.GetComponent<Npc>() != null)
                Debug.Log("NPC상호작용");  // NPC와 상호작용
            else if (colliders.GetComponent<Obj>() != null)
                Debug.Log("OBJ상호작용");  // 일반 오브젝트와 상호작용
            else if (colliders.gameObject.layer == LayerMask.NameToLayer("Enemy"))    //적이 감지된 경우
            {
                isDaggerAttack = true;
                Instantiate(daggerAttackEffectPrefab, interactionCheck.position + gizmoDistance, Quaternion.identity);
            }
            else
                Debug.Log("인식할 수 없는 오브젝트입니다.");
        }
        else
        {
            Debug.Log("근처에 오브젝트가 없습니다.");  // 감지된 오브젝트가 없음
        }
    }

}
