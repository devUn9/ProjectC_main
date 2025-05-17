using UnityEngine;

public class GravitonSurgeController : MonoBehaviour
{
    [Header("Graviton Surge Info")]
    public float explodeRadius;           // 중력파의 반지름
    public float surgeDuration;         // 중력파의 지속 시간
    public float gravitonSurgeSpeed;    // 중력파의 속도
    public Vector2 direction;           // 중력파의 방향
    [SerializeField] private GameObject explodeEffectPrefab; // 중력파 이펙트 프리팹
                                                          
    public float disappearDelay;    // 중력파가 사라지기까지의 시간

    [Header("Sound")]
    public AudioClip throwSound;        // 날아가는 소리
    public AudioClip explosionSound;    // 폭발 소리

    private PlayerStats playerStats;    // 플레이어 스탯
    private Vector3 targetPosition;     // 목표 위치
    private Vector3 explosionPoint;     // 폭발 위치
    private Rigidbody2D rb;

    public void Initialize(Vector3 startPosition, Vector3 target, PlayerStats _playerStats)
    {
        transform.position = startPosition;
        targetPosition = target;
        playerStats = _playerStats;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // 사운드 재생
        if (throwSound != null && GetComponent<AudioSource>() != null)
        {
            GetComponent<AudioSource>().PlayOneShot(throwSound);
        }
    }

    private void Start()
    {
        direction = targetPosition - transform.position;
    }

    private void Update()
    {
        MoveSurge();
    }

    private void MoveSurge()
    {
        rb.linearVelocity = direction.normalized * gravitonSurgeSpeed* TimeManager.Instance.timeScale;
    }

    private void OnTriggerEnter2D()
    {
        explosionPoint = transform.position;

        GameObject explosion = Instantiate(explodeEffectPrefab, explosionPoint, Quaternion.identity);
        GravitonExplodeController explosionObj = explosion.GetComponent<GravitonExplodeController>();

        explosionObj.Initialize(playerStats, explodeRadius, explosionPoint, surgeDuration);

        Destroy(gameObject);
    }
}
