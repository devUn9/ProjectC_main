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


    // ���� �þ� ����Ʈ ũ��, ȸ��, ���� ���� 0 < targetRadius < 0.5 ���� �� �Է� ����, ȸ��, ���� �Է� �ʼ� �ƴ�
    public void SetSightEffect(float targetRadius, Quaternion rotation = default, float targetAngle = 90f )
    {
        if (spriteRenderer == null || spriteRenderer.material == null)
        {
            Debug.Log("SpriteRenderer �Ǵ� Material�� �����ϴ�.");
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
        float duration = 0.3f; // ��ȭ�ϴµ� �ɸ��� �ð� 

        mat.SetFloat("_Angle", targetAngle);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = Mathf.Clamp01(t); // Ȥ�� ���� ������ġ

            float tempRadius = Mathf.Lerp(startRadius, targetRadius, t);
            float tempAngle = Mathf.Lerp(startAngle, targetAngle, t);

            mat.SetFloat("_Radius", tempRadius);
            mat.SetFloat("_Angle", tempAngle);

            yield return null;
        }

        mat.SetFloat("_Radius", targetRadius);

        sightEffectCoroutine = null;
    }


    // ����

    public void DestroyEffect()
    {
        Destroy(gameObject);
    }

}