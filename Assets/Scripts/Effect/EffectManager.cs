using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance { get; private set; }

    [SerializeField] private List<EffectData> effectDataList;

    private Dictionary<EffectType, EffectData> effectDataMap = new Dictionary<EffectType, EffectData>();

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
            if (data.effectType == EffectType.None) continue;

            if (!effectDataMap.ContainsKey(data.effectType))
            {
                effectDataMap.Add(data.effectType, data);
            }
            else
            {
                Debug.LogWarning($"����Ʈ�� �ߺ��˴ϴ� : {data.effectType}");
            }
        }
    }

    public void PlayEffect(EffectType effectType, Vector3 position, Quaternion rotation = default)
    {
        if (!effectDataMap.TryGetValue(effectType, out EffectData data))
        {
            Debug.LogWarning($"Effect '{effectType}' ��(��) �����ϴ�. ");
            return;
        }

        GameObject effectObj = Instantiate(data.prefab, position, rotation);
        EffectController controller = effectObj.AddComponent<EffectController>();

        controller.Initialize(data);
        controller.Play(position, rotation, data.duration);
    }


  
    



}