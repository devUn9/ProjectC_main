using System.Collections;
using UnityEngine;

public class SpriteTrail : MonoBehaviour
{
    public float activeTime = 2f;
    public float refreshRate = 0.05f;
    public float destroyDelay = 0.2f;
    public Transform spawnPoint;
    public Material spriteMaterial;

    float maxtrailCount = 0;
    int trailIndex = 0;

    SpriteRenderer spriteRenderer;
    bool isTrailActive = false;

    Color[] trailColors = {
    // 청록 → 파랑
    new Color(0f, 1f, 1f),  // 청록
    new Color(0f, 0.8f, 1f),
    new Color(0f, 0.6f, 1f),  // 파랑
    // 파랑 → 보라
    new Color(0.2f, 0.4f, 1f),
    new Color(0.4f, 0.2f, 1f),
    new Color(0.6f, 0f, 1f),  // 보라
    // 보라 → 빨강
    new Color(0.8f, 0f, 1f),
    new Color(1f, 0f, 0.6f),
    new Color(1f, 0f, 0.3f),  // 빨강
    // 빨강 → 노랑
    new Color(1f, 0.3f, 0f),
    new Color(1f, 0.6f, 0f),
    new Color(1f, 1f, 0f),  // 노랑
    // 노랑 → 청록 
    new Color(0.5f, 1f, 0f),
    new Color(0f, 1f, 0.5f),
    new Color(0f, 1f, 1f),  // 청록
};

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && !isTrailActive)
        {
            isTrailActive = true;
            StartCoroutine(SpawnSpriteTrail(activeTime));
        }
    }

    IEnumerator SpawnSpriteTrail(float time)
    {
        maxtrailCount = activeTime / refreshRate;

        while (time > 0)
        {
            time -= refreshRate;

            GameObject trail = new GameObject("SpriteTrail");
            trail.transform.position = spawnPoint.position;
            trail.transform.rotation = spawnPoint.rotation;
            trail.transform.localScale = transform.localScale;

            SpriteRenderer sr = trail.AddComponent<SpriteRenderer>();
            sr.sprite = spriteRenderer.sprite; // 현재 애니메이션 프레임 그대로 복사
            sr.sortingLayerID = spriteRenderer.sortingLayerID;
            sr.sortingOrder = spriteRenderer.sortingOrder - 1;
            sr.material = spriteMaterial;

            Color color = trailColors[trailIndex % trailColors.Length];
            trailIndex++;

            Material matInstance = new Material(spriteMaterial);
            matInstance.SetColor("_Trail_Color", color);
            sr.material = matInstance;

            Destroy(trail, time);
            yield return new WaitForSeconds(refreshRate);
        }

        isTrailActive = false;
    }
}
