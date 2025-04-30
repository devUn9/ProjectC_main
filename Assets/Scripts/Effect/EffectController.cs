using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;


public class EffectController : MonoBehaviour
{
    private ParticleSystem particle;
    private SpriteRenderer spriteRenderer;
    private EffectData effectData;

    private Coroutine sightEffectCoroutine;


    public void Initialize(EffectData data)
    {
        effectData = data;
        particle = GetComponent<ParticleSystem>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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


    // 적의 시야 이펙트 크기, 회전, 각도 조절 0 < targetRadius < 0.5 사이 값 입력 권장, 회전, 각도 입력 필수 아님
    public void SetSightEffect(float targetRadius, Quaternion rotation = default, float targetAngle = 90f )
    {
        if (spriteRenderer == null || spriteRenderer.material == null)
        {
            Debug.Log("SpriteRenderer 또는 Material이 없습니다.");
            return;
        }

        if (sightEffectCoroutine != null)
        {
            StopCoroutine(sightEffectCoroutine);
        }

        float targetAngleRadian = (targetAngle * Mathf.Deg2Rad)/2;

        sightEffectCoroutine = StartCoroutine(LerpSightEffectCoroutine(targetRadius, targetAngleRadian));

        if (rotation != default)
        {
            transform.localRotation = rotation;
        }

    }


    private IEnumerator LerpSightEffectCoroutine(float targetRadius, float targetAngle)
    {
        Material mat = spriteRenderer.material;

        float startRadius = mat.GetFloat("_Radius");
        float startAngle = mat.GetFloat("_Angle");

        float elapsed = 0f;
        float duration = 0.3f; // 변화하는데 걸리는 시간 

        mat.SetFloat("_Angle", targetAngle);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = Mathf.Clamp01(t); // 혹시 몰라서 안전장치

            float tempRadius = Mathf.Lerp(startRadius, targetRadius, t);
            float tempAngle = Mathf.Lerp(startAngle, targetAngle, t);

            mat.SetFloat("_Radius", tempRadius);
            mat.SetFloat("_Angle", tempAngle);

            yield return null;
        }

        mat.SetFloat("_Radius", targetRadius);

        sightEffectCoroutine = null;
    }


    // 삭제

    public void DestroyEffect()
    {
        Destroy(gameObject);
    }

}