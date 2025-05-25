using System;
using System.Collections;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("기본적인 스탯")]   //레벨업 등에 따른 추가적인 스탯 추가시 사용
    public Stat level;
    public Stat exp;
    public Stat cyberPhycosis;


    [Header("공격관련 스탯")]
    public Stat damage;
    public Stat meleeDamage;            //근접
    public Stat bulletDamage;           //총알
    public Stat grenadeDamage;          //수류탄
    public Stat empGrenadeDamage;       //EMP 수류탄
    public Stat launcherDamage;         //로켓런처
    public Stat gravitonSurgedDamage;    //중력자탄


    [Header("방어관련 스탯")]
    public Stat maxHealth;  //체력  
    public Stat armor;
    public Stat empResistance;  // EMP 저항 퍼센트 단위 1~100

    [Header("상태 이상")]
    public bool empShock = false;    // EMP 쇼크상태
    public bool stun = false;        // 스턴
    public float StatusSpeed = 1f;  // 시간정지 상태

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

    }

    public virtual void DoEmpGrenadeDamage(CharacterStats _targetStats, float _Duration)
    {
        int damage = empGrenadeDamage.GetValue();

        StartCoroutine(DoEmpStatus(_targetStats, _Duration)); // EMP 상태 이상 효과
        _targetStats.TakeDamage(damage);
    }

    public virtual void DoLauncherDamage(CharacterStats _targetStats)
    {
        int damage = launcherDamage.GetValue();
        _targetStats.TakeDamage(damage);

    }
    public virtual void DoGravitonSurgeDamage(CharacterStats _targetStats)
    {
        int damage = gravitonSurgedDamage.GetValue();
        _targetStats.TakeDamage(damage);
    }

    // smoke grenade 효과 : EnemyType.Human에게만 기절
    public virtual void DoStun(CharacterStats _targetStats)
    {
        stun = true; // 스턴 상태로 변경
        _targetStats.StatusSpeed = 0f;
    }
    // smoke grenade 효과 : EnemyType.Human에게만 기절 회복
    public virtual void DoStunRecovery(CharacterStats _targetStats)
    {
        stun = false; // 스턴 상태로 변경
        _targetStats.StatusSpeed = 1f;
    }

    // EMP grenade 효과 : EnemyType.Robot에게만 기절
    public virtual IEnumerator DoEmpStatus(CharacterStats _targetStats, float _Duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < _Duration)
        {
            elapsedTime += Time.deltaTime * TimeManager.Instance.timeScale;
            empShock = true;
            _targetStats.StatusSpeed = 0f;
            yield return null;
        }
        empShock = false;
        _targetStats.StatusSpeed = 1f;
        Debug.Log("StatRecovery : " + _targetStats.StatusSpeed);
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
