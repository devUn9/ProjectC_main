using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    [Header("플레이어 설정")]
    public Transform player;
    public PlayerStats playerController; // 플레이어 컨트롤러 스크립트

    [Header("체크포인트 설정")]
    public Transform currentCheckpoint;
    public Vector3 initialSpawnPoint = Vector3.zero;

    [Header("리셋 효과")]
    public GameObject respawnEffect;
    public AudioClip respawnSound;
    private AudioSource audioSource;

    public Portal3 portal3;

    [SerializeField] private Transform outPoint; // 플레이어가 도착할 위치
    [SerializeField] private CinemachineVirtualCameraBase virtualCamera; // Cinemachine 가상 카메라
    private CinemachineConfiner2D confiner; // Cinemachine Confiner 2D 컴포넌트
    [SerializeField] private BoxCollider2D targetBoundingShape;

    [SerializeField] private LayerMask mapLayerMask; // 맵이 속한 레이어만 필터링


    void Awake()
    {
        // 싱글톤 패턴
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        // 플레이어가 설정되지 않았다면 자동으로 찾기
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        if (playerController == null)
            playerController = player.GetComponent<PlayerStats>();
    }

    // 체크포인트 설정
    public void SetCheckpoint(Transform checkpointTransform)
    {
        currentCheckpoint = checkpointTransform;
        Debug.Log("체크포인트 설정: " + checkpointTransform.name);
    }

    // 플레이어를 마지막 체크포인트로 리셋
    public void ResetToCheckpoint()
    {
        if (player == null) return;
        Vector3 resetPosition = currentCheckpoint != null ?
            currentCheckpoint.position : initialSpawnPoint;

        StartCoroutine(ResetPlayerCoroutine(resetPosition));
    }

    private IEnumerator ResetPlayerCoroutine(Vector3 resetPosition)
    {
        // 플레이어 비활성화 (움직임 방지)
        //if (playerController != null)
        //    playerController.enabled = false;

        //// 리스폰 이펙트
        //if (respawnEffect != null)
        //{
        //    GameObject effect = Instantiate(respawnEffect, resetPosition, Quaternion.identity);
        //    Destroy(effect, 2f);
        //}

        //// 사운드 재생
        //if (respawnSound != null && audioSource != null)
        //    audioSource.PlayOneShot(respawnSound);

        // 약간의 딜레이
        yield return new WaitForSeconds(0.5f);

        // 플레이어 위치 리셋
        player.position = resetPosition;


        portal3.AssignBoundingShapeFromOutPoint();
        portal3.CinemachineUpdate();
        Debug.Log("카메라 리셋");

        //if (virtualCamera != null)
        //{
        //    confiner = virtualCamera.GetComponent<CinemachineConfiner2D>();
        //    if (confiner == null)
        //        Debug.LogError("CinemachineConfiner2D 컴포넌트가 가상 카메라에 없습니다!", virtualCamera);
        //    virtualCamera.Follow = player.transform;
        //}
        //else
        //{
        //    Debug.LogError("Virtual Camera가 지정되지 않았습니다!", this);
        //}

        Collider2D hit = Physics2D.OverlapPoint(outPoint.position, mapLayerMask);
        if (hit != null && hit is BoxCollider2D box)
        {
            targetBoundingShape = box;
            //Debug.Log($"타겟 바운딩 셰이프 자동 할당: {box.name}");
        }
        else
        {
            //Debug.LogWarning("outPoint 위치에서 BoxCollider2D를 찾을 수 없습니다.", this);
        }






        // 2D 물리엔진 사용 시
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = 0f;
        }
        playerController.currentHealth = playerController.maxHealth.GetValue();

        Debug.Log("플레이어가 체크포인트로 리셋되었습니다.");
    }
}
