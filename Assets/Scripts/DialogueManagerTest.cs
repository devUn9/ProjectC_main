using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class DialogueManagerTest : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogueText; // 대사 텍스트 UI
    [SerializeField] private TextMeshProUGUI characterNameText; // 캐릭터 이름 UI (Text1)
    [SerializeField] private TextMeshProUGUI characterNameText2; // 캐릭터 이름 UI (Text2)
    [SerializeField] private Image characterIllustration; // 캐릭터 일러스트 UI (Image1)
    [SerializeField] private Image characterIllustration2; // 캐릭터 일러스트 UI2 (Image2)
    [SerializeField] private GameObject dialogueUI; // 대화 UI 패널
    [SerializeField] private TextAsset csvFile; // 인스펙터에서 설정할 CSV 파일

    private List<DialogueTest> dialogues = new List<DialogueTest>();
    private List<Sprite> illustrations = new List<Sprite>();
    private int currentDialogueIndex = 0;
    private bool isDialogueActive = false;

    void Start()
    {
        try
        {
            LoadIllustrations();
            LoadCSV();
            dialogueUI.SetActive(false);
        }
        catch (System.Exception)
        {
        }
    }

    void Update()
    {
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            NextDialogue();
        }
    }

    void LoadIllustrations()
    {
        try
        {
            Sprite[] loadedSprites = Resources.LoadAll<Sprite>("Illustrations");
            if (loadedSprites == null || loadedSprites.Length == 0)
            {
                return;
            }
            illustrations.AddRange(loadedSprites);
        }
        catch (System.Exception)
        {
        }
    }

    void LoadCSV()
    {
        try
        {
            if (csvFile == null)
            {
                return;
            }

            string[] lines = csvFile.text.Split('\n');

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
                catch (System.FormatException)
                {
                }
            }
        }
        catch (System.Exception)
        {
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
        catch (System.Exception)
        {
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
                return;
            }

            DialogueTest current = dialogues[currentDialogueIndex];
            dialogueText.text = current.dialogue;
            characterNameText.text = current.character;
            characterNameText2.text = current.character;

            // 폰트 색상 설정
            if (current.character == "아담스매셔")
            {
                
                characterNameText2.color = new Color(1f, 0f, 0f); // 빨간색
            }
            else if (current.character == "데이비드")
            {
                characterNameText.color = new Color(0.33f, 0.95f, 0.15f); // 초록색
            }
            else if (current.character == "루시")
            {
                characterNameText2.color = new Color(0.9f, 0.33f, 1f); // 핑크색
            }
            else
            {
                characterNameText.color = Color.white;
                characterNameText2.color = Color.white;
            }

            // Image1 설정
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

            // Image2 설정
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

            // 색상 조정
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
        catch (System.Exception)
        {
            EndDialogue();
        }
    }

    public void StartDialogue()
    {
        try
        {
            if (dialogues.Count == 0)
            {
                return;
            }

            if (dialogueUI == null)
            {
                return;
            }

            currentDialogueIndex = 0;
            isDialogueActive = true;
            dialogueUI.SetActive(true);

            Canvas canvas = dialogueUI.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.enabled = true;
            }

            UpdateDialogue();
        }
        catch (System.Exception)
        {
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
        catch (System.Exception)
        {
        }
    }

    void EndDialogue()
    {
        try
        {
            isDialogueActive = false;
            dialogueUI.SetActive(false);
        }
        catch (System.Exception)
        {
        }
    }

    void OnValidate()
    {
        if (dialogueUI == null) Debug.LogWarning("dialogueUI is not assigned in DialogueManagerTest.", this);
        if (dialogueText == null) Debug.LogWarning("dialogueText is not assigned in DialogueManagerTest.", this);
        if (characterNameText == null) Debug.LogWarning("characterNameText is not assigned in DialogueManagerTest.", this);
        if (characterNameText2 == null) Debug.LogWarning("characterNameText2 is not assigned in DialogueManagerTest.", this);
        if (characterIllustration == null) Debug.LogWarning("characterIllustration is not assigned in DialogueManagerTest.", this);
        if (characterIllustration2 == null) Debug.LogWarning("characterIllustration2 is not assigned in DialogueManagerTest.", this);
        if (csvFile == null) Debug.LogWarning("csvFile is not assigned in DialogueManagerTest.", this);
    }
}