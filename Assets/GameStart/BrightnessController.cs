using UnityEngine;
using UnityEngine.UI;

public class BrightnessController : MonoBehaviour
{
    [SerializeField] private Image brightnessOverlay; // Overlay 패널의 이미지
    [SerializeField] private Slider lightSlider;      // 밝기 조절 슬라이더

    private void Start()
    {
        if (lightSlider != null)
        {
            lightSlider.onValueChanged.AddListener(SetBrightness);
            SetBrightness(lightSlider.value); // 초기 값 적용
        }
    }

    public void SetBrightness(float value)
    {
        if (brightnessOverlay == null)
            return;

        // 밝기 조절: 1 → 완전 밝음, 0 → 어두움
        float alpha = 1f - value;
        Color c = brightnessOverlay.color;
        c.a = Mathf.Clamp01(alpha);
        brightnessOverlay.color = c;

        Debug.Log($"밝기 조절: {value} → 알파: {alpha}");
    }
}
