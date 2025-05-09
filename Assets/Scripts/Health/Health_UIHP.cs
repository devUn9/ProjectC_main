using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.UI;

public class Health_UIHP : MonoBehaviour
{
    [SerializeField] private CharacterStats characterStats;
    private Slider sliderHP;
    [SerializeField] private TextMeshProUGUI textHP;

    // 슬라이더 Lerp 효과 부분
    [SerializeField] private float currentHP;
    [SerializeField] private float displayHP;

    private void Start()
    {
        displayHP = currentHP = (float)characterStats.currentHealth;
    }

    public void UpdateHP(float currentHP, float maxHP)
    {
        if (sliderHP != null)
            sliderHP.value = Health_Utill.Percent(currentHP, maxHP);

        if (textHP != null)
            textHP.text = $"{currentHP:F0} / {maxHP:F0}";
    }

    private void Update()
    {
        // 슬라이더 Lerp 효과
        currentHP = (float)characterStats.currentHealth;
        displayHP = Mathf.Lerp(displayHP, currentHP, Time.deltaTime * 10f);

        if (sliderHP != null)
            sliderHP.value = Health_Utill.Percent((float)characterStats.currentHealth, (float)characterStats.currentHealth);
        if (textHP != null)
            textHP.text = $"{(float)characterStats.currentHealth:F0} / {(float)characterStats.currentHealth:F0}";
    }

    public float Percent(float current, float max)
    {
        return current != 0 && max != 0 ? current / max : 0;
    }
}
