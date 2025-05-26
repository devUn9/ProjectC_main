using UnityEngine;
using UnityEngine.UI;

public class BgmVolumeSlider : MonoBehaviour
{
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void Start()
    {
        // 저장된 볼륨 값 불러오기
        float savedVolume = PlayerPrefs.GetFloat("BGM_VOLUME", 1f);
        slider.value = savedVolume;

        // 슬라이더 값 변경 시 BGM 볼륨 변경
        slider.onValueChanged.AddListener(OnSliderValueChanged);

        // 초기 볼륨 설정
        if (SoundManager.instance != null)
            SoundManager.instance.audioBgm.volume = savedVolume;
    }

    private void OnSliderValueChanged(float value)
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.audioBgm.volume = value;
            PlayerPrefs.SetFloat("BGM_VOLUME", value);
        }
    }
}
