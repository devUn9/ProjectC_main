using System.Collections;
using UnityEngine;

public class GrappleHook2 : MonoBehaviour
{
    // 로프 시각화를 위한 LineRenderer 컴포넌트
    private LineRenderer line;

    [Header("Grappling 설정")]
    [SerializeField] LayerMask grapplableMask;   // 그래플링 가능한 오브젝트 레이어
    [SerializeField] float maxDistance = 10f;     // 그래플링 최대 사거리
    [SerializeField] float grappleSpeed = 10f;    // 오브젝트가 끌려오는 속도
    [SerializeField] float grappleShootSpeed = 20f; // 훅 발사 애니메이션 속도

    private bool isGrappling = false;             // 현재 그래플링 중인지 여부
    [HideInInspector] public bool retracting = false; // 오브젝트가 끌려오는 중인지 여부

    private Vector2 target;        // 훅이 맞은 위치 (현재는 사용되지 않음)
    private Transform targetObject; // 실제로 끌어올 대상 오브젝트

    private void Start()
    {
        // LineRenderer 컴포넌트 초기화
        line = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        // 우클릭 시 그래플링 시작 (이미 사용 중이 아닐 때만)
        if (Input.GetMouseButtonDown(1) && !isGrappling)
        {
            StartGrapple();
        }

        // 오브젝트가 끌려오는 중일 때 실행
        if (retracting && targetObject != null)
        {
            // 대상 오브젝트를 플레이어 쪽으로 이동
            targetObject.position = Vector2.MoveTowards(
                targetObject.position,
                transform.position,
                grappleSpeed * Time.deltaTime
            );

            // 로프 시각화 업데이트 (플레이어 ↔ 오브젝트)
            line.SetPosition(0, transform.position);
            line.SetPosition(1, targetObject.position);

            // 오브젝트가 플레이어 근처에 도달하면 그래플링 종료
            if (Vector2.Distance(targetObject.position, transform.position) < 0.5f)
            {
                retracting = false;
                isGrappling = false;
                line.enabled = false;
                targetObject = null;
            }
        }
    }

    // 그래플링 훅 발사 로직
    private void StartGrapple()
    {
        // 마우스 방향으로 Raycast 발사
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxDistance, grapplableMask);

        // 그래플링 가능한 오브젝트에 닿았을 때
        if (hit.collider != null)
        {
            isGrappling = true;
            targetObject = hit.collider.transform;   // 대상 오브젝트 저장

            // 로프 시각화 활성화
            line.enabled = true;
            line.positionCount = 2;

            // 훅 발사 애니메이션 시작
            StartCoroutine(Grapple());
        }
    }

    // 훅 발사 애니메이션 코루틴
    IEnumerator Grapple()
    {
        float t = 0f;
        float time = 10f;  // 애니메이션 진행 시간 (가상의 값)

        // 로프 시작 위치 초기화 (플레이어 기준)
        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.position);

        Vector2 hookPos;

        // 훅이 오브젝트까지 뻗어나가는 애니메이션
        for (; t < time; t += grappleShootSpeed * Time.deltaTime)
        {
            // 훅 위치를 점진적으로 대상 오브젝트 방향으로 이동
            hookPos = Vector2.Lerp(transform.position, targetObject.position, t / time);

            // 로프 갱신
            line.SetPosition(0, transform.position);
            line.SetPosition(1, hookPos);

            yield return null;  // 프레임 대기
        }

        // 훅이 완전히 도달하면, 로프 끝점을 대상 오브젝트 위치로 고정
        line.SetPosition(1, targetObject.position);

        // 이제 오브젝트를 끌어오는 단계로 전환
        retracting = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Grapplable"))
        {
            // 충돌 시 상대 Rigidbody 속도 초기화 (밀림 방지)
            Rigidbody2D rb = collision.collider.attachedRigidbody;
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }

}
