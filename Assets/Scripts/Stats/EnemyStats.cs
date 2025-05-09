using System.Collections;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    private Enemy enemy;

    // hp 흔들림 효과
    [SerializeField] private Transform hpBarTransform;
    private Vector3 hpBarOriginalScale;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    protected override void Start()
    {
        base.Start();
        hpBarOriginalScale = hpBarTransform.localScale;
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
        enemy.healthCheck();
        enemy.DamageEffect();
        StartCoroutine(ScaleHPBar());
        //데미지 받을 때 효과 추가
    }

    protected override void Die()
    {
        base.Die();
        Destroy(gameObject); //적 죽을 때 오브젝트 삭제
        //적 죽을 때 효과 추가
    }

    private IEnumerator ScaleHPBar()
    {
        hpBarTransform.localScale = hpBarOriginalScale * 1.2f;
        yield return new WaitForSeconds(0.1f);
        hpBarTransform.localScale = hpBarOriginalScale;
    }
}
