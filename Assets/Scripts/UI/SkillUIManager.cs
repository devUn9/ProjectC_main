using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillUIManager : MonoBehaviour
{
    [SerializeField] private SkillManager skillManager;
    [SerializeField] private Image[] skillIcons;
    [SerializeField] private Image[] cooldownFills;
    [SerializeField] private TMP_Text[] cooldownTexts;
    // 추가: 스킬 아이콘 캔버스 배열
    [SerializeField] private Canvas[] skillIconCanvases;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private Skill[] skills;
    private float[] previousCooldownTimers;

    void Start()
    {
        Debug.Log("[SkillUIManager] Starting initialization...");

        if (skillManager == null)
        {
            Debug.LogError("[SkillUIManager] CRITICAL ERROR: SkillManager is not assigned in the inspector!");
            return;
        }
        else
        {
            Debug.Log("[SkillUIManager] SkillManager reference found successfully.");
        }

        if (showDebugLogs)
        {
            Debug.Log($"[SkillUIManager] Array counts - skillIcons: {skillIcons?.Length ?? 0}, cooldownFills: {cooldownFills?.Length ?? 0}, cooldownTexts: {cooldownTexts?.Length ?? 0}, skillIconCanvases: {skillIconCanvases?.Length ?? 0}");
        }

        Invoke("InitializeSkills", 0.1f);
    }

    private void InitializeSkills()
    {
        Debug.Log("[SkillUIManager] Initializing skills...");

        skills = new Skill[]
        {
            skillManager.grenade,
            skillManager.launcherArm,
            skillManager.gravitonSurge,
            skillManager.sandevistan
        };

        for (int i = 0; i < skills.Length; i++)
        {
            string skillName = (skills[i] != null) ? skills[i].GetType().Name : "NULL";
            Debug.Log($"[SkillUIManager] Skill {i}: {skillName}");
        }

        // 배열 길이 검증에 skillIconCanvases 추가
        if (skills.Length != skillIcons.Length || skills.Length != cooldownFills.Length ||
            skills.Length != cooldownTexts.Length || skills.Length != skillIconCanvases.Length)
        {
            Debug.LogError($"[SkillUIManager] CRITICAL ERROR: UI arrays and skill array lengths do not match! Skills: {skills.Length}, Icons: {skillIcons.Length}, Fills: {cooldownFills.Length}, Texts: {cooldownTexts.Length}, Canvases: {skillIconCanvases.Length}");
            return;
        }

        SetupCooldownVisuals();
        // 추가: 초기 캔버스 상태 설정
        UpdateSkillCanvasVisibility();

        previousCooldownTimers = new float[skills.Length];
        for (int i = 0; i < skills.Length; i++)
        {
            if (skills[i] != null)
            {
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

        for (int i = 0; i < cooldownFills.Length; i++)
        {
            if (cooldownFills[i] != null)
            {
                cooldownFills[i].type = Image.Type.Filled;
                cooldownFills[i].fillMethod = Image.FillMethod.Radial360;
                cooldownFills[i].fillClockwise = false;
                cooldownFills[i].fillAmount = 0f;

                Debug.Log($"[SkillUIManager] Cooldown fill {i} setup: type = Filled, method = Radial360, clockwise = false");
            }
            else
            {
                Debug.LogError($"[SkillUIManager] Cooldown fill {i} is NULL during setup!");
            }
        }
    }

    // 추가: 스킬 캔버스 가시성 업데이트 메서드
    private void UpdateSkillCanvasVisibility()
    {
        if (skillIconCanvases == null || skillIconCanvases.Length < skills.Length)
        {
            Debug.LogError("[SkillUIManager] skillIconCanvases array is null or too short!");
            return;
        }

        // 각 스킬의 잠금 상태에 따라 캔버스 활성화/비활성화
        bool[] skillUsableStates = new bool[]
        {
            true, // grenade는 잠금 변수 없으므로 항상 활성화
            skillManager.isLauncherArmUsable,
            skillManager.isGravitonUsable,
            skillManager.isSandevistanUsable
        };

        for (int i = 0; i < skills.Length; i++)
        {
            if (skillIconCanvases[i] != null)
            {
                bool isActive = skillUsableStates[i];
                if (skillIconCanvases[i].enabled != isActive)
                {
                    skillIconCanvases[i].enabled = isActive;
                    if (showDebugLogs)
                    {
                        Debug.Log($"[SkillUIManager] Skill canvas {i} ({skills[i]?.GetType().Name ?? "NULL"}) set to {(isActive ? "active" : "inactive")}");
                    }
                }
            }
            else
            {
                Debug.LogError($"[SkillUIManager] Skill canvas {i} is NULL!");
            }
        }
    }

    void Update()
    {
        bool logThisFrame = showDebugLogs && (Time.frameCount % 60 == 0);

        if (skills == null)
        {
            if (logThisFrame) Debug.LogError("[SkillUIManager] Skills array is NULL! Reinitializing...");
            InitializeSkills();
            return;
        }

        // 추가: 매 프레임 캔버스 가시성 업데이트
        UpdateSkillCanvasVisibility();

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

            if (cooldownDuration <= 0)
            {
                cooldownDuration = 1f;
                if (logThisFrame) Debug.LogWarning($"[SkillUIManager] Skill {i} has 0 or negative cooldown duration! Using 1f instead.");
            }

            float fillAmount = Mathf.Clamp01(timer / cooldownDuration);
            bool isCoolingDown = timer > 0.01f;

            if (logThisFrame)
            {
                Debug.Log($"[SkillUIManager] Skill {i}: timer={timer:F2}, cooldown={cooldownDuration:F2}, ratio={fillAmount:F2}, cooling={isCoolingDown}");
            }

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

            if (cooldownTexts[i] != null)
            {
                if (isCoolingDown)
                {
                    string newText = Mathf.Ceil(timer).ToString();
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

}