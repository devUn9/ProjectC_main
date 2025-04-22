using UnityEngine;

[CreateAssetMenu(fileName = "EffectData", menuName = "Effect/EffectData")]
public class EffectData : ScriptableObject
{
    public string effectName; // ����Ʈ �̸� (��: "Explosion", "SpriteTrail")
    public GameObject prefab; // ����Ʈ ������
    public float duration = 2f; // ����Ʈ ���� �ð�
    public bool isSpriteTrail = false; // SpriteTrail ����
    // SpriteTrail ���� ����
    public float refreshRate = 0.05f; // �ܻ� ���� �ֱ�
    public Color[] trailColors; // ���� �迭
    public Material spriteMaterial; // �ܻ� ����
}