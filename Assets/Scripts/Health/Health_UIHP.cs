using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.UI;

public class Health_UIHP : MonoBehaviour
{
    [SerializeField] private Health_Entity health_entity;
    [SerializeField] private Slider sliderHP;
    [SerializeField] private TextMeshProUGUI textHP;

    // 슬라이더 Lerp 효과 부분
    [SerializeField] private float currentHP;
    [SerializeField] private float displayHP;

    private void Start()
    {
        displayHP = currentHP = health_entity.HP;
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
        currentHP = health_entity.HP;
        displayHP = Mathf.Lerp(displayHP, currentHP, Time.deltaTime * 10f);

        if (sliderHP != null)
            sliderHP.value = Health_Utill.Percent(health_entity.HP, health_entity.MaxHP);
        if (textHP != null)
            textHP.text = $"{health_entity.HP:F0} / {health_entity.MaxHP:F0}";
    }
}
