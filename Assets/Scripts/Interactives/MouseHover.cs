using UnityEngine;
using TMPro;

public class MouseHover : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private SpriteRenderer hoverImage; // 2D 스프라이트
    [SerializeField] private Vector3 textOffset = new Vector3(0, 2f, 0); // 텍스트 오프셋 (World)
    [SerializeField] private Vector3 imageOffset = new Vector3(0, 1f, 0); // 이미지 오프셋 (World)
    [SerializeField] private LayerMask enemyLayer; // 감지할 레이어 (Enemy 레이어로 설정)

    private Enemy enemy;
    private EnemyStats stat;
    private CanvasGroup textCanvasGroup;
    private Camera mainCamera;
    private bool isMouseOver;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
        stat = GetComponentInParent<EnemyStats>();
        mainCamera = Camera.main;

        if (speedText == null || hoverImage == null || enemy == null || mainCamera == null || GetComponent<Collider2D>() == null)
        {
            enabled = false;
            return;
        }

        textCanvasGroup = speedText.GetComponent<CanvasGroup>();
        if (textCanvasGroup == null)
            textCanvasGroup = speedText.gameObject.AddComponent<CanvasGroup>();

        textCanvasGroup.alpha = 0f;
        hoverImage.color = new Color(1, 1, 1, 0); // 이미지 초기 투명
    }

    private void Update()
    {
        if (speedText == null || hoverImage == null || textCanvasGroup == null || mainCamera == null)
            return;

        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        // enemyLayer를 사용하여 Enemy 레이어만 감지
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, enemyLayer);

        bool wasMouseOver = isMouseOver;
        isMouseOver = hit.collider != null && hit.collider.gameObject == gameObject;

        if (isMouseOver)
        {
            if (!wasMouseOver)
            {
                string attackType = enemy.isMelee ? "근접" : "원거리";
                Stat attackDamage = enemy.isMelee ? stat.meleeDamage : stat.bulletDamage;
                speedText.text = $" 공격 타입 : {attackType}\n 공  격  력 : {attackDamage.GetValue()}\n 체        력 : {stat.currentHealth}";
                textCanvasGroup.alpha = 1f;
                hoverImage.color = new Color(1, 1, 1, 0.75f); // 이미지 표시
            }

            // 자식 오브젝트의 로컬 위치로 오프셋 적용
            speedText.transform.localPosition = textOffset;
            hoverImage.transform.localPosition = imageOffset;
        }
        else if (wasMouseOver)
        {
            textCanvasGroup.alpha = 0f;
            hoverImage.color = new Color(1, 1, 1, 0); // 이미지 숨김
            speedText.text = "";
        }
    }
}