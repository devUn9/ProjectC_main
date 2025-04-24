using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class GrappleHook3 : MonoBehaviour
{
    // 로프 시각화를 위한 LineRenderer 컴포넌트
    private LineRenderer line;

    [Header("Grappling 설정")]
    [SerializeField] LayerMask grapplableMask;   // 그래플링 가능한 오브젝트 레이어
    [SerializeField] float maxDistance = 10f;     // 그래플링 최대 사거리
    [SerializeField] float grappleSpeed = 10f;    // 오브젝트가 끌려오는 속도
    [SerializeField] float grappleShootSpeed = 20f; // 훅 발사 애니메이션 속도

    private bool isGrappling = false;             // 현재 그래플링 중인지 여부

    private Vector2 target;        // 훅이 맞은 위치 (현재는 사용되지 않음)
    private Transform targetObject; // 실제로 끌어올 대상 오브젝트
    private float retractTimer = 0f; // 오브젝트를 끌어올 때, 판정을 너무 늦게 하거나 못 할 수도 있기에 타이머 지정
    public int itemCount;

    [HideInInspector] public bool isRetractingPlayer = false; // 플레이어가 끌려가지는 여부
    [HideInInspector] public bool isRetractingObject = false; // 오브젝트가 끌려오는지 여부

    private void Start()
    {
        line = GetComponent<LineRenderer>();  // LineRenderer 컴포넌트 참조
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

    // 그래플링 훅 발사 메서드
    private void StartGrapple()
    {
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

            if (targetLayer == LayerMask.NameToLayer("Grappleable"))
            {
                Debug.Log("오브젝트 끌어오기 시작");
                targetObject = hit.collider.transform;
                StartCoroutine(Grapple(targetObject.position, false));
            }
            else if (targetLayer == LayerMask.NameToLayer("Grappeable2"))
            {
                Debug.Log("플레이어 이동 시작");
                target = hit.collider.transform.position;
                StartCoroutine(Grapple(target, true));
            }
        }
        else
        {
            Debug.Log("Raycast 미스");
        }
    }

    // 플레이어가 끌려가는 로프 발사
    private void HandlePlayerRetract()
    {
        // 플레이어 이동
        transform.position = Vector2.MoveTowards(transform.position, target, grappleSpeed * Time.deltaTime);

        // 로프 갱신
        line.SetPosition(0, transform.position);
        line.SetPosition(1, target);

        // 도착 시 종료
        if (Vector2.Distance(transform.position, target) < 0.5f)
        {
            ResetGrapple();
        }
    }

    // 플레이어가 끌고 오는 로프 발사
    private void HandleObjectRetract()
    {
        if(targetObject == null)
        {
            ResetGrapple();
            return;
        }

        retractTimer += Time.deltaTime;

        targetObject.position = Vector2.MoveTowards(targetObject.position, transform.position, grappleSpeed * Time.deltaTime);

        // 로프 갱신
        line.SetPosition(0, transform.position);
        line.SetPosition(1, targetObject.position);

        // 도착 시 종료
        if (Vector2.Distance(targetObject.position, transform.position) < 0.1f || retractTimer > 3f)
        {
            if (targetObject.CompareTag("Collectible"))
            {
                itemCount++;
                Debug.Log($"아이템 획득! 총 개수: {itemCount}");
                Destroy(targetObject.gameObject);
            }
            ResetGrapple();
        }
    }
    
    private void ResetGrapple()
    {
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
        float time = 0.2f;  // 발사 애니메이션 조절용 (하드코딩)

        // 로프의 시작과 끝점을 초기화
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

        // 훅이 타겟에 도달
        line.SetPosition(1, targetPosition);

        // 발사 후 상태 전환
        if (isPlayerMoving)
            isRetractingPlayer = true;
        else
            isRetractingObject = true;
    }

}
