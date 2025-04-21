using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Move Info")]
    public float moveSpeed = 5f;
    private Vector3 InputVector;

    [Header("Attack Info")]
    public GameObject bulletPrefab;

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
