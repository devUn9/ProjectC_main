using UnityEngine;

public class LauncherArmSkill : Skill
{
    [Header("Launcher Arm Info")]
    public GameObject launcherMissilePrefab;    // 발사체 프리팹
    public Transform launchStartPosition;       // 발사 위치 (플레이어)
    public Vector2 direction;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        
        //if (Input.GetKeyUp(KeyCode.Q) && getInProcess)
        if (getInProcess)
        {
            if (!CanUseSkill())
                return;
            LaunchMissile();
            GetOutProcessCheck();
            SkillCoolDownReset();
        }
    }

    private void LaunchMissile()
    {
        Vector3 launchPosition = launchStartPosition.position;

        SoundManager.instance.PlayESFX(SoundManager.ESfx.SFX_LauncherArmExplosion);

        GameObject missile = Instantiate(launcherMissilePrefab, launchPosition, Quaternion.identity);
        LauncherMissileController missileController = missile.GetComponent<LauncherMissileController>();
        if (missileController != null)
        {
            missileController.Initialize(launchPosition, SkillManager.instance.playerStats);
        }
    }
}
