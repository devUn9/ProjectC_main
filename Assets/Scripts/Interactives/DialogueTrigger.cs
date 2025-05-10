using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueManagerTest dialogueManagerTest;
    //[SerializeField] private Portal3 targetPortal;
    [Header("사용시 삭제?")]
    [SerializeField] private bool disableAfterTrigger = true; // true면 재사용 불가
    [SerializeField] private int csvFileIndex = 0; // 사용할 CSV 파일의 인덱스

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            dialogueManagerTest.StartDialogue(csvFileIndex); // 선택된 CSV 인덱스 전달

            //if (targetPortal != null)
            //{
            //    targetPortal.ActivatePortal();
            //}
            //else
            //{
            //    Debug.LogWarning("Target Portal이 지정되지 않았습니다.");
            //}

            if (disableAfterTrigger)
            {
                gameObject.SetActive(false);
                Debug.Log("트리거가 비활성화되었습니다.");
            }
        }
    }

    void OnValidate()
    {
        if (dialogueManagerTest == null) Debug.LogWarning("DialogueTrigger에 dialogueManagerTest가 지정되지 않았습니다.", this);
        if (csvFileIndex < 0) Debug.LogWarning("DialogueTrigger에서 csvFileIndex는 음수일 수 없습니다.", this);
    }
}