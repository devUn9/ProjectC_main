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

    private float currentSize;
    private CinemachineConfiner2D confiner;
    private float defaultPlayerSpeed; // 원래 Player의 moveSpeed 저장

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

        // CinemachineConfiner2D 컴포넌트 가져오기
        confiner = virtualCamera.GetComponent<CinemachineConfiner2D>();
        if (confiner == null)
        {
            Debug.LogWarning("CinemachineConfiner2D 컴포넌트가 없습니다. 경계 검사가 비활성화됩니다.");
        }

        // 초기 렌즈 설정
        if (virtualCamera != null && virtualCamera.Lens.Orthographic)
        {
            currentSize = defaultSize;
            var lens = virtualCamera.Lens;
            lens.OrthographicSize = currentSize;
            virtualCamera.Lens = lens;
        }
    }

    private void Update()
    {
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
            }
            else
            {
                // 키를 떼면 원래 크기와 속도로 복귀
                targetSize = defaultSize;
                if (player != null)
                {
                    player.moveSpeed = defaultPlayerSpeed; // 원래 moveSpeed로 복구
                }
            }

            // 현재 크기와 목표 크기 사이의 다음 단계 계산
            float nextSize = Mathf.Lerp(currentSize, targetSize, zoomSpeed * Time.deltaTime);

            // 현재 -> 목표로 가는 중인지, 목표 -> 현재로 돌아오는 중인지 확인
            bool isZoomingOut = nextSize > currentSize;

            // 줌 아웃하는 경우에만 바운딩 체크 수행
            if (isZoomingOut && confiner != null && confiner.enabled)
            {
                // 새 크기로 업데이트하기 전에 바운딩 검사
                if (!WillStayWithinBounds(nextSize))
                {
                    // 충돌이 발생하면 크기를 변경하지 않음
                    nextSize = currentSize;
                }
            }

            // 최종 크기 적용
            currentSize = nextSize;
            lens.OrthographicSize = currentSize;
            virtualCamera.Lens = lens;
        }
    }

    private bool WillStayWithinBounds(float orthographicSize)
    {
        // Confiner가 없거나 바운딩 셰이프가 없으면 true 반환
        if (confiner == null || confiner.BoundingShape2D == null)
            return true;

        // Confiner에서 바운딩 셰이프 가져오기
        Collider2D boundingShape = confiner.BoundingShape2D;

        // 카메라의 화면 크기 계산
        float aspectRatio = (float)Screen.width / Screen.height;
        float cameraWidth = orthographicSize * 2f * aspectRatio;
        float cameraHeight = orthographicSize * 2f;

        // 카메라 화면의 네 모서리가 모두 바운딩 셰이프 내에 있는지 확인
        Vector2 cameraPos = virtualCamera.transform.position;
        Vector2[] corners = new Vector2[4];
        corners[0] = new Vector2(cameraPos.x - cameraWidth / 2, cameraPos.y - cameraHeight / 2); // 좌하단
        corners[1] = new Vector2(cameraPos.x + cameraWidth / 2, cameraPos.y - cameraHeight / 2); // 우하단
        corners[2] = new Vector2(cameraPos.x - cameraWidth / 2, cameraPos.y + cameraHeight / 2); // 좌상단
        corners[3] = new Vector2(cameraPos.x + cameraWidth / 2, cameraPos.y + cameraHeight / 2); // 우상단

        // 모든 모서리가 바운딩 셰이프 내에 있는지 확인
        foreach (Vector2 corner in corners)
        {
            if (!boundingShape.OverlapPoint(corner))
            {
                return false; // 하나라도 바운드 밖이면 false 반환
            }
        }
        return true; // 모든 코너가 바운드 안에 있음
    }
}