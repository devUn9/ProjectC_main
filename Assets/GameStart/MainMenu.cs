using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OnClickNewGame()
    {
        Debug.Log("새 게임");
        StartCoroutine(DelayedStartGame(0.3f));
    }

    private IEnumerator DelayedStartGame(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Hook");
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
