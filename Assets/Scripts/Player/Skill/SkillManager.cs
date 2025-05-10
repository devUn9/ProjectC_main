using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [Header("Skill Lock Controller")]
    public bool isLauncherArmUsable = false;    // 스킬 사용 잠금 상태
    public bool isGravitonUsable = false;    // 스킬 사용 잠금 상태
    public bool isSandevistanUsable = false;    // 스킬 사용 잠금 상태

    public static SkillManager instance;
    public PlayerStats playerStats;
    public GrenadeSkill grenade { get; private set; }
    public LauncherArmSkill launcherArm { get; private set; }
    public GravitonSurgeSkill gravitonSurge { get; private set; }
    public SandevistanSkill sandevistan { get; private set; }

    public void Initialize(PlayerStats _playerStats)
    {
        playerStats = _playerStats;
    }

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    private void Start()
    {
        grenade = GetComponent<GrenadeSkill>();
        launcherArm = GetComponent<LauncherArmSkill>();
        gravitonSurge = GetComponent<GravitonSurgeSkill>();
        sandevistan = GetComponent<SandevistanSkill>();
    }
}
