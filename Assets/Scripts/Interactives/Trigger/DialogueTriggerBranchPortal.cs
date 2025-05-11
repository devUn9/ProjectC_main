using UnityEngine;

public class DialogueTriggerBranchPortal : MonoBehaviour
{
    // 포탈이 활성화 상태일 때만 분기 대사 출력
    [SerializeField] private DialogueManagerTest dialogueManagerTest;
    [SerializeField] private bool disableAfterTrigger = true; // true면 재사용 불가
    [SerializeField] private int csvFileIndexNormal = 0; // Graviton 비활성 시 사용할 CSV 파일 인덱스
    [SerializeField] private int csvFileIndexGraviton = 0; // Graviton 활성 시 사용할 CSV 파일 인덱스
    [SerializeField] private Player player; // Player 참조
    [SerializeField] private Portal3 targetPortal; // 포털 참조

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && targetPortal != null && targetPortal.IsActive)
        {
            // player.skill.isGravitonUsable에 따라 CSV 인덱스 선택
            int selectedCsvIndex = player.skill.isGravitonUsable ? csvFileIndexGraviton : csvFileIndexNormal;
            dialogueManagerTest.StartDialogue(selectedCsvIndex); // 선택된 CSV 인덱스 전달

            if (disableAfterTrigger)
            {
                gameObject.SetActive(false);
                Debug.Log("트리거가 비활성화되었습니다.");
            }
        }
    }

    void OnValidate()
    {
        if (dialogueManagerTest == null) Debug.LogWarning("DialogueTriggerBranch에 dialogueManagerTest가 지정되지 않았습니다.", this);
        if (player == null) Debug.LogWarning("DialogueTriggerBranch에 player가 지정되지 않았습니다.", this);
        if (targetPortal == null) Debug.LogWarning("DialogueTriggerBranch에 targetPortal이 지정되지 않았습니다.", this);
        if (csvFileIndexNormal < 0) Debug.LogWarning("DialogueTriggerBranch에서 csvFileIndexNormal은 음수일 수 없습니다.", this);
        if (csvFileIndexGraviton < 0) Debug.LogWarning("DialogueTriggerBranch에서 csvFileIndexGraviton은 음수일 수 없습니다.", this);
    }
}