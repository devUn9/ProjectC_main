using System;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("기본적인 스탯")]   //레벨업 등에 따른 추가적인 스탯 추가시 사용
    public Stat Health;  //체력  

    [Header("공격관련 스탯")]
    public Stat damage;

    [Header("방어관련 스탯")]
    public Stat armor;

    public int currentHealth; //현재 체력

    public Action onHealthChanged; //체력 변화시 호출되는 델리게이트

    protected virtual void Start()
    {
        currentHealth = GetMaxHealth(); //현재 체력을 최대 체력으로 초기화; 
    }

    protected virtual void Update()
    {
        int totalDamage = damage.GetValue();

    }

    public virtual void DoDamage(CharacterStats _targetStats)
    {
        
    }

    public virtual void TakeDamage(int _damage)
    {
        DecreaseHealth(_damage);

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void DecreaseHealth(int _damage)
    {
        currentHealth -= _damage;
        if(onHealthChanged != null)
            onHealthChanged?.Invoke();
    }

    protected virtual void Die()
    {

    }


    private int GetMaxHealth()
    {
        return Health.GetValue();
    }
}
