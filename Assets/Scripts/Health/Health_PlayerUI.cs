using UnityEngine;
using UnityEngine.UI;

public class Health_PlayerUI : MonoBehaviour
{
    [SerializeField] private Health_Entity healthEntity;
    [SerializeField] private Slider sliderHP;
    [SerializeField] private Health_UIHP hudUI; // 좌상단 UI 연결

    private void Update()
    {
        float current = healthEntity.HP;
        float max = healthEntity.MaxHP;

        // 본인 UI 업데이트
        if (sliderHP != null)
            sliderHP.value = Health_Utill.Percent(current, max);

        // HUD UI도 같이 업데이트
        if (hudUI != null)
            hudUI.UpdateHP(current, max);
    }
}
