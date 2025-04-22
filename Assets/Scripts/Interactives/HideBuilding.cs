using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapFade : MonoBehaviour
{
    [SerializeField] private Transform player; // �÷��̾� Transform
    [SerializeField] private Transform referencePoint; // ������ �� ������Ʈ Transform
    [SerializeField] private float maxDistance = 5f; // ���İ��� ���ϱ� �����ϴ� �Ÿ�
    [SerializeField] private float minAlpha = 0f; // �ּ� ���İ�
    [SerializeField] private float maxAlpha = 1f; // �ִ� ���İ�

    private Tilemap tilemap;

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
    {
        if (player == null || tilemap == null || referencePoint == null) return;

        // �÷��̾�� ������ �� ������Ʈ ���� �Ÿ� ���
        float distance = Vector2.Distance(player.position, referencePoint.position);

        // �Ÿ��� ���� ���İ� ���
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, distance / maxDistance);

        // ���İ��� 0 ���Ϸ� �������� �ʵ��� Ŭ����
        alpha = Mathf.Clamp01(alpha);

        // Ÿ�ϸ� ���� ����
        Color tilemapColor = tilemap.color;
        tilemapColor.a = alpha;
        tilemap.color = tilemapColor;
    }
}