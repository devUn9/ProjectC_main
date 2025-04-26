using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class GrappleHook3 : MonoBehaviour
{
    // ���� �ð�ȭ�� ���� LineRenderer ������Ʈ
    private LineRenderer line;

    [Header("Grappling ����")]
    [SerializeField] LayerMask grapplableMask;   // �׷��ø� ������ ������Ʈ ���̾�
    [SerializeField] float maxDistance = 10f;     // �׷��ø� �ִ� ��Ÿ�
    [SerializeField] float grappleSpeed = 10f;    // ������Ʈ�� �������� �ӵ�
    [SerializeField] float grappleShootSpeed = 20f; // �� �߻� �ִϸ��̼� �ӵ�

    private bool isGrappling = false;             // ���� �׷��ø� ������ ����

    private Vector2 target;        // ���� ���� ��ġ (����� ������ ����)
    private Transform targetObject; // ������ ����� ��� ������Ʈ
    private float retractTimer = 0f; // ������Ʈ�� ����� ��, ������ �ʹ� �ʰ� �ϰų� �� �� ���� �ֱ⿡ Ÿ�̸� ����
    public int itemCount;
    private Vector2 pullStopPosition;

    [HideInInspector] public bool isRetractingPlayer = false; // �÷��̾ ���������� ����
    [HideInInspector] public bool isRetractingObject = false; // ������Ʈ�� ���������� ����

    private void Start()
    {
        line = GetComponent<LineRenderer>();  // LineRenderer ������Ʈ ����
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && !isGrappling)
        {
            StartGrapple();
        }

        if (isRetractingPlayer)
            HandlePlayerRetract();

        if (isRetractingObject)
            HandleObjectRetract();
    }

    // �׷��ø� �� �߻� �޼���
    private void StartGrapple()
    {
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxDistance, grapplableMask);

        if (hit.collider != null)
        {
            Debug.Log("Raycast �浹: " + hit.collider.name);

            isGrappling = true;
            line.enabled = true;
            line.positionCount = 2;

            int targetLayer = hit.collider.gameObject.layer;
            Debug.Log("�浹 ���̾�: " + LayerMask.LayerToName(targetLayer));

            if (targetLayer == LayerMask.NameToLayer("Grappleable"))
            {
                Debug.Log("������Ʈ ������� ����");
                targetObject = hit.collider.transform;

                // �浹 ���� ó��
                Collider2D playerCol = GetComponent<Collider2D>();
                Collider2D targetCol = targetObject.GetComponent<Collider2D>();

                if (playerCol != null && targetCol != null)
                {
                    Physics2D.IgnoreCollision(playerCol, targetCol, true);
                }

                Vector2 directionToPlayer = ((Vector2)transform.position - (Vector2)targetObject.position).normalized;
                pullStopPosition = (Vector2)transform.position + directionToPlayer * -1f;

                StartCoroutine(Grapple(targetObject.position, false));
            }
            else if (targetLayer == LayerMask.NameToLayer("Grappeable2"))
            {
                Debug.Log("�÷��̾� �̵� ����");
                target = hit.point; // ��� �������ε� �̵� �����ϵ��� ����
                StartCoroutine(Grapple(target, true));
            }
        }
        else
        {
            Debug.Log("Raycast �̽�");
        }
    }

    // �÷��̾ �������� ���� �߻�
    private void HandlePlayerRetract()
    {
        // �÷��̾� �̵�
        transform.position = Vector2.MoveTowards(transform.position, target, grappleSpeed * Time.deltaTime);

        // ���� ����
        line.SetPosition(0, transform.position);
        line.SetPosition(1, target);

        // ���� �� ����
        if (Vector2.Distance(transform.position, target) < 0.5f)
        {
            ResetGrapple();
        }
    }

    // �÷��̾ ���� ���� ���� �߻�
    private void HandleObjectRetract()
    {
        if (targetObject == null)
        {
            ResetGrapple();
            return;
        }

        retractTimer += Time.deltaTime;


        targetObject.position = Vector2.MoveTowards(targetObject.position, pullStopPosition, grappleSpeed * Time.deltaTime);

        // ���� ����
        line.SetPosition(0, transform.position);
        line.SetPosition(1, targetObject.position);

        // ���� �� ����
        if (Vector2.Distance(targetObject.position, pullStopPosition) < 0.1f || retractTimer > 3f)
        {
            if (targetObject.CompareTag("Collectible"))
            {
                itemCount++;
                Debug.Log($"������ ȹ��! �� ����: {itemCount}");
                Destroy(targetObject.gameObject);
                ResetGrapple();
            }
            else if (targetObject.CompareTag("Grapplable"))
            {
                Debug.Log("����!");
                StartCoroutine(StunObject(targetObject));
                ResetGrapple();
            }
            else
            {
                Debug.Log("���ع� ���� - �÷��̾� �տ� ��ġ �Ϸ�");
                ResetGrapple();
            }
        }
    }

    private void ResetGrapple()
    {
        if (targetObject != null)
        {
            Collider2D playerCol = GetComponent<Collider2D>();
            Collider2D targetCol = targetObject.GetComponent<Collider2D>();

            if (playerCol != null && targetCol != null)
            {
                Physics2D.IgnoreCollision(playerCol, targetCol, false);
            }
        }

        isRetractingObject = false;
        isRetractingPlayer = false;
        isGrappling = false;
        line.enabled = false;
        targetObject = null;
        retractTimer = 0f;
    }

    // �� �߻� �ִϸ��̼� �ڷ�ƾ
    IEnumerator Grapple(Vector2 targetPosition, bool isPlayerMoving)
    {
        float t = 0f;
        float time = 0.2f;  // �߻� �ִϸ��̼� ������ (�ϵ��ڵ�)

        // ������ ���۰� ������ �ʱ�ȭ
        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.position);

        while (t < time)
        {
            t += Time.deltaTime;

            Vector2 startPoint = transform.position;
            Vector2 endPoint = targetPosition;

            Vector2 newPos = Vector2.Lerp(startPoint, endPoint, t / time);

            line.SetPosition(0, transform.position);
            line.SetPosition(1, newPos);

            yield return null;
        }

        // ���� Ÿ�ٿ� ����
        line.SetPosition(1, targetPosition);

        // �߻� �� ���� ��ȯ
        if (isPlayerMoving)
            isRetractingPlayer = true;
        else
            isRetractingObject = true;
    }

    // �� ������Ʈ�� ���� ���� ���� ���� �ڷ�ƾ
    IEnumerator StunObject(Transform obj)
    {
        SpriteRenderer sp = obj.GetComponent<SpriteRenderer>();

        if (sp != null)
        {
            Color originalColor = sp.color;

            // ���� ����
            sp.color = Color.yellow;

            yield return new WaitForSeconds(2f);

            // ���� ���� ����
            sp.color = originalColor;
        }
        else
        {
            Debug.Log("��������Ʈ�� ����");
        }

        ResetGrapple();
    }
}
