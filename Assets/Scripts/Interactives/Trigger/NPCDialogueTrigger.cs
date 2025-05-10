    using UnityEngine;

public class NPCDialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueManagerTest dialogueManagerTest; // 대화 매니저
    [Header("설정")]
    [SerializeField] private bool disableAfterTrigger = true; // true면 재사용 불가
    [SerializeField] private int csvFileIndex = 0; // 사용할 CSV 파일의 인덱스

    private bool isPlayerInTrigger = false; // 플레이어가 트리거 안에 있는지 확인
    private bool isTriggered = false; // 트리거가 이미 실행되었는지 확인

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어가 트리거 영역에 들어왔는지 확인
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 플레이어가 트리거 영역에서 나갔는지 확인
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
        }
    }

    private void Update()
    {
        // 플레이어가 트리거 안에 있고, 스페이스바를 누르면 대화 시작
        if (isPlayerInTrigger && !isTriggered && Input.GetKeyDown(KeyCode.Space))
        {
            if (dialogueManagerTest != null)
            {
                dialogueManagerTest.StartDialogue(csvFileIndex); // 선택된 CSV 인덱스 전달
                isTriggered = true; // 재실행 방지

                if (disableAfterTrigger)
                {
                    gameObject.SetActive(false);
                    Debug.Log("트리거가 비활성화되었습니다.", this);
                }
            }
        }
    }

    private void OnValidate()
    {
        // Inspector에서 필수 참조가 누락되었는지 확인
        if (dialogueManagerTest == null)
        {
            Debug.LogWarning("NPCDialogueTrigger에 dialogueManagerTest가 지정되지 않았습니다.", this);
        }
        if (csvFileIndex < 0)
        {
            Debug.LogWarning("NPCDialogueTrigger에서 csvFileIndex는 음수일 수 없습니다.", this);
        }
    }
}