using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapFade : MonoBehaviour
{
    [SerializeField] private Transform player; // 플레이어 Transform
    [SerializeField] private Transform referencePoint; // 투명한 원 오브젝트 Transform
    [SerializeField] private float maxDistance = 5f; // 알파값이 변하기 시작하는 거리
    [SerializeField] private float minAlpha = 0f; // 최소 알파값
    [SerializeField] private float maxAlpha = 1f; // 최대 알파값

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

        // 플레이어와 투명한 원 오브젝트 간의 거리 계산
        float distance = Vector2.Distance(player.position, referencePoint.position);

        // 거리에 따라 알파값 계산
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, distance / maxDistance);

        // 알파값이 0 이하로 내려가지 않도록 클램프
        alpha = Mathf.Clamp01(alpha);

        // 타일맵 색상 변경
        Color tilemapColor = tilemap.color;
        tilemapColor.a = alpha;
        tilemap.color = tilemapColor;
    }
}