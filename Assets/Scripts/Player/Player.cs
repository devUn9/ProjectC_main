using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Move Info")]
    public float moveSpeed = 5f;
    private Vector3 InputVector;

    [Header("Attack Info")]
    public GameObject bulletPrefab;

    [Header("Interaction Info")]
    [SerializeField] protected Transform interactionCheck;   // 상호작용 체크 위치의 기준점
    [SerializeField] private Vector3 interactionDistance;    // 상호작용 거리 설정값
    [SerializeField] private float interactionRadius;        // 상호작용 감지 반경
    [SerializeField] private LayerMask detectionLayers;      // 감지할 레이어 설정
    [SerializeField] private Vector2 raycastDirection;       // 레이캐스트 방향
    private Vector2 lastDirection;                           // 마지막으로 이동한 방향 저장
    private Vector3 gizmoDistance;                           // 기즈모를 그릴 위치 계산용 변수

    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }

    public PlayerStateMachine stateMachine { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerPistolMoveState pistolMove { get; private set; }
    public PlayerAttackState attackState { get; private set; }

    protected void Awake()
    {
        stateMachine = new PlayerStateMachine();
        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        pistolMove = new PlayerPistolMoveState(this, stateMachine, "Attack");
        attackState = new PlayerAttackState(this, stateMachine, "Attack");
    }

    protected void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();

        stateMachine.Initialize(idleState);
    }

    protected void Update()
    {
        InputVector.x = Input.GetAxisRaw("Horizontal");
        InputVector.y = Input.GetAxisRaw("Vertical");

        if (InputVector.x != 0 || InputVector.y != 0)
        {
            lastDirection = InputVector.normalized;
        }

        stateMachine.currentState.Update();
        gizmoDistance = SetRaycastDirectionFromInput(InputVector);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Interaction();
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
            else
                Debug.Log("인식할 수 없는 오브젝트입니다.");
        }
        else
        {
            Debug.Log("근처에 오브젝트가 없습니다.");  // 감지된 오브젝트가 없음
        }
    }
}
