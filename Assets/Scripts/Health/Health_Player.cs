using System.Collections;
using UnityEngine;

public class Health_Player : Health_Entity
{
    private SpriteRenderer spriteRenderer;

    // hp 흔들림 효과
    [SerializeField] private Transform hpBarTransform;
    private Vector3 hpBarOriginalScale;

    private void Awake()
    {
        spriteRenderer = transform.Find("Animator")?.GetComponent<SpriteRenderer>();
        hpBarOriginalScale = hpBarTransform.localScale;
        // Entity에 정의된 Setup 호출
        base.SetUP();
    }

    private void Update()
    {
        //// 적 공격 메소드
        //if (Input.GetKeyDown(KeyCode.V))
        //{
        //    DamageNearbyEnemies(10, 5f);
        //}
    }

    private void DamageNearbyEnemies(float damage, float radius)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                Health_Entity enemy = hit.GetComponent<Health_Entity>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }
        }
    }

    // 기본 체력 + 스텟 보너스 + 버프 등과 같이 계산
    public override float MaxHP => MaxHPBasic + MaxHPBonus;
    public override float HPRecovery => 10;
    public float MaxHPBasic => 100 + 1 * 30;
    public float MaxHPBonus => 10 * 10;

    public override void TakeDamage(float damage)
    {
        HP -= damage;

        StartCoroutine("Hit");
        StartCoroutine(ScaleHPBar());
    }

    private IEnumerator Hit()
    {
        Color color = spriteRenderer.color;

        color.a = 0.2f;
        spriteRenderer.color = color;

        yield return new WaitForSeconds(0.1f);

        color.a = 1f;
        spriteRenderer.color = color;
    }

    private IEnumerator ScaleHPBar()
    {
        hpBarTransform.localScale = hpBarOriginalScale * 1.2f;
        yield return new WaitForSeconds(0.1f);
        hpBarTransform.localScale = hpBarOriginalScale;
    }
}
