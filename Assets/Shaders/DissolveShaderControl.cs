using UnityEngine;

public class DissolveShaderControl : MonoBehaviour
{
    [SerializeField] private Material material;

    private bool isRunning = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && !isRunning)
        {
            StartCoroutine(DissolveSequence());
        }
    }

    private System.Collections.IEnumerator DissolveSequence()
    {
        isRunning = true;

        // 1초간 1 → -1로 선형 감소
        float duration = 1f;
        float timer = 0f;

        while (timer < duration)
        {
            float t = timer / duration; // 0 → 1
            float value = Mathf.Lerp(1f, -1f, t);
            material.SetFloat("_SplitValue", value);
            timer += Time.deltaTime;
            yield return null;
        }

        material.SetFloat("_SplitValue", -1f);

        // 2초 대기
        yield return new WaitForSeconds(2f);

        // 다시 1로 복구
        material.SetFloat("_SplitValue", 1f);

        isRunning = false;
    }
}
