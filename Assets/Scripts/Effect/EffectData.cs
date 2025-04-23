using UnityEngine;

[CreateAssetMenu(fileName = "EffectData", menuName = "Effect/EffectData")]
public class EffectData : ScriptableObject
{
    public EffectType effectType; // ����Ʈ Ÿ��
    public GameObject prefab; // ����Ʈ ������
    public float duration = 2f; // ����Ʈ ���� �ð�

}