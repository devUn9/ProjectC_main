using UnityEngine;

public class DissolveShaderControl : MonoBehaviour
{
    private Material material;
    private bool isRunning = false;

    void Start()
    {
        // 플레이어의 SpriteRenderer에서 머티리얼을 가져옴
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            material = sr.material;
        }
        else
        {
            Debug.LogError("SpriteRenderer가 없습니다.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && !isRunning && material != null)
        {
            StartCoroutine(DissolveSequence());
        }
    }

    private System.Collections.IEnumerator DissolveSequence()
    {
        isRunning = true;

        float duration = 1f;
        float timer = 0f;

        // 1초 동안 1 → -1로 선형 감소
        while (timer < duration)
        {
            float t = timer / duration; // 0 → 1
            float value = Mathf.Lerp(1f, -1f, t);
            material.SetFloat("_SplitValue", value);
            timer += Time.deltaTime;
            yield return null;
        }

        material.SetFloat("_SplitValue", -1f);

        // 1초 대기 후 복구
        yield return new WaitForSeconds(1f);
        material.SetFloat("_SplitValue", 1f);

        isRunning = false;
    }
}
