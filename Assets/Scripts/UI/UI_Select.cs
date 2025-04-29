using UnityEngine;

public class UI_Select : UI_Option
{
    public GameObject StoreInterface;
    
    protected override void Start()
    {
        base.Start();
    }

    protected override void OnButtonClicked(int buttonIndex)
    {
        base.OnButtonClicked(buttonIndex);

        switch (buttonIndex)
        {
            case 0:
                Npc.instance.ShowDialogue();
                gameObject.SetActive(false);
                break;
            case 1:
                StoreInterface.SetActive(true);
                gameObject.SetActive(false);
                break;
            case 2:
                gameObject.SetActive(false);
                break;
        }
    }
}
