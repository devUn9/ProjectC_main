using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueManagerTest dialogueManagerTest; // DialogueManager 참조
    [SerializeField] private Portal3 targetPortal; // 활성화할 특정 Portal2 스크립트 참조
    [SerializeField] private bool disableAfterTrigger = true; // 트리거 비활성화 여부

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // 플레이어 태그 확인
        {
            dialogueManagerTest.StartDialogue();

            if (targetPortal != null)
            {
                targetPortal.ActivatePortal(); // 지정된 포털 활성화
            }
            else
            {
                Debug.LogWarning("Target Portal이 지정되지 않았습니다.");
            }

            if (disableAfterTrigger)
            {
                gameObject.SetActive(false);
                Debug.Log("Trigger deactivated.");
            }
        }
    }
}