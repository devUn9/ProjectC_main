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
    public Portal3 portalInfo;

    [Header("리셋 효과")]
    public GameObject respawnEffect;
    public AudioClip respawnSound;
    private AudioSource audioSource;

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
    public void SetCheckpoint(Transform checkpointTransform, Portal3 portal)
    {
        portalInfo = portal;
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
        yield return new WaitForSeconds(0.6f);
        portalInfo.MovePlayer();

        //portalInfo.CinemachineUpdate();


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
