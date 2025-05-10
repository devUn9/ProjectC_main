using UnityEngine;
using Unity.Cinemachine;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] private CinemachineCamera virtualCamera; // Cinemachine 가상 카메라
    [SerializeField] private float zoomSpeed = 5f; // 줌 속도
    [SerializeField] private float defaultSize = 5f; // 기본 Orthographic Size
    [SerializeField] private float zoomedSize = 10f; // 줌 아웃된 Orthographic Size
    [SerializeField] private float walkSpeed = 2f; // Ctrl 키 눌렀을 때의 이동 속도
    [SerializeField] private Player player; // Player 스크립트 참조

    [Header("마우스 방향 보정 설정")]
    [SerializeField] private float maxCorrectionDistance = 3f; // 최대 보정 거리
    [SerializeField] private float minCorrectionDistance = 0.5f; // 최소 보정 거리
    [SerializeField] private float correctionSpeed = 3f; // 보정 속도
    [SerializeField] private bool enableMouse = true; // 마우스 보정 활성화 여부

    private float currentSize;
    private float defaultPlayerSpeed; // 원래 Player의 moveSpeed 저장
    private Vector3 cameraOffset = Vector3.zero; // 카메라 오프셋
    private GameObject cameraTarget; // 카메라가 따라갈 별도의 타겟 오브젝트

    private void Start()
    {
        // Player 컴포넌트 확인
        if (player == null)
        {
            player = GetComponent<Player>();
            if (player == null)
            {
                Debug.LogError("Player 컴포넌트가 설정되지 않았습니다!");
            }
        }

        // 원래 Player의 moveSpeed 저장
        if (player != null)
        {
            defaultPlayerSpeed = player.moveSpeed;
        }

        // 카메라 타겟 오브젝트 생성
        SetupCameraTarget();

        // 초기 렌즈 설정
        if (virtualCamera != null && virtualCamera.Lens.Orthographic)
        {
            currentSize = defaultSize;
            var lens = virtualCamera.Lens;
            lens.OrthographicSize = currentSize;
            virtualCamera.Lens = lens;
        }
    }

    private void SetupCameraTarget()
    {
        // 이미 존재하는 카메라 타겟이 있는지 확인
        if (cameraTarget == null)
        {
            // 플레이어 위치에 빈 게임 오브젝트 생성
            cameraTarget = new GameObject("CameraTarget");
            cameraTarget.transform.position = transform.position;

            // 카메라가 이 타겟을 따라가도록 설정
            if (virtualCamera != null)
            {
                virtualCamera.Follow = cameraTarget.transform;
            }
        }
    }

    private void Update()
    {
        // 카메라 타겟이 플레이어를 따라가도록 업데이트
        UpdateCameraTargetPosition();

        if (virtualCamera != null && virtualCamera.Lens.Orthographic)
        {
            var lens = virtualCamera.Lens;
            float targetSize;

            // Ctrl 키가 눌렸는지 확인
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                // 키를 누르고 있으면 줌 아웃 시도 (크기 증가)
                targetSize = zoomedSize;

                if (player != null)
                {
                    player.moveSpeed = walkSpeed; // Player의 moveSpeed를 walkSpeed로 설정
                }

                // 마우스 방향으로 보정 적용
                if (enableMouse)
                {
                    UpdateCameraOffset(true);
                }
            }
            else
            {
                // 키를 떼면 원래 크기와 속도로 복귀
                targetSize = defaultSize;

                if (player != null)
                {
                    player.moveSpeed = defaultPlayerSpeed; // 원래 moveSpeed로 복구
                }

                // 오프셋 초기화
                if (enableMouse)
                {
                    UpdateCameraOffset(false);
                }
            }

            // 현재 크기와 목표 크기 사이의 다음 단계 계산
            float nextSize = Mathf.Lerp(currentSize, targetSize, zoomSpeed * Time.deltaTime);

            // 최종 크기 적용
            currentSize = nextSize;
            lens.OrthographicSize = currentSize;
            virtualCamera.Lens = lens;
        }
    }

    private void UpdateCameraTargetPosition()
    {
        if (cameraTarget != null && player != null)
        {
            // 카메라 타겟 위치를 플레이어 + 오프셋으로 설정
            cameraTarget.transform.position = player.transform.position + cameraOffset;
        }
    }

    private void UpdateCameraOffset(bool isZooming)
    {
        // 마우스 위치를 월드 좌표로 변환
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = -Camera.main.transform.position.z; // 카메라와의 거리 설정
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePosition);

        // 플레이어 위치에서 마우스 위치까지의 방향 계산
        Vector3 playerPosition = player != null ? player.transform.position : transform.position;
        Vector3 directionToMouse = worldMousePos - playerPosition;

        // 최소 거리 체크
        bool applyCorrection = directionToMouse.magnitude >= minCorrectionDistance;

        // 최대 보정 거리 제한 적용
        if (directionToMouse.magnitude > maxCorrectionDistance)
        {
            directionToMouse = directionToMouse.normalized * maxCorrectionDistance;
        }

        // 줌 상태와 최소 거리에 따라 타겟 오프셋 계산
        Vector3 targetOffset = (isZooming && applyCorrection) ? directionToMouse : Vector3.zero;

        // 부드러운 전환을 위한 보간
        cameraOffset = Vector3.Lerp(cameraOffset, targetOffset, correctionSpeed * Time.deltaTime);
    }

    // 게임 종료 시 생성한 오브젝트 정리
    private void OnDestroy()
    {
        if (cameraTarget != null)
        {
            Destroy(cameraTarget);
        }
    }

    // 씬 뷰에서 시각화
    private void OnDrawGizmosSelected()
    {
        if (enableMouse && player != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(player.transform.position, maxCorrectionDistance);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.transform.position, minCorrectionDistance);
        }
    }
}