using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    public GameObject optionMenu;

    private bool isGamePaused = false;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isGamePaused = true;
        Time.timeScale = 0f;

        if(optionMenu != null)
        {
            optionMenu.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        isGamePaused = false;
        Time.timeScale = 1f;

        if (optionMenu != null)
        {
            optionMenu.SetActive(false);
        }
    }
}
