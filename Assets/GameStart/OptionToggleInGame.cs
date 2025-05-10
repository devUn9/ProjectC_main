using UnityEngine;

public class OptionToggleInGame : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && OptionsManager.Instance != null)
        {
            OptionsManager.Instance.ToggleOption();
        }
    }
}
