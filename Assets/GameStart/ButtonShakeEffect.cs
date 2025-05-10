using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonShakeEffect : MonoBehaviour
{
    [SerializeField] private GameObject[] buttons;
    [SerializeField] private float shakeAmount = 2f;
    [SerializeField] private float shakeSpeed = 20f;

    private GameObject current;
    private Vector3[] originalPositions;

    void Start()
    {
        originalPositions = new Vector3[buttons.Length];
        for (int i = 0; i < buttons.Length; i++)
            originalPositions[i] = buttons[i].transform.localPosition;
    }

    void Update()
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;
        for (int i = 0; i < buttons.Length; i++)
        {
            GameObject btn = buttons[i];
            if (btn == selected)
            {
                float offset = Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;
                btn.transform.localPosition = originalPositions[i] + new Vector3(offset, 0, 0);
            }
            else
            {
                btn.transform.localPosition = originalPositions[i];
            }
        }
    }
}
