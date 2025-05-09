using UnityEngine;
using UnityEngine.UI;

public class Health_EnemyUI : MonoBehaviour
{
    [SerializeField] private CharacterStats characterstats;
    [SerializeField] private Slider sliderHP;

    
    void Update()
    {
        float current = (float)characterstats.currentHealth;
        float max = (float)characterstats.maxHealth.GetValue();

        // 본인 UI 업데이트
        if (sliderHP != null)
            sliderHP.value = Health_Utill.Percent(current, max);

    }
    
    public float Percent(float current, float max)
    {
        return current != 0 && max != 0 ? current / max : 0;
    }
}
