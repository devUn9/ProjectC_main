using UnityEngine;

public class NPCTrigger : MonoBehaviour
{
    [SerializeField] private DialogueManagerTest dialogueManagerTest; // 대화 매니저
    [SerializeField] private Portal3 targetPortal; // 연결된 포털
    [SerializeField] private Player player; // 플레이어 오브젝트

    [Header("설정")]
    [SerializeField] private bool disableAfterTrigger = true; // true면 재사용 불가
    [SerializeField] private int csvFileIndex = 0; // 사용할 CSV 파일의 인덱스
    [SerializeField] private SkillSelection skillsToEnable; // 활성화할 스킬 선택

    // 플래그 기반 Enum 정의
    [System.Flags]
    private enum SkillSelection
    {
        None = 0,
        Sandevistan = 1 << 0, // 1
        LauncherArm = 1 << 1, // 2
        Graviton = 1 << 2     // 4
    }

    private bool isPlayerInTrigger = false; // 플레이어가 트리거 안에 있는지
    private bool isTriggered = false; // 트리거가 이미 실행되었는지

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
        }
    }

    private void Update()
    {
        if (isPlayerInTrigger && !isTriggered && Input.GetKeyDown(KeyCode.Space))
        {
            TryActivateDialogueAndPortal();
        }
    }

    private void TryActivateDialogueAndPortal()
    {
        isTriggered = true;

        // 플레이어 스킬 설정
        if (player != null)
        {
            var skill = player.GetComponent<Player>();
            if (skill != null)
            {
                // 선택된 스킬 활성화
                if (skillsToEnable.HasFlag(SkillSelection.Sandevistan))
                {
                    player.skill.isSandevistanUsable = true;
                }
                if (skillsToEnable.HasFlag(SkillSelection.LauncherArm))
                {
                    player.skill.isLauncherArmUsable = true;
                }
                if (skillsToEnable.HasFlag(SkillSelection.Graviton))
                {
                    player.skill.isGravitonUsable = true;
                }
            }
        }

        // 대화 시작
        if (dialogueManagerTest != null)
        {
            dialogueManagerTest.StartDialogue(csvFileIndex);
        }

        // 포털 활성화
        if (targetPortal != null)
        {
            if (!targetPortal.IsActive)
            {
                targetPortal.ActivatePortal();
            }
        }

        // 트리거 비활성화
        if (disableAfterTrigger)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnValidate()
    {
        // 디버그 로그 제거
    }
}