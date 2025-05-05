using System;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("기본적인 스탯")]   //레벨업 등에 따른 추가적인 스탯 추가시 사용
    public Stat level;
    public Stat exp;
    public Stat cyberPhycosis;


    [Header("공격관련 스탯")]
    public Stat damage;
    public Stat meleeDamage;        //근접
    public Stat bulletDamage;       //총알
    public Stat grenadeDamage;    //폭발
    public Stat empGrenadeDamage;    //폭발
    public Stat launcherDamage;     //로켓런처


    [Header("방어관련 스탯")]
    public Stat maxHealth;  //체력  
    public Stat armor;
    public Stat empResistance;  // EMP 저항 퍼센트 단위 1~100

    [Header("상태 이상")]
    public Stat empShock;   // EMP 쇼크상태
    public Stat stun;       // 스턴


    public int currentHealth; //현재 체력

    public Action onHealthChanged; //체력 변화시 호출되는 델리게이트

    protected virtual void Start()
    {
        currentHealth = GetMaxHealth(); //현재 체력을 최대 체력으로 초기화; 
    }

    protected virtual void Update()
    {

    }

    public virtual void DoDamage(CharacterStats _targetStats)
    {
        int totalDamage = damage.GetValue();
        _targetStats.TakeDamage(totalDamage);

    }
    public virtual void DoMeleeDamage(CharacterStats _targetStats)
    {
        int damage = meleeDamage.GetValue();
        _targetStats.TakeDamage(damage);

    }
    public virtual void DoBulletDamage(CharacterStats _targetStats)
    {
        int damage = bulletDamage.GetValue();
        _targetStats.TakeDamage(damage);

    }
    public virtual void DoGrenadeDamage(CharacterStats _targetStats)
    {
        int damage = grenadeDamage.GetValue();
        _targetStats.TakeDamage(damage);

    }public virtual void DoLauncherDamage(CharacterStats _targetStats)
    {
        int damage = launcherDamage.GetValue();
        _targetStats.TakeDamage(damage);

    }

    public virtual void TakeDamage(int _damage)
    {
        _damage = CheckTargetArmor(this, _damage);

        DecreaseHealth(_damage);

        Debug.Log($"{_damage} _damage");
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void DecreaseHealth(int _damage)
    {
        currentHealth -= _damage;
        onHealthChanged?.Invoke();
    }

    protected virtual void Die()
    {

    }


    private int GetMaxHealth()
    {
        return maxHealth.GetValue();
    }

    private int CheckTargetArmor(CharacterStats _targetStats, int totalDamage)
    {
        totalDamage -= _targetStats.armor.GetValue();

        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        return totalDamage;
    }
}
