using UnityEngine;

public class WalkSound : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [Header("검사 주기?")]
    [SerializeField] private float movementCheckInterval = 1f; // 검사 주기
    [Header("검사 이동거리?")]
    [SerializeField] private float minDistanceForSound = 1.5f;   // 검사 주기 내 최소 이동 거리
    [Header("소리 최소 간격")]
    [SerializeField] private float soundInterval = 0.4f;         // 발소리 간 최소 간격

    private Vector3 lastPosition;
    private float accumulatedDistance = 0f;
    private float checkTimer = 0f;
    private float soundCooldown = 0f;
    private bool wasMoving = false;

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError("플레이어 오브젝트가 할당되지 않았습니다.");
            enabled = false;
            return;
        }

        lastPosition = player.transform.position;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            ResetTracking();
            return;
        }

        Vector3 currentPosition = player.transform.position;
        float deltaDistance = Vector3.Distance(currentPosition, lastPosition);
        bool isMoving = deltaDistance > 0.001f;

        accumulatedDistance += deltaDistance;
        checkTimer += Time.deltaTime;
        soundCooldown -= Time.deltaTime;

        //  걷기 시작 순간이면 즉시 발소리 1회
        if (isMoving && !wasMoving && soundCooldown <= 0f)
        {
            PlayWalkSound();
            soundCooldown = soundInterval;
            accumulatedDistance = 0f;
            checkTimer = 0f;
        }

        //  검사 주기마다 판단
        if (checkTimer >= movementCheckInterval)
        {
            if (accumulatedDistance >= minDistanceForSound && soundCooldown <= 0f)
            {
                PlayWalkSound();
                soundCooldown = soundInterval;
            }

            accumulatedDistance = 0f;
            checkTimer = 0f;
        }

        lastPosition = currentPosition;
        wasMoving = isMoving;
    }

    private void PlayWalkSound()
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayESFX(SoundManager.ESfx.SFX_PlayerWalking);
        }
        else
        {
            Debug.LogError("SoundManager.instance is null!");
        }
    }

    private void ResetTracking()
    {
        accumulatedDistance = 0f;
        checkTimer = 0f;
        soundCooldown = 0f;
        wasMoving = false;
        lastPosition = player.transform.position;
    }
}
