using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIIntroSequence : MonoBehaviour
{
    [SerializeField] private CanvasGroup[] uiElements; // 순차적으로 등장시킬 UI들
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float delayBetween = 0.3f;

    void Start()
    {
        StartCoroutine(ShowUISequence());
    }

    private IEnumerator ShowUISequence()
    {
        foreach (CanvasGroup cg in uiElements)
        {
            yield return StartCoroutine(FadeIn(cg));
            yield return new WaitForSeconds(delayBetween);
        }
    }

    private IEnumerator FadeIn(CanvasGroup cg)
    {
        float t = 0f;
        cg.gameObject.SetActive(true); // 꺼져있던 경우 대비
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }
        cg.alpha = 1f;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }
}
