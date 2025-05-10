using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class DialogueManagerTest : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI characterNameText2;
    [SerializeField] private Image characterIllustration;
    [SerializeField] private Image characterIllustration2;
    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private TextAsset[] csvFiles; // 여러 CSV 파일을 지원하기 위해 배열로 변경
    [SerializeField] private Image blinkImageUI;
    [SerializeField] private GameObject player; // 플레이어 오브젝트 (Inspector에서 지정)

    private List<DialogueTest> dialogues = new List<DialogueTest>();
    private List<Sprite> illustrations = new List<Sprite>();
    private int currentDialogueIndex = 0;
    public bool isDialogueActive = false;
    private float lastInputTime = 0f;

    [Header("반응 시작 시간 / 깜빡임 시작 시간")]
    [SerializeField] private float inputCooldown = 1f;
    [SerializeField] private float noInputThreshold = 5f;
    private bool isBlinking = false;
    private Animator playerAnimator; // 플레이어의 Animator 컴포넌트

    void Start()
    {
        try
        {
            LoadIllustrations();
            dialogueUI.SetActive(false);
            if (blinkImageUI != null) blinkImageUI.gameObject.SetActive(false);

            // 플레이어의 Animator 컴포넌트 가져오기
            if (player != null)
            {
                playerAnimator = player.GetComponentInChildren<Animator>();
                if (playerAnimator == null)
                {
                    Debug.LogWarning("플레이어에 Animator 컴포넌트가 없습니다.", player);
                }
            }
            else
            {
                Debug.LogWarning("DialogueManagerTest에 플레이어 오브젝트가 지정되지 않았습니다.", this);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Start 오류: {ex.Message}");
        }
    }

    void Update()
    {
        if (isDialogueActive)
        {
            // Time.unscaledTime을 사용하여 Time.timeScale = 0에서도 작동
            if (Time.unscaledTime - lastInputTime >= noInputThreshold && !isBlinking)
            {
                StartBlinking();
            }

            if (Input.GetKeyDown(KeyCode.Space) && Time.unscaledTime - lastInputTime >= inputCooldown)
            {
                lastInputTime = Time.unscaledTime;
                StopBlinking();
                NextDialogue();
            }
        }
    }

    void LoadIllustrations()
    {
        try
        {
            Sprite[] loadedSprites = Resources.LoadAll<Sprite>("Illustrations");
            if (loadedSprites == null || loadedSprites.Length == 0)
            {
                Debug.LogWarning("Illustrations 폴더에서 스프라이트를 로드하지 못했습니다.");
                return;
            }
            illustrations.AddRange(loadedSprites);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"LoadIllustrations 오류: {ex.Message}");
        }
    }

    void LoadCSV(int csvIndex)
    {
        try
        {
            if (csvFiles == null || csvIndex < 0 || csvIndex >= csvFiles.Length || csvFiles[csvIndex] == null)
            {
                Debug.LogWarning($"유효하지 않은 CSV 인덱스 {csvIndex} 또는 CSV 파일이 지정되지 않았습니다.");
                return;
            }

            dialogues.Clear(); // 기존 대화 목록 초기화
            string[] lines = csvFiles[csvIndex].text.Split('\n');

            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                string[] fields = SplitCSVLine(line);
                if (fields.Length < 5)
                {
                    continue;
                }

                try
                {
                    DialogueTest dialogue = new DialogueTest
                    {
                        id = int.Parse(fields[0]),
                        character = fields[1],
                        dialogue = fields[2],
                        illustrationIndex = int.Parse(fields[3]),
                        illustrationIndex2 = int.Parse(fields[4])
                    };
                    dialogues.Add(dialogue);
                }
                catch (System.FormatException ex)
                {
                    Debug.LogWarning($"CSV 파싱 오류 (줄 {i}): {ex.Message}");
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"LoadCSV 오류: {ex.Message}");
        }
    }

    string[] SplitCSVLine(string line)
    {
        try
        {
            List<string> result = new List<string>();
            bool inQuotes = false;
            string field = "";

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(field);
                    field = "";
                }
                else
                {
                    field += c;
                }
            }
            result.Add(field);
            return result.ToArray();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"SplitCSVLine 오류: {ex.Message}");
            return new string[0];
        }
    }

    void UpdateDialogue()
    {
        try
        {
            if (currentDialogueIndex >= dialogues.Count)
            {
                EndDialogue();
                return;
            }

            if (dialogueText == null || characterNameText == null || characterNameText2 == null || characterIllustration == null || characterIllustration2 == null)
            {
                Debug.LogWarning("UI 요소가 지정되지 않았습니다.");
                return;
            }

            DialogueTest current = dialogues[currentDialogueIndex];
            dialogueText.text = current.dialogue;
            characterNameText.text = current.character;
            characterNameText2.text = current.character;

            // 캐릭터별 이름 색상 설정
            if (current.character == "데이비드")
            {
                characterNameText.color = new Color(0.1f, 0.9f, 0.9f); // 청록
            }
            else if (current.character == "루시")
            {
                characterNameText2.color = new Color(0.9f, 0.33f, 1f); // 핑크
            }
            else if (current.character == "리퍼닥")
            {
                characterNameText2.color = new Color(0.33f, 0.95f, 0.15f); // 초록
            }
            else if (current.character == "아담 스매셔")
            {
                characterNameText2.color = new Color(1f, 0f, 0f); // 빨강
            }
            else
            {
                characterNameText.color = Color.white;
                characterNameText2.color = Color.white;
            }

            // 일러스트 설정
            if (current.illustrationIndex >= 0 && current.illustrationIndex < illustrations.Count)
            {
                characterIllustration.sprite = illustrations[current.illustrationIndex];
                characterIllustration.enabled = true;
            }
            else
            {
                characterIllustration.sprite = null;
                characterIllustration.enabled = false;
            }

            if (current.illustrationIndex2 >= 0 && current.illustrationIndex2 < illustrations.Count)
            {
                characterIllustration2.sprite = illustrations[current.illustrationIndex2];
                characterIllustration2.enabled = true;
            }
            else
            {
                characterIllustration2.sprite = null;
                characterIllustration2.enabled = false;
            }

            // 대화 ID에 따른 UI 조정
            if (current.id == 1)
            {
                characterIllustration.color = Color.white;
                characterIllustration2.color = new Color(0.4f, 0.4f, 0.4f, 1f);
                characterNameText.enabled = true;
                characterNameText2.enabled = false;
            }
            else if (current.id == 2)
            {
                characterIllustration.color = new Color(0.4f, 0.4f, 0.4f, 1f);
                characterIllustration2.color = Color.white;
                characterNameText.enabled = false;
                characterNameText2.enabled = true;
            }
            else
            {
                characterIllustration.color = Color.white;
                characterIllustration2.color = Color.white;
                characterNameText.enabled = true;
                characterNameText2.enabled = true;
            }

            dialogueText.enabled = true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"UpdateDialogue 오류: {ex.Message}");
            EndDialogue();
        }
    }

    public void StartDialogue(int csvIndex)
    {
        try
        {
            LoadCSV(csvIndex); // 지정된 CSV 파일 로드
            if (dialogues.Count == 0)
            {
                Debug.LogWarning("CSV에서 대화를 로드하지 못했습니다.");
                return;
            }

            if (dialogueUI == null)
            {
                Debug.LogWarning("dialogueUI가 지정되지 않았습니다.");
                return;
            }

            // 게임 일시정지 및 플레이어 애니메이션 비활성화
            Time.timeScale = 0f;
            if (playerAnimator != null)
            {
                playerAnimator.enabled = false;
            }

            currentDialogueIndex = 0;
            isDialogueActive = true;
            dialogueUI.SetActive(true);
            lastInputTime = Time.unscaledTime; // unscaledTime 사용

            Canvas canvas = dialogueUI.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.enabled = true;
            }

            UpdateDialogue();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"StartDialogue 오류: {ex.Message}");
        }
    }

    public void NextDialogue()
    {
        try
        {
            if (currentDialogueIndex < dialogues.Count - 1)
            {
                currentDialogueIndex++;
                UpdateDialogue();
            }
            else
            {
                EndDialogue();
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"NextDialogue 오류: {ex.Message}");
        }
    }

    void EndDialogue()
    {
        try
        {
            isDialogueActive = false;
            dialogueUI.SetActive(false);
            StopBlinking();

            // 게임 재개 및 플레이어 애니메이션 활성화
            Time.timeScale = 1f;
            if (playerAnimator != null)
            {
                playerAnimator.enabled = true;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"EndDialogue 오류: {ex.Message}");
        }
    }

    void StartBlinking()
    {
        if (blinkImageUI == null) return;
        isBlinking = true;
        blinkImageUI.gameObject.SetActive(true);
        StartCoroutine(BlinkImage());
    }

    void StopBlinking()
    {
        if (blinkImageUI == null) return;
        isBlinking = false;
        StopCoroutine(BlinkImage());
        blinkImageUI.gameObject.SetActive(false);
    }

    IEnumerator BlinkImage()
    {
        while (isBlinking)
        {
            blinkImageUI.enabled = !blinkImageUI.enabled;
            yield return new WaitForSecondsRealtime(0.5f); // WaitForSecondsRealtime 사용
        }
    }

    void OnValidate()
    {
        if (dialogueUI == null) Debug.LogWarning("DialogueManagerTest에 dialogueUI가 지정되지 않았습니다.", this);
        if (dialogueText == null) Debug.LogWarning("DialogueManagerTest에 dialogueText가 지정되지 않았습니다.", this);
        if (characterNameText == null) Debug.LogWarning("DialogueManagerTest에 characterNameText가 지정되지 않았습니다.", this);
        if (characterNameText2 == null) Debug.LogWarning("DialogueManagerTest에 characterNameText2가 지정되지 않았습니다.", this);
        if (characterIllustration == null) Debug.LogWarning("DialogueManagerTest에 characterIllustration이 지정되지 않았습니다.", this);
        if (characterIllustration2 == null) Debug.LogWarning("DialogueManagerTest에 characterIllustration2가 지정되지 않았습니다.", this);
        if (csvFiles == null || csvFiles.Length == 0) Debug.LogWarning("DialogueManagerTest에 csvFiles 배열이 비어 있거나 지정되지 않았습니다.", this);
        if (blinkImageUI == null) Debug.LogWarning("DialogueManagerTest에 blinkImageUI가 지정되지 않았습니다.", this);
        if (player == null) Debug.LogWarning("DialogueManagerTest에 player 오브젝트가 지정되지 않았습니다.", this);
    }
}