using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillUIManager : MonoBehaviour
{
    [SerializeField] private SkillManager skillManager;
    [SerializeField] private Image[] skillIcons;
    [SerializeField] private Image[] cooldownFills;
    [SerializeField] private TMP_Text[] cooldownTexts;

    // 디버그 목적의 변수들
    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private Skill[] skills;
    private float[] previousCooldownTimers;

    void Start()
    {
        Debug.Log("[SkillUIManager] Starting initialization...");

        // SkillManager 유효성 확인
        if (skillManager == null)
        {
            Debug.LogError("[SkillUIManager] CRITICAL ERROR: SkillManager is not assigned in the inspector!");
            return;
        }
        else
        {
            Debug.Log("[SkillUIManager] SkillManager reference found successfully.");
        }

        // 각 배열 길이 출력
        if (showDebugLogs)
        {
            Debug.Log($"[SkillUIManager] Array counts - skillIcons: {skillIcons?.Length ?? 0}, cooldownFills: {cooldownFills?.Length ?? 0}, cooldownTexts: {cooldownTexts?.Length ?? 0}");
        }

        // SkillManager가 Skills를 로드하기 전에 참조하면 null이 될 수 있음
        // SkillManager의 Start() 메서드가 먼저 실행되도록 한 프레임 대기
        Invoke("InitializeSkills", 0.1f);
    }

    private void InitializeSkills()
    {
        Debug.Log("[SkillUIManager] Initializing skills...");

        // Skill 배열 설정
        skills = new Skill[]
        {
            skillManager.grenade,
            skillManager.launcherArm,
            skillManager.gravitonSurge,
            skillManager.sandevistan
        };

        // 스킬 참조 로그
        for (int i = 0; i < skills.Length; i++)
        {
            string skillName = (skills[i] != null) ? skills[i].GetType().Name : "NULL";
            Debug.Log($"[SkillUIManager] Skill {i}: {skillName}");
        }

        if (skills.Length != skillIcons.Length || skills.Length != cooldownFills.Length || skills.Length != cooldownTexts.Length)
        {
            Debug.LogError($"[SkillUIManager] CRITICAL ERROR: UI arrays and skill array lengths do not match! Skills: {skills.Length}, Icons: {skillIcons.Length}, Fills: {cooldownFills.Length}, Texts: {cooldownTexts.Length}");
            return;
        }

        // 쿨다운 시각 효과 초기화
        SetupCooldownVisuals();

        previousCooldownTimers = new float[skills.Length];
        for (int i = 0; i < skills.Length; i++)
        {
            if (skills[i] != null)
            {
                // 쿨다운 Fill 이미지 초기 설정
                if (cooldownFills[i] != null)
                {
                    cooldownFills[i].fillAmount = 0f;
                    cooldownFills[i].gameObject.SetActive(false);
                    Debug.Log($"[SkillUIManager] Cooldown fill {i} initialized: fillAmount = 0, active = false");
                }
                else
                {
                    Debug.LogError($"[SkillUIManager] Cooldown fill {i} is NULL!");
                }

                previousCooldownTimers[i] = skills[i].cooldownTimer;
                Debug.Log($"[SkillUIManager] Skill {i} initial cooldown timer: {previousCooldownTimers[i]}");
            }
        }

        Debug.Log("[SkillUIManager] Initialization complete!");
    }

    private void SetupCooldownVisuals()
    {
        Debug.Log("[SkillUIManager] Setting up cooldown visuals...");

        // 쿨다운 Fill 이미지의 Fill Method를 확인하고 설정
        for (int i = 0; i < cooldownFills.Length; i++)
        {
            if (cooldownFills[i] != null)
            {
                // Fill Method를 Radial 360로 설정
                cooldownFills[i].type = Image.Type.Filled;
                cooldownFills[i].fillMethod = Image.FillMethod.Radial360;
                cooldownFills[i].fillClockwise = false; // 반시계 방향 채우기 (시계방향 비우기)
                cooldownFills[i].fillAmount = 0f;

                Debug.Log($"[SkillUIManager] Cooldown fill {i} setup: type = Filled, method = Radial360, clockwise = false");
            }
            else
            {
                Debug.LogError($"[SkillUIManager] Cooldown fill {i} is NULL during setup!");
            }
        }
    }

    void Update()
    {
        // 1초마다 한 번씩 디버그 로그 출력 (성능 저하 방지)
        bool logThisFrame = showDebugLogs && (Time.frameCount % 60 == 0);

        if (skills == null)
        {
            if (logThisFrame) Debug.LogError("[SkillUIManager] Skills array is NULL! Reinitializing...");
            InitializeSkills();
            return;
        }

        for (int i = 0; i < skills.Length; i++)
        {
            var skill = skills[i];
            if (skill == null)
            {
                if (logThisFrame) Debug.LogWarning($"[SkillUIManager] Skill {i} is NULL!");
                continue;
            }

            float timer = skill.cooldownTimer;
            float cooldownDuration = skill.cooldown;

            // 쿨다운이 0인 경우 0으로 나누기 오류 방지
            if (cooldownDuration <= 0)
            {
                cooldownDuration = 1f;
                if (logThisFrame) Debug.LogWarning($"[SkillUIManager] Skill {i} has 0 or negative cooldown duration! Using 1f instead.");
            }

            // 쿨다운 비율 계산 (1 = 쿨다운 시작, 0 = 쿨다운 완료)
            float fillAmount = Mathf.Clamp01(timer / cooldownDuration);

            // 쿨다운 중인지 확인 (약간의 여유를 두어 계산 오류 방지)
            bool isCoolingDown = timer > 0.01f;

            if (logThisFrame)
            {
                Debug.Log($"[SkillUIManager] Skill {i}: timer={timer:F2}, cooldown={cooldownDuration:F2}, ratio={fillAmount:F2}, cooling={isCoolingDown}");
            }

            // 쿨다운 Fill 이미지 업데이트
            if (cooldownFills[i] != null)
            {
                bool previousActive = cooldownFills[i].gameObject.activeSelf;
                cooldownFills[i].gameObject.SetActive(isCoolingDown);

                if (previousActive != isCoolingDown && logThisFrame)
                {
                    Debug.Log($"[SkillUIManager] Cooldown fill {i} visibility changed: {previousActive} -> {isCoolingDown}");
                }

                if (isCoolingDown)
                {
                    float previousFill = cooldownFills[i].fillAmount;
                    cooldownFills[i].fillAmount = fillAmount;

                    if (Mathf.Abs(previousFill - fillAmount) > 0.05f && logThisFrame)
                    {
                        Debug.Log($"[SkillUIManager] Cooldown fill {i} amount changed: {previousFill:F2} -> {fillAmount:F2}");
                    }
                }
                else if (cooldownFills[i].fillAmount != 0f)
                {
                    cooldownFills[i].fillAmount = 0f;
                    if (logThisFrame) Debug.Log($"[SkillUIManager] Cooldown fill {i} reset to 0");
                }
            }
            else
            {
                if (logThisFrame) Debug.LogError($"[SkillUIManager] Cooldown fill {i} is NULL during update!");
            }

            // 쿨다운 시간 텍스트 업데이트
            if (cooldownTexts[i] != null)
            {
                if (isCoolingDown)
                {
                    string newText = Mathf.Ceil(timer).ToString() + "s";
                    if (cooldownTexts[i].text != newText)
                    {
                        cooldownTexts[i].text = newText;
                        if (logThisFrame) Debug.Log($"[SkillUIManager] Cooldown text {i} updated: {newText}");
                    }

                    if (!cooldownTexts[i].gameObject.activeSelf)
                    {
                        cooldownTexts[i].gameObject.SetActive(true);
                        if (logThisFrame) Debug.Log($"[SkillUIManager] Cooldown text {i} activated");
                    }
                }
                else if (cooldownTexts[i].gameObject.activeSelf)
                {
                    cooldownTexts[i].gameObject.SetActive(false);
                    cooldownTexts[i].text = "";
                    if (logThisFrame) Debug.Log($"[SkillUIManager] Cooldown text {i} deactivated");
                }
            }
            else
            {
                if (logThisFrame) Debug.LogError($"[SkillUIManager] Cooldown text {i} is NULL!");
            }

            previousCooldownTimers[i] = timer;
        }
    }

    // Unity 에디터에서 Inspector에서 테스트할 수 있는 디버그 메서드
    public void TestCooldown(int skillIndex, float testCooldownAmount)
    {
        if (skills == null || skillIndex < 0 || skillIndex >= skills.Length)
        {
            Debug.LogError($"[SkillUIManager] Cannot test cooldown: invalid skill index {skillIndex}");
            return;
        }

        if (skills[skillIndex] == null)
        {
            Debug.LogError($"[SkillUIManager] Cannot test cooldown: skill {skillIndex} is NULL");
            return;
        }

        Debug.Log($"[SkillUIManager] Testing cooldown for skill {skillIndex}: setting timer to {testCooldownAmount}");
        skills[skillIndex].cooldownTimer = testCooldownAmount;
    }
}