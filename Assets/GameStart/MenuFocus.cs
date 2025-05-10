using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuFocus : MonoBehaviour
{
    [SerializeField] private Button startButton;

    void Start()
    {
        EventSystem.current.SetSelectedGameObject(null); // 기존 포커스 제거
        EventSystem.current.SetSelectedGameObject(startButton.gameObject); // 시작 버튼 선택
    }
}
