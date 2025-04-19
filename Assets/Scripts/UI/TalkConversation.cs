using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TalkConversation : MonoBehaviour
{
    public static TalkConversation instance;
    public TalkManager talkManager;
    //public GameObject talkPanel;
    public TextMeshProUGUI talkText;
    private GameObject scanObject;
    public bool isAction;
    public int talkIndex;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if(hit.collider != null)
            {
                GameObject clickedObject = hit.collider.gameObject;
                
                Action(clickedObject);
            }
        }
    }

    

    public void Action(GameObject scanObj)
    {
        if (isAction)
        {   //Exit Action
            isAction = false;
        }
        else
        {   //Enter Action
            isAction = true;
            scanObject = scanObj;
            ObjData objData = scanObject.GetComponent<ObjData>();
            Talk(objData.id);
        }

        //talkPanel.SetActive(isAction);
    }

    void Talk(int id)
    {
        string talkData = talkManager.GetTalk(id, talkIndex);

        talkText.text = talkData;
    }
}
