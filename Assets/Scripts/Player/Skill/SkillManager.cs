using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;
    public PlayerStats playerStats;
    public GrenadeSkill grenade { get; private set; }
    public LauncherArmSkill launcherArm { get; private set; }

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
    }
}
