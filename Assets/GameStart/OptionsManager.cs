using UnityEngine;

public class OptionsManager : MonoBehaviour
{
    public static OptionsManager Instance { get; private set; }

    [SerializeField] private GameObject optionsPanel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 중복 방지
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        optionsPanel.SetActive(false); // 처음엔 비활성화
    }

    public void ToggleOption()
    {
        bool isOpen = !optionsPanel.activeSelf;
        optionsPanel.SetActive(isOpen);

        if (isOpen)
        {
            Manager.Instance.PauseGame();
        }
        else
        {
            Manager.Instance.ResumeGame();
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }


    public void CloseOption()
    {
        optionsPanel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
