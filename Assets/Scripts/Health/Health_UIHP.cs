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

    public void UpdateHP(float currentHP, float maxHP)
    {
        if (sliderHP != null)
            sliderHP.value = Health_Utill.Percent(currentHP, maxHP);

        if (textHP != null)
            textHP.text = $"{currentHP:F0} / {maxHP:F0}";
    }

    private void Update()
    {
        if (sliderHP != null)
            sliderHP.value = Health_Utill.Percent(health_entity.HP, health_entity.MaxHP);
        if (textHP != null)
            textHP.text = $"{health_entity.HP:F0} / {health_entity.MaxHP:F0}";
    }
}
