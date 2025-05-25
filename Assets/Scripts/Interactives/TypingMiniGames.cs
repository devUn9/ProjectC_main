using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class TypingMiniGames : MonoBehaviour
{
    public TextMeshProUGUI targetText;  // 목표 키 시퀀스
    public TextMeshProUGUI inputText;   // 현재 입력된 키
    public Image LeftTime;

    [SerializeField] private Player player;
    [SerializeField] private float timeLimit = 5f; // 제한 시간
    [SerializeField] private float timer = 0f;
    [SerializeField] private int requiredSuccessCount = 3; // 연속 성공 횟수 (Inspector에서 설정 가능)
    private int currentSuccessCount = 0; // 현재 성공 횟수
    private bool isPlaying = false;

    private string currentSequence = "";
    private string playerInput = "";
    private char[] keyPool = new char[] { 'Q', 'W', 'E', 'R' };

    // 투명 문자(Zero-Width Space) - 공백 대신 사용
    private readonly string invisibleChar = "<color=#00000000>O</color>";
    private float originalTimeScale; // 원래 Time.timeScale 저장
    private Animator playerAnimator; // 플레이어의 Animator 컴포넌트
    

    private void Awake()
    {
        playerAnimator = player.GetComponentInChildren<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            StartMiniGame();
        }
    }

    void Update()
    {
        if (!isPlaying) return;

        // Time.unscaledDeltaTime 사용
        timer += Time.unscaledDeltaTime;
        LeftTime.fillAmount = (timeLimit - timer) / timeLimit;
        if (timer > timeLimit)
        {
            StartCoroutine(FailMiniGame());
            return;
        }

        foreach (char key in keyPool)
        {
            if (Input.GetKeyDown(key.ToString().ToLower()))
            {
                playerInput += key;
                SoundManager.instance.PlayESFX(SoundManager.ESfx.SFX_Clicker);
                UpdateInputText();
                CheckInput();
                break;
            }
        }
    }

    void StartMiniGame()
    {
        // 원래 Time.timeScale 저장
        originalTimeScale = Time.timeScale;
        
        playerAnimator.enabled = false;
        player.skill.isLauncherArmUsable = false;
        player.skill.isSandevistanUsable = false;
        Time.timeScale = 0f; // TimeScale을 0으로 설정


        targetText.gameObject.SetActive(true);
        inputText.gameObject.SetActive(true);
        LeftTime.gameObject.SetActive(true);

        currentSuccessCount = 0; // 성공 횟수 초기화
        GenerateNewSequence();
    }

    // 새로운 시퀀스 생성 함수 분리
    void GenerateNewSequence()
    {
        int length = Random.Range(5, 8); // 5~7 글자
        currentSequence = "";
        playerInput = "";
        timer = 1f; // 타이머를 1로 초기화

        // 타이머는 리셋하지 않고 계속 진행
        isPlaying = true;

        for (int i = 0; i < length; i++)
        {
            currentSequence += keyPool[Random.Range(0, keyPool.Length)];
        }

        targetText.text = currentSequence;
        inputText.text = currentSequence; // 처음에는 동일한 텍스트로 시작
        UpdateInputText();
    }

    void UpdateInputText()
    {
        // 입력 텍스트를 만들기 위해 리치 텍스트 형식 사용
        string displayText = "";

        for (int i = 0; i < currentSequence.Length; i++)
        {
            if (i < playerInput.Length)
            {
                // 이미 입력된 문자는 투명 문자로 대체
                displayText += invisibleChar;
            }
            else
            {
                // 아직 입력되지 않은 문자는 원래 문자 그대로 표시
                displayText += currentSequence[i];
            }
        }

        // 리치 텍스트 활성화
        inputText.richText = true;
        inputText.text = displayText;
    }

    void CheckInput()
    {
        // 현재까지 입력한 글자들이 목표 시퀀스와 일치하는지 확인
        for (int i = 0; i < playerInput.Length; i++)
        {
            if (i >= currentSequence.Length || playerInput[i] != currentSequence[i])
            {
                // 틀렸을 때 제한 시간 내에 있으면 새로운 랜덤 시퀀스 생성
                if (timer < timeLimit)
                {
                    // 틝렸다는 피드백 표시 (짧은 시간 동안 빨간색으로 표시)
                    StartCoroutine(ShowIncorrectFeedback());
                }
                else
                {
                    // 제한 시간이 지났으면 게임 종료
                    StartCoroutine(FailMiniGame());
                }
                return;
            }
        }

        // 전체 시퀀스를 성공적으로 입력했는지 확인
        if (playerInput.Length == currentSequence.Length)
        {
            StartCoroutine(SuccessMiniGame());
        }
    }

    // 틀렸을 때 시각적 피드백 제공
    IEnumerator ShowIncorrectFeedback()
    {
        // 텍스트를 잠시 빨간색으로 변경
        Color originalColor = targetText.color;
        targetText.color = Color.red;

        yield return new WaitForSecondsRealtime(0.3f); // WaitForSecondsRealtime 사용

        // 원래 색상으로 복원
        targetText.color = originalColor;

        // 새로운 시퀀스 생성
        GenerateNewSequence();
    }

    IEnumerator SuccessMiniGame()
    {
        isPlaying = false;
        currentSuccessCount++; // 성공 횟수 증가
        targetText.color = Color.green;
        targetText.text = "성공!";
        inputText.text = "";
        yield return new WaitForSecondsRealtime(1f); // WaitForSecondsRealtime 사용

        if (currentSuccessCount >= requiredSuccessCount)
        {
            // 요구된 성공 횟수에 도달하면 미니게임 종료
            targetText.gameObject.SetActive(false);
            inputText.gameObject.SetActive(false);
            LeftTime.gameObject.SetActive(false);

            Time.timeScale = originalTimeScale;
            playerAnimator.enabled = true;
            gameObject.SetActive(false); // 아이템 비활성화
            player.skill.isLauncherArmUsable = true;
            player.skill.isSandevistanUsable = true;
        }
        else
        {
            // 아직 성공 횟수가 부족하면 새로운 시퀀스 생성
            GenerateNewSequence();
        }
    }

    IEnumerator FailMiniGame()
    {
        isPlaying = false;
        targetText.text = "실패!";
        inputText.text = "";
        yield return new WaitForSecondsRealtime(1f); // WaitForSecondsRealtime 사용
        targetText.gameObject.SetActive(false);
        inputText.gameObject.SetActive(false);
        LeftTime.gameObject.SetActive(false);

        // TimeScale 복원
        Time.timeScale = originalTimeScale;
        playerAnimator.enabled = true;
        player.skill.isLauncherArmUsable = true;
        player.skill.isSandevistanUsable = true;

        gameObject.SetActive(false); // 아이템 비활성화


    }
}