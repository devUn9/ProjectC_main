using System.Collections;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.UIElements;

public class LauncherMissileController : MonoBehaviour
{
    [Header("Missile Info")]
    public float startSpeed;         // 발사체 초기 속도;
    public float maxSpeed;           // 발사체 최대 속도;
    public float explosionRadius;    // 폭발 범위
    public float explosionDelay;     // 폭발까지 걸리는 시간(초)
    [SerializeField] private LayerMask explosionLayers;   // 폭발 영향을 받는 레이어

    [SerializeField] private GameObject explosionEffect;  // 폭발 이펙트

    [Header("Sound")]
    [SerializeField] private AudioClip launchSound;       // 발사 소리
    [SerializeField] private AudioClip explosionSound;    // 폭발 소리

    private Vector3 beforeMissileDir;       // 발사체 발사방향;
    private bool isControl;
    private float mouseToMissileDistance;
    private Camera mainCamera;
    private Rigidbody2D rb;

    private PlayerStats playerStats; // 플레이어 스탯

    public void Initialize(Vector3 startPosition, PlayerStats _playerStats)
    {
        transform.position = startPosition;
        playerStats = _playerStats;

        // 사운드 재생
        if (launchSound != null && GetComponent<AudioSource>() != null)
        {
            GetComponent<AudioSource>().PlayOneShot(launchSound);
        }

        // 폭발 타이머 시작
        StartCoroutine(ExplosionTimer());
    }
    private void Awake()
    {
        isControl = true;
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        MoveMissile();
        if (explosionDelay > 0)
        {
            explosionDelay -= Time.deltaTime * TimeManager.Instance.timeScale;
        }
        else
        {
            ExplodeMissile();
        }
    }

    private void MoveMissile()
    {
        Vector2 dir;
        if (isControl)
        {
            dir = MousePosition() - transform.position;
            beforeMissileDir = dir;
            mouseToMissileDistance = Vector2.Distance(MousePosition(), transform.position);

            if (mouseToMissileDistance <= 1)
                isControl = false;
        }
        else
        {
            dir = beforeMissileDir;
        }
        rb.linearVelocity = dir.normalized * maxSpeed * TimeManager.Instance.timeScale;
        transform.right = dir.normalized; // 발사체가 바라보는 방향을 업데이트
    }

    private void ExplodeMissile()
    {
        // 폭발 이펙트 생성
        if (explosionEffect != null)
        {
            EffectManager.Instance.PlayEffect(EffectType.GrenadeEffect, transform.position, explosionRadius * 0.8f);
        }
        else
        {
            Debug.LogError("폭발 이펙트가 설정되지 않았습니다.");
        }


        // 폭발 사운드 재생
        if (explosionSound != null && GetComponent<AudioSource>() != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, explosionLayers);
        foreach (Collider2D collider in colliders)
        {
            // 적에게 대미지 적용
            Enemy enemy = collider.GetComponent<Enemy>();
            EnemyStats _target = enemy.GetComponent<EnemyStats>();
            if (enemy != null)
            {
                playerStats.DoLauncherDamage(_target); // 대미지 값 설정
            }
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null)
            return;

        int layer = collision.gameObject.layer;

        if (layer == LayerMask.NameToLayer("Enemy"))
        {
            ExplodeMissile();
        }
    }

    public Vector3 MousePosition()
    {
        Vector3 targetPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        targetPosition.z = transform.position.z; // Z축 값 고정
        return targetPosition;
    }


    private IEnumerator ExplosionTimer()
    {
        float elapsedTime = 0f;

        while (elapsedTime < explosionDelay)
        {
            // 현재 시간 스케일에 따라 경과 시간 계산
            elapsedTime += Time.deltaTime * TimeManager.Instance.timeScale;
            yield return null; // 다음 프레임까지 대기
        }
        ExplodeMissile();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
