using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;


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

        if(spriteRenderer != null)
        {

        }

        gameObject.SetActive(true);

        if (effectData.effectEndType == EffectEndType.DurationBased)
        {
            Invoke(nameof(DestroyEffect), duration);
        }


    }

    public void DestroyEffect()
    {
        Destroy(gameObject);
    }

}