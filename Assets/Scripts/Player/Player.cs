using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Move Info")]
    public float moveSpeed = 5f;

    public Vector3 finalAttackInputVec;
    private Vector3 inputVector;
    public float attackStatusRemainTime;
    public bool isMovingAttack = false;

    [Header("Attack Info")]
    public GameObject bulletPrefab;
    public float attackStateTimer = 0.5f;   //���� �ִϸ��̼� �ߵ��� �ణ�� ���� ������ ���� �ð� �� don't touch plz

    [Header("Interaction Info")]
    [SerializeField] protected Transform interactionCheck;   // ��ȣ�ۿ� üũ ��ġ�� ������
    [SerializeField] private Vector3 interactionDistance;    // ��ȣ�ۿ� �Ÿ� ������
    [SerializeField] private float interactionRadius;        // ��ȣ�ۿ� ���� �ݰ�
    [SerializeField] private LayerMask detectionLayers;      // ������ ���̾� ����
    [SerializeField] private Vector2 raycastDirection;       // ����ĳ��Ʈ ����
    private Vector2 lastDirection;                           // ���������� �̵��� ���� ����
    private Vector3 gizmoDistance;                           // ����� �׸� ��ġ ���� ����


    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }

    public SpriteTrail MeshTrailscript { get; private set; }


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
        pistolMove = new PlayerPistolMoveState(this, stateMachine, "MovingAttack");
        attackState = new PlayerAttackState(this, stateMachine, "Attack");
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

    // �Է� �� ������� ����ĳ��Ʈ ���� ����
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

    //��ȣ�ۿ�
    public void Interaction()
    {
        Vector3 checkPosition = interactionCheck.position + gizmoDistance;
        Vector2 checkPosition2D = new Vector2(checkPosition.x, checkPosition.y);

        Collider2D colliders = Physics2D.OverlapCircle(checkPosition2D, interactionRadius);

        if (colliders != null)
        {
            if (colliders.GetComponent<Npc>() != null)
                Debug.Log("NPC��ȣ�ۿ�");  // NPC�� ��ȣ�ۿ�
            else if (colliders.GetComponent<Obj>() != null)
                Debug.Log("OBJ��ȣ�ۿ�");  // �Ϲ� ������Ʈ�� ��ȣ�ۿ�
            else
                Debug.Log("�ν��� �� ���� ������Ʈ�Դϴ�.");
        }
        else
        {
            Debug.Log("��ó�� ������Ʈ�� �����ϴ�.");  // ������ ������Ʈ�� ����
        }
    }
}
