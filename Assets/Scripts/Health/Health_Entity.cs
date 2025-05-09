using System;
using System.Collections;
using UnityEngine;

public abstract class Health_Entity : MonoBehaviour
{
    private Stats stats; // 캐릭터 정보
    public Health_Entity target; // 공격 대상

    // 체력 프로퍼티: 0~최대 체력 사이값을 넘기지 못하게 설정
    public float HP
    {
        set => stats.HP = Mathf.Clamp(value, 0, MaxHP);
        get => stats.HP;
    }

    public float damage
    {
        set => stats.damage = 0f;
        get => stats.damage;
    }

    public float meleeDamage
    {
        set => stats.meleeDamage = 55f;
        get => stats.meleeDamage;
    }
    
    public float bulletDamage
    {
        set => stats.bulletDamage = 15f;
        get => stats.bulletDamage;
    }

    public float grenadeDamage
    {
        set => stats.grenadeDamage = 80f;
        get => stats.grenadeDamage;
    }

    public float empGrenadeDamage
    {
        set => stats.empGrenadeDamage = 65f;
        get => stats.empGrenadeDamage;
    }

    public float launcherDamage
    {
        set => stats.launcherDamage = 95f;
        get => stats.launcherDamage;
    }

    public float gravitonSurgedDamage
    {
        set => stats.gravitonSurgedDamage = 15f;
        get => stats.gravitonSurgedDamage;
    }
    

    // 현재 프로퍼티에서 추상 선언하여서 실제 작동하는 내용은 플레이어, 적과 같은 파생 클래스에서 정의
    public abstract float MaxHP { get; }
    public abstract float HPRecovery { get; }

    protected void SetUP()
    {
        HP = MaxHP;

        StartCoroutine("Recovery");
    }

    // 초당 체력 회복 코루틴
    protected IEnumerator Recovery()
    {
        while (true)
        {
            if (HP < MaxHP) HP += HPRecovery;

            yield return new WaitForSeconds(1f);
        }
    }

    //stat합치는 작업중

    public virtual void DoDamage(Health_Entity _targetStats)
    {

    }
    public virtual void DoMeleeDamage(Health_Entity _targetStats)
    {

    }
    public virtual void DoBulletDamage(Health_Entity _targetStats)
    {

    }
    public virtual void DoGrenadeDamage(Health_Entity _targetStats)
    {

    }
    public virtual void DoLauncherDamage(Health_Entity _targetStats)
    {

    }
    public virtual void DoGravitonSurgeDamage(Health_Entity _targetStats)
    {

    }

    // 상대방 공격 시 데미지 호출
    public abstract void TakeDamage(float damage);

    public virtual void Die()
    {

    }
}

[System.Serializable] // 직렬화로 인스펙터에서 구조체 수정 가능
public struct Stats
{
    // 체력 정보
    [HideInInspector] public float HP;

    [Tooltip("공격관련 스탯")]
    public float damage;
    public float meleeDamage;
    public float bulletDamage;
    public float grenadeDamage;
    public float empGrenadeDamage;
    public float launcherDamage;
    public float gravitonSurgedDamage;

    [Tooltip("방어관련 스탯")]
    public float maxHealth;  //체력  
    public float armor;
}
