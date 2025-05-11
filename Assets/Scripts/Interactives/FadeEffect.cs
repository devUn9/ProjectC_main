using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeEffect : MonoBehaviour
{
    public Image fadeImage; // 검은색 Image UI
    public float fadeDuration = 1f; // 페이드 시간

    public void FadeOut(System.Action onComplete)
    {
        StartCoroutine(Fade(1f, onComplete));
    }

    public void FadeIn(System.Action onComplete)
    {
        StartCoroutine(Fade(0f, onComplete));
    }

    private IEnumerator Fade(float targetAlpha, System.Action onComplete)
    {
        float startAlpha = fadeImage.color.a;
        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, targetAlpha);
        onComplete?.Invoke();
    }
}