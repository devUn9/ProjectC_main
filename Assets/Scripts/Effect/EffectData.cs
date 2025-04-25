using UnityEngine;

[CreateAssetMenu(fileName = "EffectData", menuName = "Effect/EffectData")]
public class EffectData : ScriptableObject
{
    public EffectType effectType;
    public EffectEndType effectEndType;
    public GameObject prefab;
    public float duration;

    [Header("�ݺ� Ƚ�� ����")]

    public bool useRepeat = false;
    public int RepeatCount;
    public float RepeatInterval;


}