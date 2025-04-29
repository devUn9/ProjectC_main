using UnityEngine;

public class Npc : MonoBehaviour
{
    public static Npc instance;
    public float interactionRange;
    public Transform player;
    public GameObject interactive_KeyUI;
    public GameObject selectInterface; // 선택 인터페이스 UI

    private InteractionEvent IE; // InteractionEvent 참조
    private DialogueManager DM; // DialogueManager 참조

    private bool isPlayerNearby = false;
    private bool doSomething = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        IE = GetComponent<InteractionEvent>();
        DM = DialogueManager.instance.GetComponent<DialogueManager>();
    }

    void Update()
    {
        CheckNearby();
        InteractiveKeySet();
        CheckInput();
    }

    private void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.E) && isPlayerNearby)
        {
            doSomething = true;
            ShowSelectInterface();
        }
    }

    private void CheckNearby()
    {
        float distance = Vector3.Distance(player.position, transform.position);
        isPlayerNearby = distance <= interactionRange;
    }

    private void InteractiveKeySet()
    {
        if (isPlayerNearby && !doSomething)
            interactive_KeyUI.SetActive(true);
        else
            interactive_KeyUI.SetActive(false);
    }

    private void ShowSelectInterface()
    {
        selectInterface.SetActive(true);
    }

    public void ShowDialogue()
    {
        DM.ShowDialogue(IE.GetDialogues());
    }
}
