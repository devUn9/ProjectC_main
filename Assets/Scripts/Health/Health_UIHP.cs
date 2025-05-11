using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Health_UIHP : MonoBehaviour
{
    [SerializeField] private CharacterStats characterStats;
    [SerializeField] private Slider sliderHP;
    [SerializeField] private TextMeshProUGUI textHP;

    // Lerp 효과용 변수
    private float currentHP;
    private float displayHP;

    private void Awake()
    {
        // 슬라이더 컴포넌트 자동 할당 (필요한 경우)
        if (sliderHP == null)
        {
            sliderHP = GetComponent<Slider>();
            if (sliderHP == null)
            {
                Debug.LogError("Slider component not found on " + gameObject.name);
            }
        }
    }

    private void Start()
    {
        if (characterStats != null)
        {
            currentHP = displayHP = (float)characterStats.currentHealth;
            UpdateHP(currentHP, (float)characterStats.maxHealth.GetValue());
        }
    }

    public void UpdateHP(float currentHP, float maxHP)
    {
        this.currentHP = currentHP;
        // 즉시 텍스트 업데이트
        if (textHP != null)
        {
            textHP.text = $"{currentHP:F0} / {maxHP:F0}";
        }
    }

    private void Update()
    {
        if (characterStats == null || sliderHP == null) return;

        // Lerp 효과로 displayHP 업데이트
        displayHP = Mathf.Lerp(displayHP, currentHP, Time.deltaTime * 10f);

        // 슬라이더 업데이트
        float maxHP = (float)characterStats.maxHealth.GetValue();
        sliderHP.value = Health_Utill.Percent(displayHP, maxHP);

        // 텍스트에 Lerp된 값 반영 (선택 사항)
        if (textHP != null)
        {
            textHP.text = $"{displayHP:F0} / {maxHP:F0}";
        }
    }
}