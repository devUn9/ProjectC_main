using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;


public class EffectController : MonoBehaviour
{
    private ParticleSystem particle;
    private SpriteRenderer spriteRenderer;
    private Light2D sightLight;

    private EffectData effectData;

    private Coroutine sightEffectCoroutine;


    public void Initialize(EffectData data)
    {
        effectData = data;
        particle = GetComponent<ParticleSystem>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        sightLight = GetComponent<Light2D>();
        if (spriteRenderer != null) { spriteRenderer.material = new Material(spriteRenderer.material); } 
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


    // 적의 시야 이펙트 크기, 회전, 각도 조절
    public void SetSightEffect(float targetRadius, Quaternion rotation, float targetAngle)
    {
        if (sightLight == null)
        {
            Debug.LogWarning("Light2D 컴포넌트가 없습니다.");
            return;
        }

        if (sightEffectCoroutine != null)
        {
            StopCoroutine(sightEffectCoroutine);
        }


        sightEffectCoroutine = StartCoroutine(LerpSightEffectCoroutine(targetRadius, rotation, targetAngle));
    }

    public void SetSightColor(Color color)
    {
        if (sightLight != null)
        {
            sightLight.color = color;
        }
    }




    private IEnumerator LerpSightEffectCoroutine(float targetRadius,  Quaternion targetRotation, float targetAngle)
    {
        float startRadius = sightLight.pointLightOuterRadius;
        float startOuterAngle = sightLight.pointLightOuterAngle;
        float startInnerAngle = sightLight.pointLightInnerAngle;
        Quaternion startRotation = transform.localRotation;

        float modifier = 1;
        
        float targetOuterAngle = targetAngle;
        if (targetAngle == 360)
            modifier = 10/9f;
        
        float targetInnerAngle = targetAngle * modifier * 0.9f ;

        // DeltaAngle 보정 
        float correctedOuterTarget = startOuterAngle + Mathf.DeltaAngle(startOuterAngle, targetOuterAngle);
        float correctedInnerTarget = startInnerAngle + Mathf.DeltaAngle(startInnerAngle, targetInnerAngle);

        float elapsed = 0f;
        float duration = 0.3f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            sightLight.pointLightOuterRadius = Mathf.Lerp(startRadius, targetRadius, t);
            sightLight.pointLightOuterAngle = Mathf.Lerp(startOuterAngle, correctedOuterTarget, t);
            sightLight.pointLightInnerAngle = Mathf.Lerp(startInnerAngle, correctedInnerTarget, t);
            transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, t);

            yield return null;
        }

        // 최종 값 보정
        sightLight.pointLightOuterRadius = targetRadius;
        sightLight.pointLightOuterAngle = targetOuterAngle;
        sightLight.pointLightInnerAngle = targetInnerAngle;
        transform.localRotation = targetRotation;

        sightEffectCoroutine = null;
    }



    // 삭제

    public void DestroyEffect()
    {
        Destroy(gameObject);
    }

}

