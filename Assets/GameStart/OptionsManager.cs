using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class OptionsManager : MonoBehaviour
{
    public static OptionsManager Instance { get; private set; }

    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject explanationKey;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 중복 방지
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (optionsPanel == null)
        {
            Debug.LogError("OptionsPanel is not assigned in " + gameObject.name, this);
        }
        else
        {
            optionsPanel.SetActive(false); // 처음엔 비활성화
            explanationKey.SetActive(false);
        }
    }

    private void Update()
    {
        // 메인 메뉴 씬에서는 ESC 입력 무시
        if (SceneManager.GetActiveScene().name == "GameStart") return;

        // ESC 키로 옵션 패널 토글
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleOption();
        }
    }

    public void ToggleOption()
    {
        if (optionsPanel == null)
        {
            Debug.LogError("OptionsPanel is not assigned", this);
            return;
        }

        bool isOpen = !optionsPanel.activeSelf;
        optionsPanel.SetActive(isOpen);
        explanationKey.SetActive(isOpen);

        if (isOpen)
        {
            if (Manager.Instance != null)
            {
                Manager.Instance.PauseGame();
            }
            else
            {
                Debug.LogWarning("Manager.Instance is null, cannot pause game", this);
            }
        }
        else
        {
            if (Manager.Instance != null)
            {
                Manager.Instance.ResumeGame();
            }
            else
            {
                Debug.LogWarning("Manager.Instance is null, cannot resume game", this);
            }
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void CloseOption()
    {
        if (optionsPanel == null)
        {
            Debug.LogError("OptionsPanel is not assigned", this);
            return;
        }

        optionsPanel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void DisableExplanationButton()
    {
        explanationKey.GetComponent<UI_Button>().DisableButton();
    }
}