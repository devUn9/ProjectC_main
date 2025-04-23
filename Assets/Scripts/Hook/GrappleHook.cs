using System.Collections;
using UnityEngine;

public class GrappleHook : MonoBehaviour
{
    // 로프 시각화를 위한 라인 렌더러
    private LineRenderer line;

    [Header("Grappling 설정")]
    [SerializeField] LayerMask grapplableMask;   // 그래플링 훅이 걸릴 수 있는 오브젝트 레이어
    [SerializeField] float maxDistance = 10f;     // 최대 사거리
    [SerializeField] float grappleSpeed = 10f;    // 플레이어가 끌려가는 속도
    [SerializeField] float grappleShootSpeed = 20f; // 훅이 발사되는 속도

    private bool isGrappling = false;  // 그래플 상태 여부
    [HideInInspector] public bool retracting = false; // 훅이 고정된 후, 플레이어가 당겨지는 상태

    private Vector2 target;   // 훅이 고정된 목표 지점

    private void Start()
    {
        line = GetComponent<LineRenderer>();  // LineRenderer 컴포넌트 참조
    }

    private void Update()
    {
        // 우클릭 시 그래플 발사 (이미 그래플링 중이 아닐 때만)
        if (Input.GetMouseButtonDown(1) && !isGrappling)
        {
            StartGrapple();
        }

        // 플레이어가 목표 지점으로 끌려가는 과정
        if (retracting)
        {
            // 현재 위치에서 타겟까지 이동
            Vector2 grapplePos = Vector2.Lerp(transform.position, target, grappleSpeed * Time.deltaTime);
            transform.position = grapplePos;

            // 로프의 시작점 갱신
            line.SetPosition(0, transform.position);

            // 목표 지점에 도달하면 종료
            if (Vector2.Distance(transform.position, target) < 0.5f)
            {
                retracting = false;
                isGrappling = false;
                line.enabled = false;
            }
        }
    }

    // 그래플링 훅 발사 메서드
    private void StartGrapple()
    {
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;

        // 레이캐스트로 충돌 체크
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxDistance, grapplableMask);

        if (hit.collider != null)
        {
            isGrappling = true;
            target = hit.point;

            // 로프 활성화 및 초기화
            line.enabled = true;
            line.positionCount = 2;

            // 훅 발사 코루틴 시작
            StartCoroutine(Grapple());
        }
    }

    // 훅 발사 애니메이션 코루틴
    IEnumerator Grapple()
    {
        float t = 0f;
        float time = 10f;  // 발사 애니메이션 조절용 (하드코딩)

        // 로프의 시작과 끝점을 초기화
        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.position);

        Vector2 newPos;

        // 훅이 타겟으로 점점 나아가는 애니메이션
        for (; t < time; t += grappleShootSpeed * Time.deltaTime)
        {
            newPos = Vector2.Lerp(transform.position, target, t / time);

            line.SetPosition(0, transform.position);
            line.SetPosition(1, newPos);

            yield return null;
        }

        // 훅이 타겟에 도달
        line.SetPosition(1, target);
        retracting = true;  // 이제 플레이어가 끌려가기 시작
    }
}
