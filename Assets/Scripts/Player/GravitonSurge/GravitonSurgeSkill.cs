using UnityEngine;

public class GravitonSurgeSkill : Skill
{
    [Header("Graviton Surge Info")]
    [SerializeField] private GameObject GravitonSurgePrefab; // 중력파 프리팹

    public Transform launchPosition;         // 발사 위치 (플레이어)

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        
        if (!CanUseBool())
            return;
        if (Input.GetKeyUp(KeyCode.E)&& getInProcess)
        {
            LaunchGravitonSurge();
            GetOutProcessCheck();
            SkillCoolDownReset();
        }
    }

    private void LaunchGravitonSurge()
    {
        // 마우스 위치를 월드 좌표로 변환
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 2f; // 카메라와의 거리
        Vector3 targetPosition = mainCamera.ScreenToWorldPoint(mousePosition);

        GameObject gravitonSurge = Instantiate(GravitonSurgePrefab, launchPosition.transform.position, Quaternion.identity);
        GravitonSurgeController gravitonSurgeController = gravitonSurge.GetComponent<GravitonSurgeController>();

        if (gravitonSurgeController != null)
        {
            gravitonSurgeController.Initialize(launchPosition.transform.position , targetPosition, SkillManager.instance.playerStats);
        }
    }
}
