using UnityEngine;

public class Npc : MonoBehaviour
{
    public float interactionRange;
    public Transform player;
    public GameObject interactive_KeyUI;

    public InteractionEvent IE; // InteractionEvent 참조
    public DialogueManager DM; // DialogueManager 참조

    private bool isPlayerNearby = false;

    void Start()
    {
        IE = GetComponent<InteractionEvent>();
        DM = DialogueManager.instance.GetComponent<DialogueManager>();
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        isPlayerNearby = distance <= interactionRange;

        InteractiveKeySet();

        if (Input.GetKeyDown(KeyCode.E) && isPlayerNearby)
        {
            Debug.Log("실행됨");
            if (IE != null && DM != null)
            {
                // InteractionEvent에서 대화 데이터를 가져와 DialogueManager로 전달
                DM.ShowDialogue(IE.GetDialogues());
            }
        }
    }

    private void InteractiveKeySet()
    {
        if (isPlayerNearby)
            interactive_KeyUI.SetActive(true);
        else
            interactive_KeyUI.SetActive(false);
    }
}
