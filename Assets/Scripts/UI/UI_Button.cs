using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Button : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private Image buttonImage;        // 기본 이미지
    private Sprite defaultSprite;    // 기본 이미지
    [SerializeField] private Sprite hoverSprite;      // 마우스 오버시 보여줄 이미지
    [SerializeField] private GameObject explanation_Tab;

    bool isClicked = false;

    private void Start()
    {
        buttonImage = GetComponent<Image>();
        defaultSprite = buttonImage.sprite;  // 시작할 때 기본 이미지 설정
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            explanation_Tab.SetActive(false);
            isClicked = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 마우스가 버튼 위로 올라갈 때
        buttonImage.sprite = hoverSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 마우스가 버튼에서 벗어날 때
        buttonImage.sprite = defaultSprite;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 마우스가 버튼을 클릭할 때
        buttonImage.sprite = defaultSprite;
        if (!isClicked)
        {
            OptionsManager.Instance.CloseOption();
            explanation_Tab.SetActive(true);
        }
        else
        {
            OptionsManager.Instance.ToggleOption();
            explanation_Tab.SetActive(false);
        }
        isClicked = !isClicked;
    }

    public void DisableButton()
    {
        gameObject.SetActive(false);
    }
}
