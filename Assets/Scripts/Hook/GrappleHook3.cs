using System;
using System.Collections;
using UnityEngine;

public class GrappleHook3 : MonoBehaviour
{
    // 로프 시각화를 위한 LineRenderer 컴포넌트
    private LineRenderer line;

    [Header("Grappling 설정")]
    [SerializeField] LayerMask grapplableMask;   // 그래플링 가능한 오브젝트의 레이어 마스크
    [SerializeField] float maxDistance = 10f;     // 그래플링 최대 거리
    [SerializeField] float grappleSpeed = 10f;    // 끌려오는 속도 (플레이어나 오브젝트)
    [SerializeField] float grappleShootSpeed = 20f; // 훅 발사 속도 (현재 미사용)

    private bool isGrappling = false;             // 그래플링 중인지 여부

    private Vector2 target;                       // 플레이어가 이동할 목표 지점 (벽 그래플링)
    private Transform targetObject;               // 끌어올 오브젝트 참조
    private float retractTimer = 0f;              // 오브젝트 끌기 제한 시간용 타이머
    public int itemCount;                         // 수집한 아이템 개수
    private Vector2 pullStopPosition;             // 오브젝트가 멈출 위치 (플레이어 앞)

    [HideInInspector] public bool isRetractingPlayer = false;  // 플레이어가 이동 중인지 여부
    [HideInInspector] public bool isRetractingObject = false;  // 오브젝트가 끌려오는 중인지 여부

    public bool isUpgrade = false;                // 속도 업그레이드 여부
    public float SpeedMultiplier => isUpgrade ? 1.5f : 1f;   // 속도 배율 계산

    private void Start()
    {
        line = GetComponent<LineRenderer>();  // LineRenderer 컴포넌트 초기화
    }

    private void Update()
    {
        // 우클릭 시 그래플링 발사 (그래플링 중이 아닐 때만)
        if (Input.GetMouseButtonDown(1) && !isGrappling)
        {
            StartGrapple();
        }

        // 플레이어 이동 처리
        if (isRetractingPlayer)
            HandlePlayerRetract();

        // 오브젝트 끌어오기 처리
        if (isRetractingObject)
            HandleObjectRetract();

        // 속도 업그레이드 처리
        if (isUpgrade == true)
            SpeedUpgrade();
    }

    private void SpeedUpgrade()
    {
        // 속도 업그레이드 기능 예정
    }

    // 그래플링 훅 발사 메서드
    private void StartGrapple()
    {
        // 마우스 방향으로 Raycast 발사
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxDistance, grapplableMask);

        if (hit.collider != null)
        {
            Debug.Log("Raycast 충돌: " + hit.collider.name);

            isGrappling = true;
            line.enabled = true;
            line.positionCount = 2;

            int targetLayer = hit.collider.gameObject.layer;
            Debug.Log("충돌 레이어: " + LayerMask.LayerToName(targetLayer));

            // 오브젝트 그래플링 처리
            if (targetLayer == LayerMask.NameToLayer("Grappleable"))
            {
                Debug.Log("오브젝트 끌어오기 시작");
                targetObject = hit.collider.transform;

                // 플레이어와 오브젝트 충돌 무시 설정
                Collider2D playerCol = GetComponent<Collider2D>();
                Collider2D targetCol = targetObject.GetComponent<Collider2D>();

                if (playerCol != null && targetCol != null)
                {
                    Physics2D.IgnoreCollision(playerCol, targetCol, true);
                }

                // 플레이어 앞 1f 지점으로 끌어올 위치 계산
                Vector2 directionToPlayer = ((Vector2)transform.position - (Vector2)targetObject.position).normalized;
                pullStopPosition = (Vector2)transform.position + directionToPlayer * -1f;

                StartCoroutine(Grapple(targetObject.position, false));
            }
            // 벽 그래플링 처리
            else if (targetLayer == LayerMask.NameToLayer("Grappeable2"))
            {
                Debug.Log("플레이어 이동 시작");
                target = hit.point;  // 벽 위치로 설정
                StartCoroutine(Grapple(target, true));
            }
        }
        else
        {
            Debug.Log("Raycast 미스");
        }
    }

    // 플레이어가 벽으로 끌려가는 처리
    private void HandlePlayerRetract()
    {
        transform.position = Vector2.MoveTowards(transform.position, target, grappleSpeed * Time.deltaTime);

        // 로프 시각화 갱신
        line.SetPosition(0, transform.position);
        line.SetPosition(1, target);

        // 목표 지점 도착 시 종료
        if (Vector2.Distance(transform.position, target) < 0.5f)
        {
            ResetGrapple();
        }
    }

    // 오브젝트를 끌어오는 처리
    private void HandleObjectRetract()
    {
        if (targetObject == null)
        {
            ResetGrapple();
            return;
        }

        retractTimer += Time.deltaTime;

        targetObject.position = Vector2.MoveTowards(targetObject.position, pullStopPosition, grappleSpeed * Time.deltaTime);

        // 로프 시각화 갱신
        line.SetPosition(0, transform.position);
        line.SetPosition(1, targetObject.position);

        // 도착 시 판정 처리
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
                Debug.Log("적 기절!");
                StartCoroutine(StunObject(targetObject));
                ResetGrapple();
            }
            else
            {
                Debug.Log("방해물 도착 - 플레이어 앞에 배치 완료");
                ResetGrapple();
            }
        }
    }

    // 그래플링 상태 초기화
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

    // 훅 발사 애니메이션 코루틴
    IEnumerator Grapple(Vector2 targetPosition, bool isPlayerMoving)
    {
        float t = 0f;
        float time = 0.2f;

        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.position);

        while (t < time)
        {
            t += grappleShootSpeed * Time.deltaTime;

            Vector2 newPos = Vector2.Lerp(transform.position, targetPosition, t / time);

            line.SetPosition(0, transform.position);
            line.SetPosition(1, newPos);

            yield return null;
        }

        line.SetPosition(1, targetPosition);

        if (isPlayerMoving)
            isRetractingPlayer = true;
        else
            isRetractingObject = true;
    }

    // 적 오브젝트 기절 효과 코루틴
    IEnumerator StunObject(Transform obj)
    {
        SpriteRenderer sp = obj.GetComponent<SpriteRenderer>();

        if (sp != null)
        {
            Color originalColor = sp.color;

            sp.color = Color.yellow;   // 기절 색상
            yield return new WaitForSeconds(0.5f);
            sp.color = originalColor;  // 원래 색상 복구
        }
        else
        {
            Debug.Log("스프라이트가 없다");
        }

        ResetGrapple();
    }
}
