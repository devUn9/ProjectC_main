using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance { get; private set; }

    [SerializeField] private List<EffectData> effectDataList;
    private Dictionary<string, EffectData> effectDataMap = new Dictionary<string, EffectData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        Initialize();
    }

    private void Initialize()
    {
        foreach (var data in effectDataList)
        {
            effectDataMap[data.effectName] = data;
        }
    }

    public void PlayEffect(string effectName, Vector3 position, Quaternion rotation = default, Sprite sprite = null, Color? color = null)
    {
        if (!effectDataMap.ContainsKey(effectName))
        {
            Debug.LogWarning($"Effect '{effectName}' not found.");
            return;
        }

        var data = effectDataMap[effectName];
        var effectObj = Instantiate(data.prefab, position, rotation);
        var controller = effectObj.AddComponent<EffectController>();
        controller.Initialize(data);
        controller.Play(position, rotation, data.duration, sprite, color);
    }

    public void PlaySpriteTrail(string effectName, Transform spawnPoint, Sprite sprite, float activeTime, SpriteRenderer originalRenderer = null)
    {
        if (!effectDataMap.ContainsKey(effectName))
        {
            Debug.LogWarning($"Effect '{effectName}' not found.");
            return;
        }

        var data = effectDataMap[effectName];
        if (!data.isSpriteTrail)
        {
            Debug.LogWarning($"Effect '{effectName}' is not a SpriteTrail.");
            return;
        }

        StartCoroutine(SpawnSpriteTrail(effectName, spawnPoint, sprite, activeTime, data, originalRenderer));
    }

    private IEnumerator SpawnSpriteTrail(string effectName, Transform spawnPoint, Sprite sprite, float activeTime, EffectData data, SpriteRenderer originalRenderer)
    {
        int trailIndex = 0;
        float time = activeTime;

        while (time > 0)
        {
            time -= data.refreshRate;

            var color = data.trailColors[trailIndex % data.trailColors.Length];
            var effectObj = Instantiate(data.prefab, spawnPoint.position, spawnPoint.rotation);
            var controller = effectObj.AddComponent<EffectController>();
            controller.Initialize(data);

            // 원본 SpriteRenderer의 sortingOrder를 기반으로 설정
            if (originalRenderer != null)
            {
                var sr = effectObj.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sortingLayerID = originalRenderer.sortingLayerID;
                    sr.sortingOrder = originalRenderer.sortingOrder - 1;
                }
            }

            controller.Play(spawnPoint.position, spawnPoint.rotation, data.duration, sprite, color);
            trailIndex++;

            yield return new WaitForSeconds(data.refreshRate);
        }
    }
}