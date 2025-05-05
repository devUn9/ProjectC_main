using NUnit.Framework;
using UnityEngine;

public class GrenadeSkill : Skill
{
    [Header("Grenade Info")]
    public GameObject smokeGrenadePrefab;   // 연막 수류탄 프리팹
    public GameObject handGrenadePrefab;    // 파쇄 수류탄 프리팹
    public GameObject empGrenadePrefab;     // EMP 수류탄 프리팹

    public Transform throwPosition;         // 던지는 위치 (플레이어)
    private GameObject selectedGrenadePrefab;
    public bool getKeyLock; //키 입력 Lock

    //폭발 범위 시각화
    [Header("Explode Range Info")]
    [SerializeField] private GameObject rangePrefab;
    [SerializeField] private Transform rangeTransform;
    private GameObject range;

    protected override void Start()
    {
        base.Start();
        GenerateRange();
        getKeyLock = true;
    }

    protected override void Update()
    {
        base.Update();

        if(!CanUseBool())
            return;

        if(getKeyLock)
            return;
        Debug.Log("getKeyLock : "+getKeyLock);
        if ((Input.GetKey(KeyCode.Alpha1)
            || Input.GetKey(KeyCode.Alpha2)
            || Input.GetKey(KeyCode.Alpha3)))
        {
            ModifyRange();
            range.transform.position = MousePosition();
        }
        if (Input.GetKeyUp(KeyCode.Alpha1)
            || Input.GetKeyUp(KeyCode.Alpha2)
            || Input.GetKeyUp(KeyCode.Alpha3))
        {
            RangeActive(false);
        }

        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            if (!CanUseSkill())
                return;
            selectedGrenadePrefab = smokeGrenadePrefab;
            isKeyProcessing = true;
            ThrowGrenade();
            getKeyLock = true;
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            if (!CanUseSkill())
                return;
            selectedGrenadePrefab = handGrenadePrefab;
            isKeyProcessing = true;
            ThrowGrenade();
            getKeyLock = true;
        }
        else if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            if (!CanUseSkill())
                return;
            selectedGrenadePrefab = empGrenadePrefab;
            isKeyProcessing = true;
            ThrowGrenade();
            getKeyLock = true;
        }
    }

    // 수류탄 던지기
    private void ThrowGrenade()
    {
        // 마우스 위치를 월드 좌표로 변환
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 2f; // 카메라와의 거리
        Vector3 targetPosition = mainCamera.ScreenToWorldPoint(mousePosition);

        // 플레이어 위치 및 던지는 위치 설정
        Vector3 throwPos = throwPosition != null ? throwPosition.position : transform.position;

        // 수류탄 생성 및 초기화
        GameObject grenadeObj = Instantiate(selectedGrenadePrefab, throwPos, Quaternion.identity);
        GrenadeController grenadeController = grenadeObj.GetComponent<GrenadeController>();

        if (grenadeController != null)
        {
            grenadeController.Initialize(throwPos, targetPosition, SkillManager.instance.playerStats);
        }
        isKeyProcessing = false;
    }

    // 수류탄 범위 시각화 최초 생성
    public void GenerateRange()
    {
        float radius = 5f;
        rangePrefab.transform.localScale = new Vector3(radius, radius, 2);
        SpriteRenderer sr = rangePrefab.GetComponent<SpriteRenderer>();
        sr.color = new Color(227f, 0f, 0f, 0.15f);

        range = Instantiate(rangePrefab, rangeTransform.position, Quaternion.identity);
        range.SetActive(false);
    }

    //입력받은 Grenade에 따라 범위 및 색상 변경 (범위는 GrenadeController에서 가져옴)
    public void ModifyRange()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            selectedGrenadePrefab = smokeGrenadePrefab;
            range.GetComponent<SpriteRenderer>().color = new Color(20f / 255f, 111f / 255f, 65f / 255f, 0.15f);
        }
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            selectedGrenadePrefab = handGrenadePrefab;
            range.GetComponent<SpriteRenderer>().color = new Color(183f / 255f, 80f / 255f, 74f / 255f, 0.15f);
        }
        else if (Input.GetKey(KeyCode.Alpha3))
        {
            selectedGrenadePrefab = empGrenadePrefab;
            range.GetComponent<SpriteRenderer>().color = new Color(73f / 255f, 111f / 255f, 183f / 255f, 0.15f);
        }

        float radius = selectedGrenadePrefab.GetComponent<GrenadeController>().explosionRadius * 2f;
        range.transform.localScale = new Vector3(radius, radius, 2);
    }

    public void RangeActive(bool _isActive)
    {
        range.SetActive(_isActive);
    }

    
}
