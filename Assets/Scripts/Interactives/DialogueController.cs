using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI characterNameText2;
    [SerializeField] private Image characterIllustration;
    [SerializeField] private Image characterIllustration2;
    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private GameObject blackScreenUI; // Black screen UI
    [SerializeField] private TextMeshProUGUI blackScreenText; // Text for black screen
    [SerializeField] private TextAsset[] csvFiles; // Array of CSV files
    [SerializeField] private Image blinkImageUI;
    [SerializeField] private GameObject player;

    private List<DialogueTest> dialogues = new List<DialogueTest>();
    private List<Sprite> illustrations = new List<Sprite>();
    private int currentDialogueIndex = 0;
    private int currentCsvIndex = 0;
    private bool isDialogueActive = false;
    private float lastInputTime = 0f;
    private bool isBlackScreenActive = false;

    [Header("Timing Settings")]
    [SerializeField] private float inputCooldown = 1f;
    [SerializeField] private float noInputThreshold = 5f;
    private bool isBlinking = false;
    private Animator playerAnimator;

    [System.Serializable]
    public class DialogueTest
    {
        public int id;
        public string character;
        public string dialogue;
        public int illustrationIndex;
        public int illustrationIndex2;
        public bool useBlackScreen; // Flag for black screen usage
    }

    void Start()
    {
        try
        {
            LoadIllustrations();
            dialogueUI.SetActive(false);
            blackScreenUI.SetActive(false);
            if (blinkImageUI != null) blinkImageUI.gameObject.SetActive(false);

            if (player != null)
            {
                playerAnimator = player.GetComponentInChildren<Animator>();
                if (playerAnimator == null)
                {
                    Debug.LogWarning("Player has no Animator component.", player);
                }
            }
            else
            {
                Debug.LogWarning("Player object not assigned.", this);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Start error: {ex.Message}");
        }
    }

    void Update()
    {
        if (isDialogueActive)
        {
            if (Time.unscaledTime - lastInputTime >= noInputThreshold && !isBlinking)
            {
                StartBlinking();
            }

            if (Input.GetKeyDown(KeyCode.Space) && Time.unscaledTime - lastInputTime >= inputCooldown)
            {
                lastInputTime = Time.unscaledTime;
                StopBlinking();
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
                Debug.LogWarning("No sprites loaded from Illustrations folder.");
                return;
            }
            illustrations.AddRange(loadedSprites);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"LoadIllustrations error: {ex.Message}");
        }
    }

    void LoadCSV(int csvIndex)
    {
        try
        {
            if (csvFiles == null || csvIndex < 0 || csvIndex >= csvFiles.Length || csvFiles[csvIndex] == null)
            {
                Debug.LogWarning($"Invalid CSV index {csvIndex} or CSV file not assigned.");
                return;
            }

            dialogues.Clear();
            string[] lines = csvFiles[csvIndex].text.Split('\n');

            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrEmpty(line))
                    continue;

                string[] fields = SplitCSVLine(line);
                if (fields.Length < 6) // Increased to account for blackScreen flag
                    continue;

                try
                {
                    DialogueTest dialogue = new DialogueTest
                    {
                        id = int.Parse(fields[0]),
                        character = fields[1],
                        dialogue = fields[2],
                        illustrationIndex = int.Parse(fields[3]),
                        illustrationIndex2 = int.Parse(fields[4]),
                        useBlackScreen = bool.Parse(fields[5]) // New field for black screen
                    };
                    dialogues.Add(dialogue);
                }
                catch (System.FormatException ex)
                {
                    Debug.LogWarning($"CSV parsing error (line {i}): {ex.Message}");
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"LoadCSV error: {ex.Message}");
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
            Debug.LogError($"SplitCSVLine error: {ex.Message}");
            return new string[0];
        }
    }

    void UpdateDialogue()
    {
        try
        {
            if (currentDialogueIndex >= dialogues.Count)
            {
                LoadNextCSV();
                return;
            }

            DialogueTest current = dialogues[currentDialogueIndex];

            // Handle UI based on black screen flag
            if (current.useBlackScreen)
            {
                isBlackScreenActive = true;
                dialogueUI.SetActive(false);
                blackScreenUI.SetActive(true);
                blackScreenText.text = current.dialogue;
            }
            else
            {
                isBlackScreenActive = false;
                dialogueUI.SetActive(true);
                blackScreenUI.SetActive(false);
                UpdateRegularDialogue(current);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"UpdateDialogue error: {ex.Message}");
            EndDialogue();
        }
    }

    void UpdateRegularDialogue(DialogueTest current)
    {
        dialogueText.text = current.dialogue;
        characterNameText.text = current.character;
        characterNameText2.text = current.character;

        // Character-specific name colors
        if (current.character == "David")
        {
            characterNameText.color = new Color(0.1f, 0.9f, 0.9f); // Cyan
        }
        else if (current.character == "Lucy")
        {
            characterNameText2.color = new Color(0.9f, 0.33f, 1f); // Pink
        }
        else if (current.character == "Ripperdoc")
        {
            characterNameText2.color = new Color(0.33f, 0.95f, 0.15f); // Green
        }
        else if (current.character == "Adam Smasher")
        {
            characterNameText2.color = new Color(1f, 0f, 0f); // Red
        }
        else
        {
            characterNameText.color = Color.white;
            characterNameText2.color = Color.white;
        }

        // Illustration setup
        if (current.illustrationIndex >= 0 && current.illustrationIndex < illustrations.Count)
        {
            characterIllustration.sprite = illustrations[current.illustrationIndex];
            characterIllustration.enabled = true;
        }
        else
        {
            characterIllustration.enabled = false;
        }

        if (current.illustrationIndex2 >= 0 && current.illustrationIndex2 < illustrations.Count)
        {
            characterIllustration2.sprite = illustrations[current.illustrationIndex2];
            characterIllustration2.enabled = true;
        }
        else
        {
            characterIllustration2.enabled = false;
        }

        // Dialogue ID-based UI adjustments
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

    public void StartDialogue(int startCsvIndex = 0)
    {
        try
        {
            currentCsvIndex = startCsvIndex;
            LoadCSV(currentCsvIndex);
            if (dialogues.Count == 0)
            {
                Debug.LogWarning("No dialogues loaded from CSV.");
                return;
            }

            Time.timeScale = 0f;
            if (playerAnimator != null)
            {
                playerAnimator.enabled = false;
            }

            currentDialogueIndex = 0;
            isDialogueActive = true;
            lastInputTime = Time.unscaledTime;
            UpdateDialogue();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"StartDialogue error: {ex.Message}");
        }
    }

    void LoadNextCSV()
    {
        currentCsvIndex++;
        if (currentCsvIndex < csvFiles.Length)
        {
            LoadCSV(currentCsvIndex);
            if (dialogues.Count > 0)
            {
                currentDialogueIndex = 0;
                UpdateDialogue();
            }
            else
            {
                EndDialogue();
            }
        }
        else
        {
            EndDialogue();
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
                LoadNextCSV();
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"NextDialogue error: {ex.Message}");
        }
    }

    void EndDialogue()
    {
        try
        {
            isDialogueActive = false;
            dialogueUI.SetActive(false);
            blackScreenUI.SetActive(false);
            StopBlinking();

            Time.timeScale = 1f;
            if (playerAnimator != null)
            {
                playerAnimator.enabled = true;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"EndDialogue error: {ex.Message}");
        }
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
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }
}