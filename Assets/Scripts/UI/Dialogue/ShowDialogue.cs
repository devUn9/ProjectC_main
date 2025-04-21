using UnityEngine;

public class ShowDialogue : MonoBehaviour
{
    [SerializeField] private InteractionEvent IE; // InteractionEvent 참조
    [SerializeField] private DialogueManager DM; // DialogueManager 참조

    void Start()
    {
        if (IE == null)
        {
            IE = GetComponent<InteractionEvent>();
        }

        if (DM == null)
        {
            DM = GetComponent<DialogueManager>();
        }

        // IE 또는 DM이 여전히 null이라면 경고 출력
        if (IE == null || DM == null)
        {
            Debug.LogError("InteractionEvent 또는 DialogueManager를 찾을 수 없습니다. Inspector에서 설정하거나 같은 GameObject에 추가하세요.");
        }
    }

    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            if (IE != null && DM != null)
            {
                // InteractionEvent에서 대화 데이터를 가져와 DialogueManager로 전달
                DM.ShowDialogue(IE.GetDialogues());
            }
        }
    }
}
