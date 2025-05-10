using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject option;
    [SerializeField] private GameObject mainMenu;
    public void OnClickNewGame()
    {
        Debug.Log("새 게임");
        StartCoroutine(DelayedStartGame(0.3f));
    }

    public void OnClickOption()
    {
        Debug.Log("옵션창");
        mainMenu.SetActive(false);
        option.SetActive(true);
    }

    private IEnumerator DelayedStartGame(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Hook");
    }

    public void OnClickBack()
    {
        Debug.Log("처음으로");
        mainMenu.SetActive(true);
        option.SetActive(false);
        if (OptionsManager.Instance != null)
        {
            OptionsManager.Instance.CloseOption();  // 안전하게 추가 가능
        }
    }

    public void OnClickQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
