using System.Collections;
using UnityEngine;

public class GrappleHook5 : MonoBehaviour
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

    // 업그레이드 시 속도 증가 부분 구현
    public bool isUpgrade = false;                // 속도 업그레이드 여부
    private Player playerScript; // 플레이어 스크립트 받아와서 속도 체크
    private float originalSpeed;
    [SerializeField] private float speedBoostDuration = 2f;
    [SerializeField] private float speedMultiplier = 1.5f;
    private bool isSpeedBoosting = false;

    private bool isTargetLocked = false;   // 타겟이 고정되었는지 여부
    private RaycastHit2D lockedHit;        // 고정된 타겟 정보 저장

    [SerializeField] private GameObject crosshair;  // 조준점 이미지 오브젝트

    private enum LookDirection { Up, Down, Left, Right }
    [SerializeField] private Animator animator;  // Hooke에서 Animator를 연결

    private void Start()
    {
        line = GetComponent<LineRenderer>();  // LineRenderer 컴포넌트 초기화
        playerScript = GetComponentInParent<Player>();
    }

    private void Update()
    {
        // 우클릭 누르고 있는 동안 조준 (계속 감지)
        if (!isGrappling && Input.GetMouseButton(1))
        {
            LockTarget(); // 지속적으로 Raycast로 감지 (대상 있을 때만 조준점 표시)
        }

        // 우클릭 뗄 때: 조준이 완료된 상태면 실행
        if (!isGrappling && Input.GetMouseButtonUp(1) && isTargetLocked)
        {
            ExecuteGrapple(); // 조준했던 대상에 대해 그래플링 실행
        }

        // 0번을 누르면 연결 해제
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            CancelGrappleTarget();
        }

        // 라인 렌더러의 시작점을 항상 플레이어 위치로 갱신
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

        // 플레이어 이동 처리
        if (isRetractingPlayer)
            HandlePlayerRetract();

        // 오브젝트 끌어오기 처리
        if (isRetractingObject)
            HandleObjectRetract();
    }

    // 그래플링 훅 발사 메서드
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

            // 오브젝트 그래플링 처리
            if (targetLayer == LayerMask.NameToLayer("Obj") || targetLayer == LayerMask.NameToLayer("Enemy"))
            {
                targetObject = hit.collider.transform;

                // 플레이어와 오브젝트 충돌 무시 설정
                Collider2D playerCol = GetComponentInParent<Collider2D>();
                Collider2D targetCol = targetObject.GetComponent<Collider2D>();

                if (playerCol && targetCol)
                    Physics2D.IgnoreCollision(playerCol, targetCol, true);

                // 플레이어 앞 1f 지점으로 끌어올 위치 계산
                Vector2 dirToPlayer = ((Vector2)transform.position - (Vector2)targetObject.position).normalized;
                pullStopPosition = (Vector2)transform.position + dirToPlayer * -1f;

                StartCoroutine(Grapple(targetObject.position, false));
            }
            // 벽 그래플링 처리
            else if (targetLayer == LayerMask.NameToLayer("Wall"))
            {
                target = hit.point; // 벽 위치로 설정
                StartCoroutine(Grapple(target, true));
            }
        }
    }

    // 플레이어가 벽으로 끌려가는 처리
    private void HandlePlayerRetract()
    {
        // 로프 시각화 갱신
        transform.parent.position = Vector2.MoveTowards(transform.parent.position, target, grappleSpeed * Time.deltaTime);
        line.SetPosition(0, transform.position);
        line.SetPosition(1, target);

        // 목표 지점 도착 시 종료
        if (Vector2.Distance(transform.parent.position, target) < 0.5f)
        {
            ResetGrapple();

            // 업그레이드 조건이 체크되면 속도가 증가하도록
            if(isUpgrade && !isSpeedBoosting)
            {
                StartCoroutine(SpeedBoost());
            }
        }
    }

    // 오브젝트를 끌어오는 처리
    private void HandleObjectRetract()
    {
        if (!targetObject)
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
            if (targetObject.CompareTag("Item"))
            {
                itemCount++;
                Destroy(targetObject.gameObject);
            }
            else if (targetObject.CompareTag("Enemy"))
            {
                StartCoroutine(StunObject(targetObject));
            }
            else if (targetObject.CompareTag("Collectible"))
            {
                isUpgrade = true;
                Destroy(targetObject.gameObject);
                //StartCoroutine(SpeedBoost());
            }
            ResetGrapple();
        }
    }

    // 그래플링 상태 초기화
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

    // 훅 발사 애니메이션 코루틴
    IEnumerator Grapple(Vector2 targetPosition, bool isPlayerMoving)
    {
        FaceDirection(targetPosition);

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

    // 적 오브젝트 기절 효과 코루틴
    IEnumerator StunObject(Transform obj)
    {
        SpriteRenderer sp = obj.GetComponent<SpriteRenderer>();

        if (sp)
        {
            Color originalColor = sp.color;
            sp.color = Color.yellow; // 기절 색상
            yield return new WaitForSeconds(0.5f);
            sp.color = originalColor; // 원래 색상 복구
        }
        //ResetGrapple();
    }

    // 레이캐스트로 타겟을 맞추고, 라인만 표시
    private void LockTarget()
    {
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxDistance, grapplableMask);

        if (hit.collider != null)
        {
            isTargetLocked = true;
            lockedHit = hit;

            // 월드 좌표 → 화면 좌표 변환
            Vector3 screenPos = Camera.main.WorldToScreenPoint(hit.point);
            crosshair.transform.position = screenPos;

            crosshair.SetActive(true);  // 조준점 보이게
        }
        else
        {
            crosshair.SetActive(false); // 감지 안 되면 숨김
        }
    }

    // 고정된 lockedHit 정보를 이용해서 기존 StartGrapple() 로직 실행
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

        if (targetLayer == LayerMask.NameToLayer("Obj") || targetLayer == LayerMask.NameToLayer("Enemy"))
        {
            targetObject = lockedHit.collider.transform;
            Vector2 dirToPlayer = ((Vector2)transform.position - (Vector2)targetObject.position).normalized;
            pullStopPosition = (Vector2)transform.position + dirToPlayer * -1f;
            StartCoroutine(Grapple(targetObject.position, false));
        }
        else if (targetLayer == LayerMask.NameToLayer("Wall"))
        {
            target = lockedHit.point;
            StartCoroutine(Grapple(target, true));
        }

        isTargetLocked = false; // 실행 후 타겟 해제
        crosshair.SetActive(false);
    }

    // 0번 입력으로 타겟팅 취소
    private void CancelGrappleTarget()
    {
        if (isTargetLocked)
        {
            isTargetLocked = false;
            crosshair.SetActive(false);  // 조준점 숨김
        }

        line.enabled = false;
    }

    private LookDirection GetLookDirection(Vector2 direction)
    {
        direction = direction.normalized;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            return direction.x > 0 ? LookDirection.Right : LookDirection.Left;
        }
        else
        {
            return direction.y > 0 ? LookDirection.Up : LookDirection.Down;
        }
    }

    private void FaceDirection(Vector2 targetPosition)
    {
        Vector2 dir = (targetPosition - (Vector2)transform.position).normalized;

        // Blend Tree용 파라미터 전달
        animator.SetFloat("VelocityX", Mathf.Round(dir.x));
        animator.SetFloat("VelocityY", Mathf.Round(dir.y));

        //Debug.Log($"[DEBUG] Set VelocityX: {Mathf.Round(dir.x)}, VelocityY: {Mathf.Round(dir.y)}");
    }

    // 속도 증가 코루틴
    IEnumerator SpeedBoost()
    {
        isSpeedBoosting = true;

        if(playerScript != null)
        {
            originalSpeed = playerScript.moveSpeed;
            playerScript.moveSpeed *= speedMultiplier;

            Debug.Log($"[SpeedBoost] 속도 증가! {originalSpeed} → {playerScript.moveSpeed}");

            yield return new WaitForSeconds(speedBoostDuration);

            playerScript.moveSpeed = originalSpeed;
            Debug.Log($"[SpeedBoost] 속도 복귀: {originalSpeed}");
        }
        isSpeedBoosting = false;
    }
}