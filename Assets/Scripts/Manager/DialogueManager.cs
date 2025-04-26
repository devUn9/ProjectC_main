using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance { get; private set; }

    [SerializeField] private GameObject txt_DialogueBar;
    [SerializeField] private GameObject txt_DialogueNameBar;

    [SerializeField] private TextMeshProUGUI txt_Dialogue;
    [SerializeField] private TextMeshProUGUI txt_Name;

    Dialogue[] dialogues;

    private bool isDialogue = false;  //대화 중일 경우 true
    private bool isNext = false;      //특정 키 입력 대기

    [Header("텍스트 출력 딜레이")]
    [SerializeField] private float textDelay;

    private int lineCount = 0;        //대화 카운트
    private int conTextCount = 0;     //대사 카운트

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

    private void Update()
    {
        if(isDialogue)
        {
            if(isNext)
            {
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    isNext = false;
                    txt_Dialogue.text = "";
                    if(++conTextCount < dialogues[lineCount].contexts.Length)
                    {
                        StartCoroutine(TypeWriter());
                    }
                    else
                    {
                        conTextCount = 0;
                        if(++lineCount<dialogues.Length)
                        {
                            StartCoroutine(TypeWriter());
                        }
                        else
                        {
                            EndDialogue();
                        }
                    }
                }
            }
        }
    }


    public void ShowDialogue(Dialogue[] _dialogues)
    {
        isDialogue = true;
        txt_Dialogue.text = "";
        txt_Name.text = "";

        dialogues = _dialogues;

        StartCoroutine(TypeWriter());
    }

    private void EndDialogue()
    {
        isDialogue = false;
        conTextCount = 0;
        lineCount = 0;
        dialogues = null;
        isNext = false;
        SettingUI(false);
    }

    IEnumerator TypeWriter()
    {
        SettingUI(true);

        string t_ReplaceText = dialogues[lineCount].contexts[conTextCount];
        t_ReplaceText = t_ReplaceText.Replace("'", ",");

        txt_Name.text = dialogues[lineCount].name;

        for(int i =0; i<t_ReplaceText.Length;i++)
        {
            txt_Dialogue.text += t_ReplaceText[i];
            yield return new WaitForSeconds(textDelay);
        }

        isNext = true;
        yield return null;
    }

    void SettingUI(bool _active)
    {
        txt_DialogueBar.SetActive(_active);
        txt_DialogueNameBar.SetActive(_active);
    }
}
