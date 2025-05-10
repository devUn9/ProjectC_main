using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonFocusEffect : MonoBehaviour
{
    [SerializeField] private GameObject[] buttons;
    [SerializeField] private Vector3 focusedScale = new Vector3(1.2f, 1.2f, 1f);
    [SerializeField] private Vector3 normalScale = Vector3.one;

    private GameObject lastSelected;

    void Update()
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;

        if (selected != lastSelected)
        {
            foreach (GameObject btn in buttons)
                btn.transform.localScale = normalScale;

            if (selected != null && System.Array.Exists(buttons, b => b == selected))
                selected.transform.localScale = focusedScale;

            lastSelected = selected;
        }
    }
}
