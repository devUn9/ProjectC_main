using UnityEngine;

public class NPCTrigger : MonoBehaviour
{
    [SerializeField] private DialogueManagerTest dialogueManagerTest; // 대화 매니저
    [SerializeField] private Portal3 targetPortal; // 연결된 포털
    [SerializeField] private Player player; // 플레이어 오브젝트
    [Header("설정")]
    [SerializeField] private bool disableAfterTrigger = true; // true면 재사용 불가
    [SerializeField] private int csvFileIndex = 0; // 사용할 CSV 파일의 인덱스

    private bool isPlayerInTrigger = false; // 플레이어가 트리거 안에 있는지
    private bool isTriggered = false; // 트리거가 이미 실행되었는지

 

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어가 트리거 영역에 들어왔는지 확인
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
            Debug.Log("플레이어가 트리거에 들어왔습니다.", this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 플레이어가 트리거 영역에서 나갔는지 확인
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            Debug.Log("플레이어가 트리거에서 나갔습니다.", this);
        }
    }

    private void Update()
    {
        // 플레이어가 트리거 안에 있고, 스페이스바를 누르면 대화와 포털 활성화
        if (isPlayerInTrigger && !isTriggered && Input.GetKeyDown(KeyCode.Space))
        {
            TryActivateDialogueAndPortal();
        }
    }

    private void TryActivateDialogueAndPortal()
    {
        isTriggered = true; // 재실행 방지

        // 플레이어 스킬 설정
        if (player != null)
        {
            var skill = player.GetComponent<Player>(); // PlayerSkill 컴포넌트 가져오기
            if (skill != null)
            {
                player.skill.isSandevistanUsable = true;
                Debug.Log("플레이어 스킬 isSandevistanUsable 설정: true", this);
            }
            else
            {
                Debug.LogWarning("플레이어에 PlayerSkill 컴포넌트가 없습니다.", this);
            }
        }
        else
        {
            Debug.LogWarning("player 오브젝트가 지정되지 않았습니다.", this);
        }

        // 대화 시작
        if (dialogueManagerTest != null)
        {
            dialogueManagerTest.StartDialogue(csvFileIndex);
            Debug.Log("대화 시작: CSV 인덱스 " + csvFileIndex, this);
        }
        else
        {
            Debug.LogWarning("dialogueManagerTest가 지정되지 않았습니다.", this);
        }

        // 포털 활성화
        if (targetPortal != null)
        {
            if (!targetPortal.IsActive)
            {
                targetPortal.ActivatePortal();
                Debug.Log("포털 활성화 호출 완료.", this);
            }
            else
            {
                Debug.LogWarning("targetPortal이 이미 활성화되어 있습니다.", this);
            }
        }
        else
        {
            Debug.LogWarning("targetPortal이 null입니다.", this);
        }

        // 트리거 비활성화
        if (disableAfterTrigger)
        {
            gameObject.SetActive(false);
            Debug.Log("트리거가 비활성화되었습니다.", this);
        }
    }

    private void OnValidate()
    {
        // Inspector에서 필수 참조가 누락되었는지 확인
        if (dialogueManagerTest == null)
        {
            Debug.LogWarning("NPCTrigger에 dialogueManagerTest가 지정되지 않았습니다.", this);
        }
        if (targetPortal == null)
        {
            Debug.LogWarning("NPCTrigger에 targetPortal이 지정되지 않았습니다.", this);
        }
        if (player == null)
        {
            Debug.LogWarning("NPCTrigger에 player 오브젝트가 지정되지 않았습니다.", this);
        }
        if (csvFileIndex < 0)
        {
            Debug.LogWarning("NPCTrigger에서 csvFileIndex는 음수일 수 없습니다.", this);
        }
    }
}