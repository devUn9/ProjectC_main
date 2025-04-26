using System.Collections;
using UnityEngine;

public class ObjectGrapple : MonoBehaviour
{
    private LineRenderer line;

    [Header("Grappling 설정")]
    [SerializeField] LayerMask objectMask;      // 오브젝트 그래플링 전용 레이어
    [SerializeField] float maxDistance = 10f;
    [SerializeField] float grappleSpeed = 10f;
    [SerializeField] float grappleShootSpeed = 20f;

    private bool isGrappling = false;

    private Transform targetObject;
    private float retractTimer = 0f;
    private Vector2 pullStopPosition;

    public int itemCount;

    private bool isRetractingObject = false;

    private void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && !isGrappling)
        {
            StartGrapple();
        }

        if (isRetractingObject)
            HandleObjectRetract();
    }

    private void StartGrapple()
    {
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxDistance, objectMask);

        if (hit.collider != null)
        {
            Debug.Log("오브젝트 그래플링 시작: " + hit.collider.name);

            isGrappling = true;
            line.enabled = true;
            line.positionCount = 2;

            targetObject = hit.collider.transform;

            // 충돌 무시 처리
            Collider2D playerCol = GetComponent<Collider2D>();
            Collider2D targetCol = targetObject.GetComponent<Collider2D>();

            if (playerCol != null && targetCol != null)
            {
                Physics2D.IgnoreCollision(playerCol, targetCol, true);
            }

            // 플레이어 앞 1유닛 위치 계산
            Vector2 directionToPlayer = ((Vector2)transform.position - (Vector2)targetObject.position).normalized;
            pullStopPosition = (Vector2)transform.position + directionToPlayer * 1.0f;

            StartCoroutine(Grapple(targetObject.position));
        }
        else
        {
            Debug.Log("오브젝트 그래플링 실패");
        }
    }

    private void HandleObjectRetract()
    {
        if (targetObject == null)
        {
            ResetGrapple();
            return;
        }

        retractTimer += Time.deltaTime;

        targetObject.position = Vector2.MoveTowards(targetObject.position, pullStopPosition, grappleSpeed * Time.deltaTime);

        line.SetPosition(0, transform.position);
        line.SetPosition(1, targetObject.position);

        if (Vector2.Distance(targetObject.position, pullStopPosition) < 0.1f || retractTimer > 3f)
        {
            if (targetObject.CompareTag("Collectible"))
            {
                itemCount++;
                Debug.Log($"아이템 획득! 총 개수: {itemCount}");
                Destroy(targetObject.gameObject);
                ResetGrapple();
            }
            else if (targetObject.CompareTag("Grapplable"))
            {
                Debug.Log("기절!");
                StartCoroutine(StunObject(targetObject));
            }
            else
            {
                Debug.Log("방해물 도착 - 플레이어 앞에 배치 완료");
                ResetGrapple();
            }
        }
    }

    private void ResetGrapple()
    {
        isRetractingObject = false;
        isGrappling = false;
        line.enabled = false;
        targetObject = null;
        retractTimer = 0f;
    }

    IEnumerator Grapple(Vector2 targetPosition)
    {
        float t = 0f;
        float time = 0.2f;

        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.position);

        while (t < time)
        {
            t += Time.deltaTime;
            Vector2 newPos = Vector2.Lerp(transform.position, targetPosition, t / time);

            line.SetPosition(0, transform.position);
            line.SetPosition(1, newPos);

            yield return null;
        }

        line.SetPosition(1, targetPosition);
        isRetractingObject = true;
    }

    IEnumerator StunObject(Transform obj)
    {
        SpriteRenderer sp = obj.GetComponent<SpriteRenderer>();

        if (sp != null)
        {
            Color originalColor = sp.color;
            sp.color = Color.yellow;

            yield return new WaitForSeconds(2f);

            sp.color = originalColor;
        }
        else
        {
            Debug.Log("스프라이트가 없다");
        }

        ResetGrapple();
    }
}
