using System.Collections;
using UnityEngine;

public class GrappleHook : MonoBehaviour
{
    // ���� �ð�ȭ�� ���� ���� ������
    private LineRenderer line;

    [Header("Grappling ����")]
    [SerializeField] LayerMask grapplableMask;   // �׷��ø� ���� �ɸ� �� �ִ� ������Ʈ ���̾�
    [SerializeField] float maxDistance = 10f;     // �ִ� ��Ÿ�
    [SerializeField] float grappleSpeed = 10f;    // �÷��̾ �������� �ӵ�
    [SerializeField] float grappleShootSpeed = 20f; // ���� �߻�Ǵ� �ӵ�

    private bool isGrappling = false;  // �׷��� ���� ����
    [HideInInspector] public bool retracting = false; // ���� ������ ��, �÷��̾ ������� ����

    private Vector2 target;   // ���� ������ ��ǥ ����

    private void Start()
    {
        line = GetComponent<LineRenderer>();  // LineRenderer ������Ʈ ����
    }

    private void Update()
    {
        // ��Ŭ�� �� �׷��� �߻� (�̹� �׷��ø� ���� �ƴ� ����)
        if (Input.GetMouseButtonDown(1) && !isGrappling)
        {
            StartGrapple();
        }

        // �÷��̾ ��ǥ �������� �������� ����
        if (retracting)
        {
            // ���� ��ġ���� Ÿ�ٱ��� �̵�
            Vector2 grapplePos = Vector2.Lerp(transform.position, target, grappleSpeed * Time.deltaTime);
            transform.position = grapplePos;

            // ������ ������ ����
            line.SetPosition(0, transform.position);

            // ��ǥ ������ �����ϸ� ����
            if (Vector2.Distance(transform.position, target) < 0.5f)
            {
                retracting = false;
                isGrappling = false;
                line.enabled = false;
            }
        }
    }

    // �׷��ø� �� �߻� �޼���
    private void StartGrapple()
    {
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;

        // ����ĳ��Ʈ�� �浹 üũ
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxDistance, grapplableMask);

        if (hit.collider != null)
        {
            isGrappling = true;
            target = hit.point;

            // ���� Ȱ��ȭ �� �ʱ�ȭ
            line.enabled = true;
            line.positionCount = 2;

            // �� �߻� �ڷ�ƾ ����
            StartCoroutine(Grapple());
        }
    }

    // �� �߻� �ִϸ��̼� �ڷ�ƾ
    IEnumerator Grapple()
    {
        float t = 0f;
        float time = 10f;  // �߻� �ִϸ��̼� ������ (�ϵ��ڵ�)

        // ������ ���۰� ������ �ʱ�ȭ
        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.position);

        Vector2 newPos;

        // ���� Ÿ������ ���� ���ư��� �ִϸ��̼�
        for (; t < time; t += grappleShootSpeed * Time.deltaTime)
        {
            newPos = Vector2.Lerp(transform.position, target, t / time);

            line.SetPosition(0, transform.position);
            line.SetPosition(1, newPos);

            yield return null;
        }

        // ���� Ÿ�ٿ� ����
        line.SetPosition(1, target);
        retracting = true;  // ���� �÷��̾ �������� ����
    }
}
