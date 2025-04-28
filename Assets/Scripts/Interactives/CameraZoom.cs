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

            // 최종 크기 적용
            currentSize = nextSize;
            lens.OrthographicSize = currentSize;
            virtualCamera.Lens = lens;
        }
    }
}