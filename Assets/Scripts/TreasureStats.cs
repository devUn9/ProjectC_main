using UnityEngine;

public class TreasureStats : CharacterStats
{
    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
    }
    protected override void Die()
    {
        base.Die();
        Destroy(gameObject);
    }
}
