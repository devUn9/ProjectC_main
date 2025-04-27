using UnityEngine;
using UnityEngine.UI;

public class Option : MonoBehaviour
{
    private Button[] optionButtons;

    private void Start()
    {
        optionButtons = GetComponentsInChildren<Button>();

        if (optionButtons != null && optionButtons.Length > 0)
        {
            for (int i = 0; i < optionButtons.Length; i++)
            {
                int index = i;
                optionButtons[i].onClick.AddListener(() => OnButtonClicked(index));

                AddHoverEffect(optionButtons[i]);
            }
        }
    }

    private void OnButtonClicked(int buttonIndex)
    {
        Debug.Log($"Button {buttonIndex + 1} clicked!");
        switch (buttonIndex)
        {
            case 0:
                Debug.Log("첫 번째 버튼 동작 실행");
                break;
            case 1:
                Debug.Log("두 번째 버튼 동작 실행");
                break;
            case 2:
                Debug.Log("세 번째 버튼 동작 실행");
                break;
            case 3:
                Debug.Log("네 번째 버튼 동작 실행");
                break;
        }
    }

    private void AddHoverEffect(Button button)
    {
        HoverEffect hoverEffect = button.gameObject.AddComponent<HoverEffect>();
        hoverEffect.hoverOffset = new Vector3(0, 4f, 0);
    }
}
