using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject firstSelected;

    private void OnEnable()
    {
        // 옵션 창이 열릴 때 슬라이더에 포커스 맞추기
        EventSystem.current.SetSelectedGameObject(null); // 리셋
        EventSystem.current.SetSelectedGameObject(firstSelected);
    }
}
