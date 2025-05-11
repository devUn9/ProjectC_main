using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillCoolController : MonoBehaviour
{
    [SerializeField] private SkillManager skillManager; // SkillManager 참조
    [SerializeField] private GameObject[] hideSkillButtons; // 스킬 버튼 UI
    [SerializeField] private GameObject[] textPros; // 텍스트 오브젝트
    [SerializeField] private TextMeshProUGUI[] hideSkillTimeTexts; // 쿨타임 텍스트
    [SerializeField] private Image[] hideSkillImages; // 쿨타임 이미지

    private Skill[] skills; // SkillManager에서 가져온 스킬 배열
    private bool[] isHideSkills; // 스킬 활성화 여부
    private float[] skillTimes; // 스킬별 쿨타임
    private float[] getSkillTimes; // 현재 남은 쿨타임
    private bool[] isCoroutineRunning; // 코루틴 실행 여부
    private float rotationSpeed = 360f; // 이미지 회전 속도 (도/초)

    private void Start()
    {
        // SkillManager 확인
        if (skillManager == null)
        {
            Debug.LogError("SkillManager is not assigned in the Inspector!");
            return;
        }

        // 스킬 배열 초기화
        skills = new Skill[]
        {
            skillManager.grenade,
            skillManager.launcherArm,
            skillManager.gravitonSurge,
            skillManager.sandevistan
        };

        // 스킬 null 체크
        for (int i = 0; i < skills.Length; i++)
        {
            if (skills[i] == null)
            {
                Debug.LogError($"Skill at index {i} is null in SkillManager!");
            }
        }

        // 배열 초기화
        isHideSkills = new bool[skills.Length];
        skillTimes = new float[skills.Length];
        getSkillTimes = new float[skills.Length];
        isCoroutineRunning = new bool[skills.Length];

        // 스킬 쿨타임 설정
        for (int i = 0; i < skills.Length; i++)
        {
            if (skills[i] != null)
            {
                
                Debug.Log($"Skill {i} cooldown: {skillTimes[i]}");
            }
            else
            {
                skillTimes[i] = 1f; // 기본 쿨타임
            }
            isHideSkills[i] = false;
            getSkillTimes[i] = 0f;
            isCoroutineRunning[i] = false;
        }

        // UI 초기화 및 검증
        if (textPros.Length != skills.Length || hideSkillButtons.Length != skills.Length || hideSkillImages.Length != skills.Length)
        {
            Debug.LogError($"UI arrays length mismatch! Expected: {skills.Length}, Got: Buttons={hideSkillButtons.Length}, Texts={textPros.Length}, Images={hideSkillImages.Length}");
        }

        for (int i = 0; i < textPros.Length; i++)
        {
            if (textPros[i] == null || hideSkillButtons[i] == null || hideSkillImages[i] == null)
            {
                Debug.LogError($"UI element at index {i} is null!");
                continue;
            }
            hideSkillTimeTexts[i] = textPros[i].GetComponent<TextMeshProUGUI>();
            if (hideSkillTimeTexts[i] == null)
            {
                Debug.LogError($"TextMeshProUGUI at index {i} is missing!");
            }
            hideSkillButtons[i].SetActive(false);
            hideSkillImages[i].fillAmount = 0;


            hideSkillImages[i].fillAmount = 0;
        }
    }

    private void Update()
    {
        HiddenSkillChk();
    }

    // 스킬 사용 시 호출
    public void HideSkillSetting(int skillNum)
    {
        if (skillNum < 0 || skillNum >= skills.Length)
        {
            Debug.LogWarning($"Invalid skill index: {skillNum}");
            return;
        }

        Debug.Log($"HideSkillSetting called for skill {skillNum}, cooldown: {skillTimes[skillNum]}");
        hideSkillButtons[skillNum].SetActive(true);
        getSkillTimes[skillNum] = skillTimes[skillNum];
        isHideSkills[skillNum] = true;
        hideSkillImages[skillNum].fillAmount = 1;
        hideSkillImages[skillNum].transform.rotation = Quaternion.identity;
    }

    // 스킬 인덱스 조회
    public int GetSkillIndex(Skill skill)
    {
        for (int i = 0; i < skills.Length; i++)
        {
            if (skills[i] == skill)
                return i;
        }
        Debug.LogWarning($"Skill {skill.name} not found in skills array!");
        return -1;
    }

    private void HiddenSkillChk()
    {
        for (int i = 0; i < isHideSkills.Length; i++)
        {
            if (isHideSkills[i] && !isCoroutineRunning[i])
            {
                StartCoroutine(SkillTimeChk(i));
                isCoroutineRunning[i] = true;
            }
        }
    }

    private IEnumerator SkillTimeChk(int skillNum)
    {
        Debug.Log($"SkillTimeChk started for skill {skillNum}");
        while (getSkillTimes[skillNum] > 0)
        {
            getSkillTimes[skillNum] -= Time.deltaTime;
            if (getSkillTimes[skillNum] <= 0)
            {
                getSkillTimes[skillNum] = 0;
                isHideSkills[skillNum] = false;
                hideSkillButtons[skillNum].SetActive(false);
                hideSkillImages[skillNum].fillAmount = 0;
                hideSkillImages[skillNum].transform.rotation = Quaternion.identity;
                isCoroutineRunning[skillNum] = false;
            }

            hideSkillTimeTexts[skillNum].text = Mathf.CeilToInt(getSkillTimes[skillNum]).ToString();
            float timeRatio = getSkillTimes[skillNum] / skillTimes[skillNum];
            hideSkillImages[skillNum].fillAmount = timeRatio;
            hideSkillImages[skillNum].transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

            yield return null;
        }
        isCoroutineRunning[skillNum] = false;
        Debug.Log($"SkillTimeChk ended for skill {skillNum}");
    }
}