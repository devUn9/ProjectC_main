using UnityEngine;

public class EndingCredit : MonoBehaviour
{
    [SerializeField] private RectTransform creditsContent; // 크레딧 콘텐츠의 RectTransform (Panel 또는 ScrollView의 Content)
    [SerializeField] private float scrollSpeed = 50f; // 스크롤 속도
    [SerializeField] private float endPositionY = 2000f; // 크레딧이 도달할 최종 Y 위치

    private PlayerGravitonState playerGravitonState;
    public bool isScrolling = false;


    void Update()
    {
        if (!isScrolling) return;

        // 콘텐츠를 위로 이동
        creditsContent.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

        // 콘텐츠가 목표 Y 위치에 도달하면 스크롤 중지
        if (creditsContent.anchoredPosition.y >= endPositionY)
        {
            isScrolling = false;
            // 선택적으로 여기서 씬 전환, 게임 종료 등 추가 로직 구현
            Application.Quit();
        }
    }

    // 스크롤 시작/재시작 메서드 (필요 시 호출)
    public void StartScrolling()
    {
        isScrolling = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}