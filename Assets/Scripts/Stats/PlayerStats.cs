using UnityEngine;

public class PlayerStats : CharacterStats
{
    private Player player;
    private int damageCalculator;

    private void Awake()
    {
        player = GetComponent<Player>();
    }
    protected override void Start()
    {
        base.Start();
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
        player.DamageEffect();
        //추가 데미지 받을 때 효과 추가
    }

    protected override void Die()
    {
        base.Die();
        Debug.Log("플레이어 사망");
        //플레이어 죽을 때 효과 추가
    }

}
