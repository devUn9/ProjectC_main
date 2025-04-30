using System.Collections;
using UnityEngine;

public class GrappleHook5 : MonoBehaviour
{
    // ���� �ð�ȭ�� ���� LineRenderer ������Ʈ
    private LineRenderer line;

    [Header("Grappling ����")]
    [SerializeField] LayerMask grapplableMask;   // �׷��ø� ������ ������Ʈ�� ���̾� ����ũ
    [SerializeField] float maxDistance = 10f;     // �׷��ø� �ִ� �Ÿ�
    [SerializeField] float grappleSpeed = 10f;    // �������� �ӵ� (�÷��̾ ������Ʈ)
    [SerializeField] float grappleShootSpeed = 20f; // �� �߻� �ӵ� (���� �̻��)

    private bool isGrappling = false;             // �׷��ø� ������ ����

    private Vector2 target;                       // �÷��̾ �̵��� ��ǥ ���� (�� �׷��ø�)
    private Transform targetObject;               // ����� ������Ʈ ����
    private float retractTimer = 0f;              // ������Ʈ ���� ���� �ð��� Ÿ�̸�
    public int itemCount;                         // ������ ������ ����
    private Vector2 pullStopPosition;             // ������Ʈ�� ���� ��ġ (�÷��̾� ��)

    [HideInInspector] public bool isRetractingPlayer = false;  // �÷��̾ �̵� ������ ����
    [HideInInspector] public bool isRetractingObject = false;  // ������Ʈ�� �������� ������ ����

    public bool isUpgrade = false;                // �ӵ� ���׷��̵� ����
    public float SpeedMultiplier => isUpgrade ? 1.5f : 1f;   // �ӵ� ���� ���

    private bool isTargetLocked = false;   // Ÿ���� �����Ǿ����� ����
    private RaycastHit2D lockedHit;        // ������ Ÿ�� ���� ����

    [SerializeField] private GameObject crosshair;  // ������ �̹��� ������Ʈ

    private void Start()
    {
        line = GetComponent<LineRenderer>();  // LineRenderer ������Ʈ �ʱ�ȭ
    }

    private void Update()
    {
        // ��Ŭ�� ������ �ִ� ���� ���� (��� ����)
        if (!isGrappling && Input.GetMouseButton(1))
        {
            LockTarget(); // ���������� Raycast�� ���� (��� ���� ���� ������ ǥ��)
        }

        // ��Ŭ�� �� ��: ������ �Ϸ�� ���¸� ����
        if (!isGrappling && Input.GetMouseButtonUp(1) && isTargetLocked)
        {
            ExecuteGrapple(); // �����ߴ� ��� ���� �׷��ø� ����
        }

        // 0���� ������ ���� ����
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            CancelGrappleTarget();
        }

        // ���� �������� �������� �׻� �÷��̾� ��ġ�� ����
        if (line.enabled)
        {
            line.SetPosition(0, transform.position);
        }

        if (isTargetLocked)
        {
            float distanceToTarget = Vector2.Distance(transform.position, lockedHit.point);
            if (distanceToTarget > maxDistance)
                CancelGrappleTarget();
        }

        // �÷��̾� �̵� ó��
        if (isRetractingPlayer)
            HandlePlayerRetract();

        // ������Ʈ ������� ó��
        if (isRetractingObject)
            HandleObjectRetract();

