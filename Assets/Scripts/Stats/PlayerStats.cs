using System.Collections;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    private Player player;
    [SerializeField] private Transform hpBarTransform;
    private Vector3 hpBarOriginalScale;

    private int damageCalculator;

    private int RecoveryHealth = 10;

    private void Awake()
    {
        player = GetComponent<Player>();
        hpBarOriginalScale = hpBarTransform.localScale;
    }

    
    protected override void Start()
    {
        base.Start();
        StartCoroutine(Recovery());
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
        player.DamageEffect();
        EffectManager.Instance.PlayEffect(EffectType.BloodSplatterEffect, transform.position, 1f);
        StartCoroutine(ScaleHPBar());
        //추가 데미지 받을 때 효과 추가
    }

    protected override void Die()
    {
        base.Die();
        Debug.Log("플레이어 사망");
        //플레이어 죽을 때 효과 추가
    }

    private IEnumerator Recovery()
    {
        while(true)
        {
            if(currentHealth < maxHealth.GetValue())
                currentHealth += RecoveryHealth;
            
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator ScaleHPBar()
    {
        hpBarTransform.localScale = hpBarOriginalScale * 1.2f;
        yield return new WaitForSeconds(0.1f);
        hpBarTransform.localScale = hpBarOriginalScale;
    }

}
