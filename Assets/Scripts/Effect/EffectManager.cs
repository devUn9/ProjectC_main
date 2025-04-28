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

    public void PlayEffect(EffectType effectType, Vector3 position, Quaternion rotation = default)  // Ư�� ��ġ�� ���⿡�� ����Ʈ ���� �Լ�
    {
        if (!effectDataMap.TryGetValue(effectType, out EffectData data))
        {
            Debug.LogWarning($"Effect '{effectType}' ��(��) �����ϴ�.");
            return;
        }

        if (data.useRepeat)
        {
            StartCoroutine(PlayEffectRepeat(data, position, rotation));
            return;
        }
        else
        {
            PlaySingleEffect(data, position, rotation);
        } 
    }


    public EffectController PlayEffectFollow(EffectType effectType, Transform followTarget, Vector3 offset = default)
    {
        if (!effectDataMap.TryGetValue(effectType, out EffectData data))
        {
            Debug.LogWarning($"Effect '{effectType}' ��(��) �����ϴ�.");
            return null;
        }

        GameObject obj = Instantiate(data.prefab, followTarget); 
        obj.transform.localPosition = offset;                    
        obj.transform.localRotation = Quaternion.identity;       

        EffectController controller = obj.AddComponent<EffectController>();
        controller.Initialize(data);
        controller.Play(obj.transform.position, obj.transform.rotation, data.duration);

        return controller;
    }



    private void PlaySingleEffect(EffectData data, Vector3 position, Quaternion rotation)
    {
        GameObject obj = Instantiate(data.prefab, position, rotation);
        EffectController controller = obj.AddComponent<EffectController>();
        controller.Initialize(data);
        controller.Play(position, rotation, data.duration);
    }

    private IEnumerator PlayEffectRepeat(EffectData data, Vector3 position, Quaternion rotation)
    {
        for (int i = 0; i < data.RepeatCount; i++)
        {
            PlaySingleEffect(data, position, rotation);
            yield return new WaitForSeconds(data.RepeatInterval);
        }
    }







}