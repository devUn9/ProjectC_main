using UnityEngine;
using Unity.Cinemachine;

public class Portal : MonoBehaviour
{
    [SerializeField] private GameObject player; // 플레이어 오브젝트
    [SerializeField] private Transform targetPosition; // 이동할 목표 좌표
    [SerializeField] private CinemachineVirtualCameraBase virtualCamera; // Cinemachine 가상 카메라
    [SerializeField] private BoxCollider2D targetBoundingShape; // 목표 위치의 Bounding Shape 2D
    

    private CinemachineConfiner2D confiner; // Cinemachine Confiner 2D 컴포넌트

    private void Awake()
    {
        // Cinemachine Confiner 2D 컴포넌트 가져오기
        if (virtualCamera != null)
        {
            confiner = virtualCamera.GetComponent<CinemachineConfiner2D>();
            if (confiner == null)
            {
                Debug.LogError("CinemachineConfiner2D 컴포넌트가 가상 카메라에 없습니다!");
            }
            // Follow 초기화
            virtualCamera.Follow = player.transform; // 여기서 Follow 설정
        }
        else
        {
            Debug.LogError("Virtual Camera가 지정되지 않았습니다!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        // 충돌한 오브젝트가 플레이어인지 확인
        if (collision.gameObject == player)
        {
            // 플레이어 위치 이동
            player.transform.position = new Vector3(targetPosition.position.x, targetPosition.position.y + 1, player.transform.position.z);

            // 카메라 경계 업데이트
            if (confiner != null && targetBoundingShape != null)
            {
                confiner.BoundingShape2D = targetBoundingShape; // 새로운 경계 설정
                confiner.InvalidateBoundingShapeCache(); // 캐시 무효화로 경계 재계산
                virtualCamera.ForceCameraPosition(player.transform.position, Quaternion.identity); 
            }
            else
            {
                Debug.LogWarning("Confiner 또는 Target Bounding Shape가 설정되지 않았습니다!");
            }
        }
    }
}