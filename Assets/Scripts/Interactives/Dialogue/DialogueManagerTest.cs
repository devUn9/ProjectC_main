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
    [SerializeField] public TextAsset[] csvFiles; // 여러 CSV 파일을 지원하기 위해 배열로 변경
    [SerializeField] private Image blinkImageUI;
    [SerializeField] private Player player; // 플레이어 오브젝트 (Inspector에서 지정, Player 타입)
    [SerializeField] private Image dialogueBackground; // 대화 UI 배경 이미지 (Inspector에서 지정)
    [SerializeField] private TextMeshProUGUI pastText; // "과거" 텍스트 UI (Inspector에서 지정)
    [SerializeField] private Image happyEndingImage; // Happy Ending UI 이미지
    [SerializeField] private Image sadEndingImage;   // Sad Ending UI 이미지
    [SerializeField] private EndingCredit endingCredit; // EndingCredit 컴포넌트 참조

    [SerializeField] private float typingSpeed = 0.1f; // 글자당 표시 간격 (초)
    private string fullText = "과거 . . ."; // 표시할 전체 텍스트

    // 디졸브 효과 관련 변수
    private Material illustrationMaterial2;
    private bool isDissolving = false;

    private List<DialogueTest> dialogues = new List<DialogueTest>();
    private List<Sprite> illustrations = new List<Sprite>();
    private int currentDialogueIndex = 0;
    private int currentCsvIndex = -1; // 현재 CSV 인덱스 추적
    public bool isDialogueActive = false;
    private float lastInputTime = 0f;
    private Color originalBackgroundColor; // 원래 배경 색상 저장

    [Header("반응 시작 시간 / 깜빡임 시작 시간")]
    [SerializeField] private float inputCooldown = 1f;
    [SerializeField] private float noInputThreshold = 5f;
    [SerializeField] private float backgroundTransitionDuration = 1f; // 배경 전환 시간 (Inspector에서 조정 가능)
    [SerializeField] private float dissolveSpeed = 1f; // 디졸브 효과 속도 (Inspector에서 조정 가능)
    private bool isBlinking = false;
    private Animator playerAnimator; // 플레이어의 Animator 컴포넌트

    void Start()
    {
        try
        {
            LoadIllustrations();
            dialogueUI.SetActive(false);
            if (blinkImageUI != null) blinkImageUI.gameObject.SetActive(false);

            // Happy Ending과 Sad Ending 이미지 초기 비활성화
            if (happyEndingImage != null) happyEndingImage.gameObject.SetActive(false);
            else Debug.LogWarning("DialogueManagerTest에 happyEndingImage가 지정되지 않았습니다.", this);

            if (sadEndingImage != null) sadEndingImage.gameObject.SetActive(false);
            else Debug.LogWarning("DialogueManagerTest에 sadEndingImage가 지정되지 않았습니다.", this);

            // 대화 UI 배경 색상 저장
            if (dialogueBackground != null)
            {
                originalBackgroundColor = dialogueBackground.color;
            }
            else
            {
                Debug.LogWarning("DialogueManagerTest에 dialogueBackground가 지정되지 않았습니다.", this);
            }

            // "과거" 텍스트 초기 비활성화
            if (pastText != null)
            {
                pastText.enabled = false;
            }
            else
            {
                Debug.LogWarning("DialogueManagerTest에 pastText가 지정되지 않았습니다.", this);
            }

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

            // 캐릭터 일러스트레이션2의 머티리얼 설정
            if (characterIllustration2 != null)
            {
                // 일러스트레이션의 머티리얼 복제하여 사용
                illustrationMaterial2 = new Material(characterIllustration2.material);
                characterIllustration2.material = illustrationMaterial2;

                // 초기 _SplitValue 설정
                illustrationMaterial2.SetFloat("_SplitValue", 1f);
            }
            else
            {
                Debug.LogWarning("characterIllustration2가 지정되지 않았습니다.", this);
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

            if (UnityEngine.Input.GetKeyDown(KeyCode.Space) && Time.unscaledTime - lastInputTime >= inputCooldown)
            {
                lastInputTime = Time.unscaledTime;
                StopBlinking();

                // 마지막 대화이고, 특정 일러스트(10, 11, 12)를 사용 중인 경우 디졸브 효과 적용
                if (currentDialogueIndex == dialogues.Count - 1)
                {
                    DialogueTest current = dialogues[currentDialogueIndex];
                    if (current.illustrationIndex2 == 10 || current.illustrationIndex2 == 11 || current.illustrationIndex2 == 12)
                    {
                        if (!isDissolving)
                        {
                            StartCoroutine(DissolveEffect());
                            return; // 디졸브 효과가 진행되는 동안은 대화 넘기기를 중단
                        }
                    }
                }

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

                // 디졸브 초기화 (새 대화마다)
                if (illustrationMaterial2 != null)
                {
                    illustrationMaterial2.SetFloat("_SplitValue", 1f);
                }
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

    IEnumerator PastText()
    {
        pastText.text = ""; // 텍스트 초기화

        // 문자열을 한 글자씩 처리
        foreach (char letter in fullText)
        {
            pastText.text += letter; // 글자 추가
            yield return new WaitForSecondsRealtime(typingSpeed); // 지정된 시간 대기
        }
    }

    public void StartDialogue(int csvIndex)
    {
        try
        {
            currentCsvIndex = csvIndex; // 현재 CSV 인덱스 저장
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

            // csvIndex가 18인 경우 배경을 검은색으로 천천히 전환하고 "과거" 텍스트 표시
            if (csvIndex == 18)
            {
                if (dialogueBackground != null)
                {
                    StartCoroutine(TransitionBackgroundColor(Color.black));
                }
                if (pastText != null)
                {
                    StartCoroutine(PastText());
                    pastText.enabled = true;
                }
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

            switch (currentCsvIndex)
            {
                case 16:
                case 17:
                    StartDialogue(18);
                    return;

                case 18:
                    if (dialogueBackground != null)
                    {
                        StartCoroutine(TransitionBackgroundColor(originalBackgroundColor));
                    }
                    if (pastText != null)
                    {
                        pastText.enabled = false;
                    }
                    break;

                case 0:
                    SoundManager.instance.PlayBGM(SoundManager.EBgm.Bgm_StageBattle);
                    break;

                case 6:
                    SoundManager.instance.PlayBGM(SoundManager.EBgm.Bgm_City);
                    break;

                case 10:
                    SoundManager.instance.PlayBGM(SoundManager.EBgm.Bgm_Enterprise);
                    break;

                case 21:
                case 22:
                    SoundManager.instance.PlayBGM(SoundManager.EBgm.Bgm_BossBattle);
                    break;

                case 23:
                case 24:
                    SoundManager.instance.PlayBGM(SoundManager.EBgm.Bgm_EndingCredit);
                    endingCredit.StartScrolling();

                    if (player != null && player.skill != null)
                    {
                        if (!player.skill.isGravitonUsable)
                        {
                            happyEndingImage.gameObject.SetActive(true);
                        }
                        else
                        {
                            sadEndingImage.gameObject.SetActive(true);
                        }
                    }
                    break;
            }

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

    // 디졸브 효과 코루틴
    IEnumerator DissolveEffect()
    {
        if (illustrationMaterial2 == null)
        {
            Debug.LogWarning("일러스트레이션 머티리얼이 없습니다.");
            NextDialogue();
            yield break;
        }

        Debug.Log("디졸브 효과 시작");
        isDissolving = true;

        float duration = dissolveSpeed;
        float timer = 0f;

        // 1초 동안 1 → -0.1로 선형 감소
        while (timer < duration)
        {
            float t = timer / duration; // 0 → 1
            float value = Mathf.Lerp(1f, -0.1f, t);
            illustrationMaterial2.SetFloat("_SplitValue", value);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        illustrationMaterial2.SetFloat("_SplitValue", -0.1f);

        //// 잠시 대기 후 효과 종료
        //yield return new WaitForSecondsRealtime(1f);

        // 다음 대화로 이동
        isDissolving = false;
        Debug.Log("디졸브 효과 종료");
        NextDialogue();
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

    IEnumerator TransitionBackgroundColor(Color targetColor)
    {
        if (dialogueBackground == null) yield break;

        Color startColor = dialogueBackground.color;
        float elapsedTime = 0f;

        while (elapsedTime < backgroundTransitionDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = elapsedTime / backgroundTransitionDuration;
            dialogueBackground.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        dialogueBackground.color = targetColor; // 최종 색상 설정
    }
}