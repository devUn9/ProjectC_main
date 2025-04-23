using UnityEngine;

[CreateAssetMenu(fileName = "EffectData", menuName = "Effect/EffectData")]
public class EffectData : ScriptableObject
{
    public EffectType effectType; // ¿Ã∆Â∆Æ ≈∏¿‘
    public GameObject prefab; // ¿Ã∆Â∆Æ «¡∏Æ∆’
    public float duration = 2f; // ¿Ã∆Â∆Æ ¡ˆº” Ω√∞£

}