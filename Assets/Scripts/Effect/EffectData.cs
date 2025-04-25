using UnityEngine;

[CreateAssetMenu(fileName = "EffectData", menuName = "Effect/EffectData")]
public class EffectData : ScriptableObject
{
    public EffectType effectType;
    public EffectEndType effectEndType;
    public GameObject prefab;
    public float duration;

    [Header("반복 횟수 설정")]

    public bool useRepeat = false;
    public int RepeatCount;
    public float RepeatInterval;


}