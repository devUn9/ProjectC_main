using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillUIManager : MonoBehaviour
{
    [SerializeField] private SkillManager skillManager;
    [SerializeField] private Image[] skillIcons;
    [SerializeField] private Image[] cooldownFills;
    [SerializeField] private TMP_Text[] cooldownTexts;
    [SerializeField] private Canvas[] skillIconCanvases;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private Skill[] skills;
    private float[] previousCooldownTimers;
    private int[] skillIndexMap; // UI 인덱스를 스킬 인덱스에 매핑: [0, 0, 0, 1, 2, 3]

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

        // 실제 스킬 배열 (4개)
        skills = new Skill[]
        {
            skillManager.grenade,
            skillManager.launcherArm,
            skillManager.gravitonSurge,
            skillManager.sandevistan
        };

        // UI 슬롯과 스킬 인덱스 매핑 (6개 UI 슬롯)
        skillIndexMap = new int[] { 0, 0, 0, 1, 2, 3 }; // 첫 3개는 grenade (인덱스 0)

        // 배열 길이 검증
        if (skillIcons.Length != 6 || cooldownFills.Length != 6 || cooldownTexts.Length != 6 || skillIconCanvases.Length != 6)
        {
            Debug.LogError($"[SkillUIManager] CRITICAL ERROR: UI arrays must have length 6! Icons: {skillIcons.Length}, Fills: {cooldownFills.Length}, Texts: {cooldownTexts.Length}, Canvases: {skillIconCanvases.Length}");
            return;
        }

        SetupCooldownVisuals();
        UpdateSkillCanvasVisibility();

        previousCooldownTimers = new float[skillIcons.Length]; // UI 슬롯 수(6)에 맞춤
        for (int i = 0; i < skillIcons.Length; i++)
        {
            int skillIndex = skillIndexMap[i];
            if (skills[skillIndex] != null)
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

                previousCooldownTimers[i] = skills[skillIndex].cooldownTimer;
                Debug.Log($"[SkillUIManager] Skill {i} (mapped to skill {skillIndex}) initial cooldown timer: {previousCooldownTimers[i]}");
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

    private void UpdateSkillCanvasVisibility()
    {
        if (skillIconCanvases == null || skillIconCanvases.Length != 6)
        {
            Debug.LogError("[SkillUIManager] skillIconCanvases array is null or not length 6!");
            return;
        }

        // 스킬 사용 가능 여부
        bool[] skillUsableStates = new bool[]
        {
            true, // grenade는 항상 활성화
            skillManager.isLauncherArmUsable,
            skillManager.isGravitonUsable,
            skillManager.isSandevistanUsable
        };

        for (int i = 0; i < skillIconCanvases.Length; i++) // UI 슬롯 수(6)에 맞게 루프
        {
            int skillIndex = skillIndexMap[i];
            if (skillIconCanvases[i] != null)
            {
                bool isActive = skillUsableStates[skillIndex];
                if (skillIconCanvases[i].enabled != isActive)
                {
                    skillIconCanvases[i].enabled = isActive;
                    if (showDebugLogs)
                    {
                        Debug.Log($"[SkillUIManager] Skill canvas {i} (mapped to skill {skillIndex}: {skills[skillIndex]?.GetType().Name ?? "NULL"}) set to {(isActive ? "active" : "inactive")}");
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

        UpdateSkillCanvasVisibility();

        for (int i = 0; i < skillIcons.Length; i++) // UI 슬롯 수(6)에 맞게 루프
        {
            int skillIndex = skillIndexMap[i];
            var skill = skills[skillIndex];
            if (skill == null)
            {
                if (logThisFrame) Debug.LogWarning($"[SkillUIManager] Skill {skillIndex} is NULL!");
                continue;
            }

            float timer = skill.cooldownTimer;
            float cooldownDuration = skill.cooldown;

            if (cooldownDuration <= 0)
            {
                cooldownDuration = 1f;
                if (logThisFrame) Debug.LogWarning($"[SkillUIManager] Skill {skillIndex} has 0 or negative cooldown duration! Using 1f instead.");
            }

            float fillAmount = Mathf.Clamp01(timer / cooldownDuration);
            bool isCoolingDown = timer > 0.01f;

            if (logThisFrame)
            {
                Debug.Log($"[SkillUIManager] Skill {i} (mapped to {skillIndex}): timer={timer:F2}, cooldown={cooldownDuration:F2}, ratio={fillAmount:F2}, cooling={isCoolingDown}");
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