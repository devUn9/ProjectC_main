using UnityEngine;

public class PlayerCheckPortalTrigger : MonoBehaviour
{
    [SerializeField] private Portal3 targetPortal; // 연결된 포털
    private bool isTriggered = false; // 트리거가 이미 실행되었는지 확인

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어가 트리거 영역에 들어왔는지 확인
        if (other.CompareTag("Player") && targetPortal != null && !targetPortal.IsActive && !isTriggered)
        {
            targetPortal.ActivatePortal();
            isTriggered = true; // 재실행 방지
            gameObject.SetActive(false); // 트리거 오브젝트 비활성화
            Debug.Log("트리거가 비활성화되었습니다.", this);
        }
    }

    private void OnValidate()
    {
        // Inspector에서 포털이 지정되었는지 확인
        if (targetPortal == null)
        {
            Debug.LogWarning("PlayerCheckPortalTrigger에 targetPortal이 지정되지 않았습니다.", this);
        }
    }
}