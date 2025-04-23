using UnityEngine;

public class CircleMovement : MonoBehaviour
{
    // ������Ʈ ����
    private Rigidbody2D rb;
    private GrappleHook gh;

    [Header("�̵� ����")]
    [SerializeField] private float speed = 5f;   // �÷��̾� �̵� �ӵ�

    private float mx;  // ���� �Է�
    private float my;  // ���� �Է�

    private void Start()
    {
        // Rigidbody2D�� GrappleHook ������Ʈ ��������
        rb = GetComponent<Rigidbody2D>();
        gh = GetComponent<GrappleHook>();

        // ������Ʈ ���� �� ���
        if (gh == null)
            Debug.LogWarning("GrappleHook ������Ʈ�� �� ������Ʈ�� �����ϴ�!");
    }

    private void Update()
    {
        // �Է°� �ޱ� (�ﰢ ������)
        mx = Input.GetAxisRaw("Horizontal");
        my = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
        // �׷��ø� ���°� �ƴ� ���� �̵� ����
        if (!gh.retracting)
        {
            rb.linearVelocity = new Vector2(mx, my).normalized * speed;
        }
        else
        {
            // �׷��ø� ���� �� �̵� ����
            rb.linearVelocity = Vector2.zero;
        }
    }
}
