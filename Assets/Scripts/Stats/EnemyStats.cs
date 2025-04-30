using UnityEngine;

public class EnemyStats : CharacterStats
{
    private Enemy enemy;

    protected override void Start()
    {
        base.Start();

        enemy = GetComponent<Enemy>();
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);

        //데미지 받을 때 효과 추가
    }

    protected override void Die()
    {
        base.Die();

        //적 죽을 때 효과 추가
    }
}
