using UnityEngine;

public class EffectController : MonoBehaviour
{
    private ParticleSystem particle;
    private SpriteRenderer spriteRenderer;
    private EffectData effectData;

    public void Initialize(EffectData data)
    {
        effectData = data;
        particle = GetComponent<ParticleSystem>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Play(Vector3 position, Quaternion rotation, float duration, Sprite sprite = null, Color? color = null)
    {
        transform.position = position;
        transform.rotation = rotation;

        if (effectData.isSpriteTrail && spriteRenderer != null)
        {
            if (sprite != null) spriteRenderer.sprite = sprite;
            if (color.HasValue) spriteRenderer.color = color.Value;
            spriteRenderer.material = effectData.spriteMaterial;
        }
        else if (particle != null)
        {
            particle.Play();
        }

        gameObject.SetActive(true);
        Invoke(nameof(Deactivate), duration);
    }

    private void Deactivate()
    {
        Destroy(gameObject);
    }
}