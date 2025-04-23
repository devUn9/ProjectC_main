using System.Collections;
using UnityEngine;

public class GrappleHook2 : MonoBehaviour
{
    // ���� �ð�ȭ�� ���� LineRenderer ������Ʈ
    private LineRenderer line;

    [Header("Grappling ����")]
    [SerializeField] LayerMask grapplableMask;   // �׷��ø� ������ ������Ʈ ���̾�
    [SerializeField] float maxDistance = 10f;     // �׷��ø� �ִ� ��Ÿ�
    [SerializeField] float grappleSpeed = 10f;    // ������Ʈ�� �������� �ӵ�
    [SerializeField] float grappleShootSpeed = 20f; // �� �߻� �ִϸ��̼� �ӵ�

    private bool isGrappling = false;             // ���� �׷��ø� ������ ����
    [HideInInspector] public bool retracting = false; // ������Ʈ�� �������� ������ ����

    private Vector2 target;        // ���� ���� ��ġ (����� ������ ����)
    private Transform targetObject; // ������ ����� ��� ������Ʈ

    private void Start()
    {
        // LineRenderer ������Ʈ �ʱ�ȭ
        line = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        // ��Ŭ�� �� �׷��ø� ���� (�̹� ��� ���� �ƴ� ����)
        if (Input.GetMouseButtonDown(1) && !isGrappling)
        {
            StartGrapple();
        }

        // ������Ʈ�� �������� ���� �� ����
        if (retracting && targetObject != null)
        {
            // ��� ������Ʈ�� �÷��̾� ������ �̵�
            targetObject.position = Vector2.MoveTowards(
                targetObject.position,
                transform.position,
                grappleSpeed * Time.deltaTime
            );

            // ���� �ð�ȭ ������Ʈ (�÷��̾� �� ������Ʈ)
            line.SetPosition(0, transform.position);
            line.SetPosition(1, targetObject.position);

            // ������Ʈ�� �÷��̾� ��ó�� �����ϸ� �׷��ø� ����
            if (Vector2.Distance(targetObject.position, transform.position) < 0.5f)
            {
                retracting = false;
                isGrappling = false;
                line.enabled = false;
                targetObject = null;
            }
        }
    }

    // �׷��ø� �� �߻� ����
    private void StartGrapple()
    {
        // ���콺 �������� Raycast �߻�
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxDistance, grapplableMask);

        // �׷��ø� ������ ������Ʈ�� ����� ��
        if (hit.collider != null)
        {
            isGrappling = true;
            targetObject = hit.collider.transform;   // ��� ������Ʈ ����

            // ���� �ð�ȭ Ȱ��ȭ
            line.enabled = true;
            line.positionCount = 2;

            // �� �߻� �ִϸ��̼� ����
            StartCoroutine(Grapple());
        }
    }

    // �� �߻� �ִϸ��̼� �ڷ�ƾ
    IEnumerator Grapple()
    {
        float t = 0f;
        float time = 10f;  // �ִϸ��̼� ���� �ð� (������ ��)

        // ���� ���� ��ġ �ʱ�ȭ (�÷��̾� ����)
        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.position);

        Vector2 hookPos;

        // ���� ������Ʈ���� ������� �ִϸ��̼�
        for (; t < time; t += grappleShootSpeed * Time.deltaTime)
        {
            // �� ��ġ�� ���������� ��� ������Ʈ �������� �̵�
            hookPos = Vector2.Lerp(transform.position, targetObject.position, t / time);

            // ���� ����
            line.SetPosition(0, transform.position);
            line.SetPosition(1, hookPos);

            yield return null;  // ������ ���
        }

        // ���� ������ �����ϸ�, ���� ������ ��� ������Ʈ ��ġ�� ����
        line.SetPosition(1, targetObject.position);

        // ���� ������Ʈ�� ������� �ܰ�� ��ȯ
        retracting = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Grapplable"))
        {
            // �浹 �� ��� Rigidbody �ӵ� �ʱ�ȭ (�и� ����)
            Rigidbody2D rb = collision.collider.attachedRigidbody;
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }

}