        // �ӵ� ���׷��̵� ó��
        if (isUpgrade == true)
            SpeedUpgrade();
    }

    private void SpeedUpgrade() 
    {
        // �ӵ� ���׷��̵� ��� ����
    }

    // �׷��ø� �� �߻� �޼���
    private void StartGrapple()
    {
        Vector2 origin = transform.position;
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mouseWorldPos - origin).normalized;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, maxDistance, grapplableMask);

        if (hit.collider != null)
        {
            isGrappling = true;
            line.enabled = true;
            line.positionCount = 2;

            int targetLayer = hit.collider.gameObject.layer;

            // ������Ʈ �׷��ø� ó��
            if (targetLayer == LayerMask.NameToLayer("Grappleable"))
            {
                targetObject = hit.collider.transform;

                // �÷��̾�� ������Ʈ �浹 ���� ����
                Collider2D playerCol = GetComponentInParent<Collider2D>();
                Collider2D targetCol = targetObject.GetComponent<Collider2D>();

                if (playerCol && targetCol)
                    Physics2D.IgnoreCollision(playerCol, targetCol, true);

                // �÷��̾� �� 1f �������� ����� ��ġ ���
                Vector2 dirToPlayer = ((Vector2)transform.position - (Vector2)targetObject.position).normalized;
                pullStopPosition = (Vector2)transform.position + dirToPlayer * -1f;

                StartCoroutine(Grapple(targetObject.position, false));
            }
            // �� �׷��ø� ó��
            else if (targetLayer == LayerMask.NameToLayer("Grappeable2"))
            {
                target = hit.point; // �� ��ġ�� ����
                StartCoroutine(Grapple(target, true));
            }
        }
    }

    // �÷��̾ ������ �������� ó��
    private void HandlePlayerRetract()
    {
        // ���� �ð�ȭ ����
        transform.parent.position = Vector2.MoveTowards(transform.parent.position, target, grappleSpeed * Time.deltaTime);
        line.SetPosition(0, transform.position);
        line.SetPosition(1, target);

        // ��ǥ ���� ���� �� ����
        if (Vector2.Distance(transform.parent.position, target) < 0.5f)
            ResetGrapple();
    }

    // ������Ʈ�� ������� ó��
    private void HandleObjectRetract()
    {
        if (!targetObject)
        {
            ResetGrapple();
            return;
        }

        retractTimer += Time.deltaTime;
        targetObject.position = Vector2.MoveTowards(targetObject.position, pullStopPosition, grappleSpeed * Time.deltaTime);

        // ���� �ð�ȭ ����
        line.SetPosition(0, transform.position);
        line.SetPosition(1, targetObject.position);

        // ���� �� ���� ó��
        if (Vector2.Distance(targetObject.position, pullStopPosition) < 0.1f || retractTimer > 3f)
        {
            if (targetObject.CompareTag("Collectible"))
            {
                itemCount++;
                Destroy(targetObject.gameObject);
            }
            else if (targetObject.CompareTag("Grapplable"))
            {
                StartCoroutine(StunObject(targetObject));
            }
            ResetGrapple();
        }
    }

    // �׷��ø� ���� �ʱ�ȭ
    private void ResetGrapple()
    {
        if (targetObject)
        {
            Collider2D playerCol = GetComponentInParent<Collider2D>();
            Collider2D targetCol = targetObject.GetComponent<Collider2D>();

            if (playerCol && targetCol)
                Physics2D.IgnoreCollision(playerCol, targetCol, false);
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
        float time = 0.2f;

        line.enabled = true;
        line.positionCount = 2;

        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.position);

        while (t < time)
        {
            t += grappleShootSpeed * Time.deltaTime;
            Vector2 newPos = Vector2.Lerp(transform.position, targetPosition, t / time);
            line.SetPosition(1, newPos);
            yield return null;
        }

        line.SetPosition(1, targetPosition);

        if (isPlayerMoving)
            isRetractingPlayer = true;
        else
            isRetractingObject = true;
    }

    // �� ������Ʈ ���� ȿ�� �ڷ�ƾ
    IEnumerator StunObject(Transform obj)
    {
        SpriteRenderer sp = obj.GetComponent<SpriteRenderer>();

        if (sp)
        {
            Color originalColor = sp.color;
            sp.color = Color.yellow; // ���� ����
            yield return new WaitForSeconds(0.5f);
            sp.color = originalColor; // ���� ���� ����
        }
        ResetGrapple();
    }

    // ����ĳ��Ʈ�� Ÿ���� ���߰�, ���θ� ǥ��
    private void LockTarget()
    {
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxDistance, grapplableMask);

        if (hit.collider != null)
        {
            isTargetLocked = true;
            lockedHit = hit;

            // ���� ��ǥ �� ȭ�� ��ǥ ��ȯ
            Vector3 screenPos = Camera.main.WorldToScreenPoint(hit.point);
            crosshair.transform.position = screenPos;

            crosshair.SetActive(true);  // ������ ���̰�
        }
        else
        {
            crosshair.SetActive(false); // ���� �� �Ǹ� ����
        }
    }

    // ������ lockedHit ������ �̿��ؼ� ���� StartGrapple() ���� ����
    private void ExecuteGrapple()
    {
        if (!lockedHit.collider)
        {
            isTargetLocked = false;
            line.enabled = false;
            crosshair.SetActive(false);
            return;
        }

        isGrappling = true;
        int targetLayer = lockedHit.collider.gameObject.layer;

        if (targetLayer == LayerMask.NameToLayer("Grappleable"))
        {
            targetObject = lockedHit.collider.transform;
            Vector2 dirToPlayer = ((Vector2)transform.position - (Vector2)targetObject.position).normalized;
            pullStopPosition = (Vector2)transform.position + dirToPlayer * -1f;
            StartCoroutine(Grapple(targetObject.position, false));
        }
        else if (targetLayer == LayerMask.NameToLayer("Grappeable2"))
        {
            target = lockedHit.point;
            StartCoroutine(Grapple(target, true));
        }

        isTargetLocked = false; // ���� �� Ÿ�� ����
        crosshair.SetActive(false);
    }

    // 0�� �Է����� Ÿ���� ���
    private void CancelGrappleTarget()
    {
        if (isTargetLocked)
        {
            isTargetLocked = false;
            crosshair.SetActive(false);  // ������ ����
        }

        line.enabled = false;
    }

}