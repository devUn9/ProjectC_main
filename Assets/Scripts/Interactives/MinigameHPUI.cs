using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MinigameHPUI : MonoBehaviour
{
    [SerializeField] private MinigameObject minigameObject;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI hpText;

    private void Start()
    {
        gameObject.SetActive(false);

        if (minigameObject != null)
        {
            hpSlider.minValue = 0;
            hpSlider.maxValue = minigameObject.MaxHP;
            hpSlider.value = minigameObject.HP;
            //UpdateHPText();
        }
    }

    public void UpdateHPUI()
    {
        if (hpSlider != null)
        {
            hpSlider.value = minigameObject.HP;
        }
        //UpdateHPText();
    }

    //private void UpdateHPText()
    //{
    //    if (hpText != null && minigameObject != null)
    //    {
    //        hpText.text = $"HP: {minigameObject.HP}/{minigameObject.MaxHP}";
    //    }
    //}
}