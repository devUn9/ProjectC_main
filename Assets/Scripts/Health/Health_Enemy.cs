using System.Collections;
using UnityEngine;

public class Health_Enemy : Health_Entity
{
    private Enemy enemy;
    private SpriteRenderer spriteRenderer;

    // hp 흔들림 효과
    [SerializeField] private Transform hpBarTransform;
    private Vector3 hpBarOriginalScale;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        hpBarOriginalScale = hpBarTransform.localScale;
        // Entity에 정의된 Setup 호출
        base.SetUP();
    }

    private void Update()
    {
        // 적 공격 메소드
        if (Input.GetKeyDown(KeyCode.X))
        {
            int damage = Random.Range(1, 100);
            target.TakeDamage(damage);
        }
    }

    // 기본 체력 + 스텟 보너스 + 버프 등과 같이 계산
    public override float MaxHP => 200;
    public override float HPRecovery => 10;

    public override void TakeDamage(float damage)
    {
        HP -= damage;

        StartCoroutine("Hit");
        StartCoroutine(ScaleHPBar());
    }

    private IEnumerator Hit()
    {
        //Color color = spriteRenderer.color;

        //color.a = 0.2f;
        //spriteRenderer.color = color;

        //yield return new WaitForSeconds(0.1f);

        //color.a = 1f;
        //spriteRenderer.color = color;

        Color original = spriteRenderer.color;
        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        spriteRenderer.color = original;
    }

    private IEnumerator ScaleHPBar()
    {
        hpBarTransform.localScale = hpBarOriginalScale * 1.2f;
        yield return new WaitForSeconds(0.1f);
        hpBarTransform.localScale = hpBarOriginalScale;
    }
}
