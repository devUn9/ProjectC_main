using UnityEngine;

public class EnemyStats : CharacterStats
{
    private Enemy enemy;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    protected override void Start()
    {
        base.Start();
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
        enemy.DamageEffect();
        //데미지 받을 때 효과 추가
    }

    protected override void Die()
    {
        base.Die();
        Destroy(gameObject); //적 죽을 때 오브젝트 삭제
        //적 죽을 때 효과 추가
    }
}
