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

    public void Play(Vector3 position, Quaternion rotation, float duration)
    {
        transform.position = position;
        transform.rotation = rotation;

        if (particle != null)
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