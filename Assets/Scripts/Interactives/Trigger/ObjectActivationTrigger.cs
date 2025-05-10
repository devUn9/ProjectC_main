using UnityEngine;

public class ObjectActivationTrigger : MonoBehaviour
{
    [SerializeField] private GameObject targetObject; // 활성화할 오브젝트
    private bool isTriggered = false; // 트리거가 이미 실행되었는지 확인

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어가 트리거 영역에 들어왔는지 확인
        if (other.CompareTag("Player") && targetObject != null && !isTriggered)
        {
            targetObject.SetActive(true);
            isTriggered = true; // 재실행 방지
            gameObject.SetActive(false); // 트리거 오브젝트 비활성화
            Debug.Log($"{targetObject.name}이(가) 활성화되었습니다.", this);
        }
    }

    private void OnValidate()
    {
        // Inspector에서 타겟 오브젝트가 지정되었는지 확인
        if (targetObject == null)
        {
            Debug.LogWarning("ObjectActivationTrigger에 targetObject가 지정되지 않았습니다.", this);
        }
    }
}