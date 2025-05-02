using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GrenadeController : MonoBehaviour
{
    [Header("Grenade Info")]
    public float explosionDelay;        // 폭발까지 걸리는 시간(초)
    public float explosionRadius;       // 폭발 범위
    public float explosionForce;        // 폭발 힘
    public float throwHeight;           // 수류탄이 날아가는 최대 높이
    public GameObject explosionEffect;      // 폭발 이펙트
    public LayerMask explosionLayers;       // 폭발 영향을 받는 레이어
    public LayerMask playerLayers;          // 플레이어 레이어(연막탄용)
    public LayerMask invisablePlayerLayers; // 연막탄 효과를 받는 레이어(투명화된 플레이어)
    public float disappearTime = 0.1f;      // 수류탄이 사라지는 시간

    [Header("Sound")]
    public AudioClip throwSound;        // 던지는 소리
    public AudioClip explosionSound;    // 폭발 소리

    private bool hasExploded = false;
    private Vector3 targetPosition;
    private float totalFlyTime;
    private float elapsedTime = 0f;
    private Vector3 startPosition;

    [SerializeField] private bool isSmoke;
    private float smokeTime;

    // 수류탄 초기화 (플레이어가 던질 때 호출)
    public void Initialize(Vector3 startPosition, Vector3 target)
    {
        transform.position = startPosition;
        targetPosition = target;

        // 날아가는 시간 계산 (거리에 비례)
        float distance = Vector3.Distance(startPosition, target);
        totalFlyTime = Mathf.Sqrt(distance) * 0.25f; // 거리에 따라 자연스러운 시간 계산

        // 사운드 재생
        if (throwSound != null && GetComponent<AudioSource>() != null)
        {
            GetComponent<AudioSource>().PlayOneShot(throwSound);
        }

        // 폭발 타이머 시작
        StartCoroutine(ExplosionTimer());
    }
    private void Start()
    {
        smokeTime = disappearTime;
        startPosition = transform.position;
    }

    private void Update()
    {
        // 아직 목적지에 도달하지 않았다면 포물선 이동
        if (elapsedTime < totalFlyTime)
        {
            MoveInParabola();
        }
        else
        {
            if (!isSmoke)
                return;

            StartCoroutine(ExplosionSmokeTimer());
            smokeTime -= Time.deltaTime;
            if (smokeTime > 0)
            {
                Smoke();
            }
            else
            {
                disappearSmoke();
            }
        }
    }

    private void MoveInParabola()
    {
        elapsedTime += Time.deltaTime;
        float normalizedTime = Mathf.Clamp01(elapsedTime / totalFlyTime);

        // 시작점과 끝점 사이의 직선 거리 계산
        Vector3 startToEnd = targetPosition - startPosition;

        // 2차 베지어 곡선 포물선 계산 (자연스러운 포물선을 위해)
        // 중간 제어점은 시작점과 끝점 사이의 중간이며, 높이를 추가
        Vector3 midPoint = startPosition + startToEnd * 0.5f;
        midPoint.y += throwHeight;

        // 베지어 곡선을 사용해 현재 위치 계산
        Vector3 currentPosition = QuadraticBezier(startPosition, midPoint, targetPosition, normalizedTime);

        // 수류탄 회전 효과
        transform.Rotate(Vector3.forward * 360f * Time.deltaTime, Space.Self);

        // 위치 업데이트
        transform.position = currentPosition;

        // 목적지에 도달했을 때
        if (normalizedTime >= 1f)
        {
            transform.position = targetPosition;
        }
    }

    // 2차 베지어 곡선 계산 함수
    private Vector3 QuadraticBezier(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        // B(t) = (1-t)^2*P1 + 2(1-t)t*P2 + t^2*P2
        float oneMinusT = 1f - t;
        return (oneMinusT * oneMinusT * p0) + (2f * oneMinusT * t * p1) + (t * t * p2);
    }

    // 폭발 타이머
    private IEnumerator ExplosionTimer()
    {
        yield return new WaitForSeconds(explosionDelay);
        Explode();
    }

    private IEnumerator ExplosionSmokeTimer()
    {
        yield return new WaitForSeconds(explosionDelay);
    }


    // 폭발 함수
    private void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        // 폭발 이펙트 생성
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // 폭발 사운드 재생
        if (explosionSound != null && GetComponent<AudioSource>() != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        }

        // 주변 오브젝트에 대미지 및 밀어내기 적용
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, explosionLayers);

        foreach (Collider2D enemy in colliders)
        {
            // 데미지를 줄 수 있는 컴포넌트가 있는지 확인
            Enemy damageable = enemy.GetComponent<Enemy>();
            if (damageable != null)
            {
                // 거리에 따른 데미지 계산
                float distance = Vector2.Distance(transform.position, enemy.transform.position);
                float damagePercent = 1f - Mathf.Clamp01(distance / explosionRadius);
                int damage = Mathf.RoundToInt(damagePercent * 100f); // 최대 100 데미지

                damageable.TakeDamage(damage);
            }
            // 물리 효과가 있는 오브젝트 밀어내기
            Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direction = enemy.transform.position - transform.position;
                float distance = Mathf.Max(0.1f, direction.magnitude);
                float forceFactor = 1f - Mathf.Clamp01(distance / explosionRadius);

                rb.AddForce(direction.normalized * explosionForce * forceFactor, ForceMode2D.Impulse);
            }
        }

        Destroy(gameObject, disappearTime);
    }

    private void Smoke()
    {
        // 연막탄 효과 구현
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, playerLayers);
        foreach (Collider2D player in colliders)
        {
            Player invisibility = player.GetComponent<Player>();
            if (invisibility != null)
            {
                invisibility.Invisibility();
            }
        }
        //float smokeEnd = explosionRadius + 5.0f;
        // 바깥쪽 원의 감지 결과
        Collider2D[] outerColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius + 0.5f, invisablePlayerLayers);

        // 안쪽 원의 감지 결과
        Collider2D[] innerColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, invisablePlayerLayers);

        // 양파링 영역에 해당하는 객체를 저장할 리스트
        List<Collider2D> onionRingColliders = new List<Collider2D>();

        // 바깥쪽 원에 포함된 객체 중 안쪽 원에 포함되지 않은 객체를 필터링
        foreach (Collider2D outerCollider in outerColliders)
        {
            bool isInsideInnerCircle = false;

            // innerColliders에 포함되어 있는지 확인
            foreach (Collider2D innerCollider in innerColliders)
            {
                if (outerCollider == innerCollider)
                {
                    isInsideInnerCircle = true;
                    break;
                }
            }

            // innerColliders에 포함되지 않은 경우에만 처리
            if (!isInsideInnerCircle)
            {
                Player visibility = outerCollider.GetComponent<Player>();
                if (visibility != null)
                {
                    visibility.Visiblilty();
                }
            }
        }
    }

    private void disappearSmoke()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, invisablePlayerLayers);
        foreach (Collider2D player in colliders)
        {
            Player visibility = player.GetComponent<Player>();
            if (visibility != null)
            {
                visibility.Visiblilty();
            }
        }
        Destroy(gameObject);
    }

    // 폭발 범위 시각화 (디버깅용)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
        //Gizmos.DrawWireSphere(transform.position, explosionRadius + 0.5f);
    }

}
