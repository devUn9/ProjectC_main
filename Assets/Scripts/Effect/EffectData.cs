using UnityEngine;

[CreateAssetMenu(fileName = "EffectData", menuName = "Effect/EffectData")]
public class EffectData : ScriptableObject
{
    public string effectName; // 이펙트 이름 (예: "Explosion", "SpriteTrail")
    public GameObject prefab; // 이펙트 프리팹
    public float duration = 2f; // 이펙트 지속 시간
    public bool isSpriteTrail = false; // SpriteTrail 여부
    // SpriteTrail 전용 설정
    public float refreshRate = 0.05f; // 잔상 생성 주기
    public Color[] trailColors; // 색상 배열
    public Material spriteMaterial; // 잔상 재질
}