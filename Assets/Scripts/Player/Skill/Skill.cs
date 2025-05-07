using UnityEngine;

public class Skill : MonoBehaviour
{
    [SerializeField] protected float cooldown;
    [SerializeField] protected float cooldownTimer;

    protected Camera mainCamera;

    // 키 입력 상태 관리를 위한 변수들
    protected bool isKeyProcessing = false;   // 현재 키 처리 중인지 여부
    protected bool getInProcess = false;         // 키가 눌렸는지 여부
    protected virtual void Start()
    {
        mainCamera = Camera.main;
    }
    protected virtual void Update()
    {
        cooldownTimer -= Time.deltaTime;
    }

    public virtual bool CanUseSkill()
    {
        if (cooldownTimer < 0)
        {
            cooldownTimer = cooldown;
            return true;
        }
        return false;
    }

    public Vector3 MousePosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 2f; // 카메라와의 거리
        Vector3 targetPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        return targetPosition;
    }

    public bool CanUseBool()
    {
        if (cooldownTimer < 0)
            return true;

        return false;
    }

    public bool GetInProcessCheck()
    {
        return getInProcess = true;
    }

    public bool GetOutProcessCheck()
    {
        return getInProcess = false;
    }
}
