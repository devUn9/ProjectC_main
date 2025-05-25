using UnityEngine;

public class Tab_Minimab : MonoBehaviour
{
    public Canvas canvasToToggle; // 토글할 Canvas (예: MinimapCanvas)

    void Start()
    {
        // 초기 상태 설정 (선택 사항)
        if (canvasToToggle != null)
        {
            canvasToToggle.gameObject.SetActive(false); // 기본적으로 비활성화
        }
    }

    void Update()
    {
        // Tab 키 입력 감지
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (canvasToToggle != null)
            {
                // Canvas 상태 토글
                canvasToToggle.gameObject.SetActive(!canvasToToggle.gameObject.activeSelf);
            }
        }
    }
}