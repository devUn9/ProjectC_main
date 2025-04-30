using UnityEngine;

public class PlayerStats : CharacterStats
{
    private Player player;

    protected override void Start()
    {
        base.Start();

        player = GetComponent<Player>();
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
        
        //추가 데미지 받을 때 효과 추가
    }

    protected override void Die()
    {
        base.Die();

        //플레이어 죽을 때 효과 추가
    }
}
