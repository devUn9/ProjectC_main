using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class TypingMiniGame : MonoBehaviour
{
    public TextMeshProUGUI targetText;  // 목표 키 시퀀스
    public TextMeshProUGUI inputText;   // 현재 입력된 키
    public Image LeftTime;

    [SerializeField] private Player player;

    private string currentSequence = "";
    private string playerInput = "";

    private float timeLimit = 5f; // 제한 시간
    [SerializeField] private float timer = 0f;
    private bool isPlaying = false;

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
        Time.timeScale = 0f; // TimeScale을 0으로 설정
        playerAnimator.enabled = false;


        targetText.gameObject.SetActive(true);
        inputText.gameObject.SetActive(true);
        LeftTime.gameObject.SetActive(true);

        GenerateNewSequence();
    }

    // 새로운 시퀀스 생성 함수 분리
    void GenerateNewSequence()
    {
        int length = Random.Range(5, 8); // 5~7 글자
        currentSequence = "";
        playerInput = "";

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
                    // 틀렸다는 피드백 표시 (짧은 시간 동안 빨간색으로 표시)
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
        targetText.color = Color.green;
        targetText.text = "성공!";
        inputText.text = "";
        yield return new WaitForSecondsRealtime(1f); // WaitForSecondsRealtime 사용
        targetText.gameObject.SetActive(false);
        inputText.gameObject.SetActive(false);
        LeftTime.gameObject.SetActive(false);

        // TimeScale 복원
        Time.timeScale = originalTimeScale;
        playerAnimator.enabled = true;

        gameObject.SetActive(false); // 아이템 비활성화
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

        gameObject.SetActive(false); // 아이템 비활성화
    }
}