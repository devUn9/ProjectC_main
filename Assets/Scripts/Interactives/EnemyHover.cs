using UnityEngine;
using TMPro;

public class MouseHover : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private SpriteRenderer hoverImage; // 2D ��������Ʈ
    [SerializeField] private Vector3 textOffset = new Vector3(0, 2f, 0); // �ؽ�Ʈ ������ (World)
    [SerializeField] private Vector3 imageOffset = new Vector3(0, 1f, 0); // �̹��� ������ (World)
    private Enemy enemy;
    private CanvasGroup textCanvasGroup;
    private Camera mainCamera;
    private bool isMouseOver;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        mainCamera = Camera.main;

        if (speedText == null || hoverImage == null || enemy == null || mainCamera == null || GetComponent<Collider2D>() == null)
        {
            enabled = false;
            return;
        }

        textCanvasGroup = speedText.GetComponent<CanvasGroup>();
        if (textCanvasGroup == null)
            textCanvasGroup = speedText.gameObject.AddComponent<CanvasGroup>();

        //// speedText�� hoverImage�� ���� �ڽ����� ����
        //speedText.transform.SetParent(transform);
        //hoverImage.transform.SetParent(transform);

        textCanvasGroup.alpha = 0f;
        hoverImage.color = new Color(1, 1, 1, 0); // �̹��� �ʱ� ����
    }

    private void Update()
    {
        if (speedText == null || hoverImage == null || textCanvasGroup == null || mainCamera == null)
            return;

        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        bool wasMouseOver = isMouseOver;
        isMouseOver = hit.collider != null && hit.collider.gameObject == gameObject;

        if (isMouseOver)
        {
            if (!wasMouseOver)
            {
                speedText.text = $"�̵��ӵ� : {enemy.moveSpeed}";
                textCanvasGroup.alpha = 1f;
                hoverImage.color = new Color(1, 1, 1, 0.75f); // �̹��� ǥ��
            }

            // �ڽ� ������Ʈ�� ���� ��ġ�� ������ ����
            speedText.transform.localPosition = textOffset;
            hoverImage.transform.localPosition = imageOffset;
        }
        else if (wasMouseOver)
        {
            textCanvasGroup.alpha = 0f;
            hoverImage.color = new Color(1, 1, 1, 0); // �̹��� ����
            speedText.text = "";
        }
    }
}