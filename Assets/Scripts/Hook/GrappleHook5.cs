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
    //[SerializeField] float grappleShootSpeed = 20f; // 훅 발사 속도 (현재 미사용)

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

    private enum LookDirection { Up, Down, Left, Right } // 바라보는 방향 부분
    [SerializeField] private Animator animator;  // Hooke에서 Animator를 연결

    // 파티클 관련 부분
    public GameObject EndVFX; // 끝 부분 파티클(현재 미사용)
    public GameObject StartVFX; // 시작되는 player의 파티클
    private Vector2 grapplePoint; // 정확한 충돌 위치

    // 사인 파형 로프 액션 부분
    [SerializeField][Range(0.01f, 4)] private float startWaveSize = 2f; // 파형 크기 조정 변수
    [SerializeField][Range(1, 50)] private float ropeProgressionSpeed = 5f; // 로프가 뻗는 속도
    [SerializeField][Range(2, 100)] private int ropeResolution = 20; // LineRenderer 점 수

    private void Start()
    {
        line = GetComponent<LineRenderer>();  // LineRenderer 컴포넌트 초기화
        playerScript = GetComponentInParent<Player>();
    }

    private void Update()
    {
        // 우클릭 누르고 있는 동안 crosshair 보이기
        if (!isGrappling && Input.GetMouseButton(1))
        {
            ShowCrosshair();
        }

        // 우클릭을 떼는 순간 → 발사 + crosshair 숨김
        if (!isGrappling && Input.GetMouseButtonUp(1))
        {
            crosshair.SetActive(false);
            StartGrapple();
        }

        // (안전하게 처리: 그래플링 중에도 crosshair 끄기)
        if (isGrappling)
        {
            crosshair.SetActive(false);
        }

        // 기존: 라인 렌더러 시작점 갱신, 이동 처리 등 유지
        if (line.enabled)
        {
            line.SetPosition(0, transform.position);
        }

        if (isRetractingPlayer)
            HandlePlayerRetract();

        if (isRetractingObject)
            HandleObjectRetract();
    }


    // 조준점 위치 설정 메서드
    private void ShowCrosshair()
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 screenPos = Camera.main.WorldToScreenPoint(mouseWorldPos);
        crosshair.transform.position = screenPos;
        crosshair.SetActive(true);
    }


    // 그래플링 훅 발사 메서드
    private void StartGrapple()
    {
        isGrappling = true;

        Vector2 origin = transform.position;
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mouseWorldPos - origin).normalized;
        float distanceToMouse = Vector2.Distance(origin, mouseWorldPos);

        // 최대 거리 제한 적용
        if (distanceToMouse > maxDistance)
        {
            mouseWorldPos = origin + direction * maxDistance;
        }

        grapplePoint = mouseWorldPos;

        StartCoroutine(Grapple(grapplePoint));
    }



    // 플레이어가 벽으로 끌려가는 처리
    private void HandlePlayerRetract()
    {
        playerScript.transform.position = Vector2.MoveTowards(playerScript.transform.position, target, grappleSpeed * Time.deltaTime);

        DrawStraightRope(transform.position, target);

        if (Vector2.Distance(playerScript.transform.position, target) < 0.5f)
        {
            ResetGrapple();

            if (isUpgrade && !isSpeedBoosting)
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

        DrawStraightRope(transform.position, targetObject.position); // 직선 라인 유지

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
        StopAllParticles(StartVFX);
        //StopAllParticles(EndVFX);
    }

    // 훅 발사 애니메이션 코루틴
    IEnumerator Grapple(Vector2 targetPosition)
    {
        FaceDirection(targetPosition);
        PlayAllParticles(StartVFX);

        line.enabled = true;
        line.positionCount = ropeResolution;

        float time = 0f;
        Vector2 startPoint = transform.position;
        Vector2 direction = (targetPosition - startPoint).normalized;
        float totalDistance = Vector2.Distance(startPoint, targetPosition);
        Vector2 currentTip = startPoint;

        while (time < 1f)
        {
            time += Time.deltaTime * ropeProgressionSpeed;
            float progress = Mathf.Clamp01(time);
            currentTip = Vector2.Lerp(startPoint, targetPosition, progress);

            for (int i = 0; i < ropeResolution; i++)
            {
                float delta = (float)i / (ropeResolution - 1);
                Vector2 point = Vector2.Lerp(startPoint, currentTip, delta);
                Vector2 normal = new Vector2(-direction.y, direction.x);
                float wave = Mathf.Sin(delta * Mathf.PI * 2f * 2f) * startWaveSize * (1f - progress);
                line.SetPosition(i, point + normal * wave);
            }

            yield return null;
        }

        // 충돌 처리
        Collider2D hitCollider = Physics2D.OverlapCircle(currentTip, 0.1f, grapplableMask);
        if (hitCollider != null)
        {
            GameObject hitObject = hitCollider.gameObject;
            int targetLayer = hitObject.layer;

            if (targetLayer == LayerMask.NameToLayer("Obj") || targetLayer == LayerMask.NameToLayer("Enemy"))
            {
                targetObject = hitObject.transform;
                Vector2 dirToPlayer = ((Vector2)transform.position - (Vector2)targetObject.position).normalized;
                pullStopPosition = (Vector2)transform.position + dirToPlayer * -1f;
                isRetractingObject = true;
            }
            else if (targetLayer == LayerMask.NameToLayer("Wall"))
            {
                target = currentTip;
                isRetractingPlayer = true;
            }

            if (hitObject.CompareTag("Collectible"))
            {
                isUpgrade = true;
                Destroy(hitObject); // 예: 업그레이드 아이템
            }

            if (EndVFX != null)
            {
                EndVFX.transform.position = currentTip;
                PlayAllParticles(EndVFX);
            }
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
            ResetGrapple();
        }


        // 직선 로프 고정
        DrawStraightRope(transform.position, currentTip);
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

    // 0번 입력으로 타겟팅 취소
    private void CancelGrappleTarget()
    {
        if (isTargetLocked)
        {
            isTargetLocked = false;
            //crosshair.SetActive(false);  // 조준점 숨김
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
    }

    // 속도 증가 코루틴
    IEnumerator SpeedBoost()
    {
        isSpeedBoosting = true;

        if (playerScript != null)
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

    // 이펙트 재생 함수
    private void PlayAllParticles(GameObject vfxRoot)
    {
        if (vfxRoot == null) return;

        ParticleSystem[] particles = vfxRoot.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in particles)
        {
            ps.Play();
        }
    }

    // 도착 시 이펙트 재생 정지
    private void StopAllParticles(GameObject vfxRoot)
    {
        if (vfxRoot == null) return;

        ParticleSystem[] particles = vfxRoot.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in particles)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    // 사인 파형 로프 그리기 
    private void DrawStraightRope(Vector2 from, Vector2 to)
    {
        for (int i = 0; i < ropeResolution; i++)
        {
            float delta = (float)i / (ropeResolution - 1);
            Vector2 pos = Vector2.Lerp(from, to, delta);
            line.SetPosition(i, pos);
        }
    }

}