using UnityEngine;
using UnityEngine.EventSystems;

public class HoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Vector3 hoverOffset; // 마우스 오버 시 이동할 위치
    private Vector3 originalPosition;

    private void Start()
    {
        // 원래 위치 저장
        originalPosition = transform.localPosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 마우스가 버튼 위로 갔을 때
        transform.localPosition = originalPosition + hoverOffset;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 마우스가 버튼에서 나갔을 때
        transform.localPosition = originalPosition;
    }
}