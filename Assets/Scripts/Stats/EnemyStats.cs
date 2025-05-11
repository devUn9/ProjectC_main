using System.Collections;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    private Enemy enemy;

    // hp 흔들림 효과
    [SerializeField] private Transform hpBarTransform;
    private Vector3 hpBarOriginalScale;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    protected override void Start()
    {
        base.Start();
        hpBarOriginalScale = hpBarTransform.localScale;
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
        enemy.healthCheck();    //공격 받을 시 Battle상태 돌입
        enemy.DamageEffect();
        StartCoroutine(ScaleHPBar());
        //데미지 받을 때 효과 추가
    }

    public override void DoEmpGrenadeDamage(CharacterStats _targetStats, float _Duration)
    {
        base.DoEmpGrenadeDamage(_targetStats, _Duration);
        enemy.EmpEffect(_Duration);
    }

    public override void DoStun(CharacterStats _targetStats)
    {
        base.DoStun(_targetStats);
        enemy.StunEffect();
    }

    public override void DoStunRecovery(CharacterStats _targetStats)
    {
        base.DoStunRecovery(_targetStats);
        //enemy.StunEffectOff();
    }

    protected override void Die()
    {
        base.Die();

        if(enemy.enemyType == EnemyType.Robot) {
            EffectManager.Instance.PlayEffect(EffectType.GrenadeEffect, transform.position, 0.5f);
            Destroy(gameObject);
        }
        else if (enemy.enemyType == EnemyType.Human)
        {
            enemy.DieShader();
            enemy.stats.StatusSpeed = 0f;
        }
        Destroy(gameObject,1f); //적 죽을 때 오브젝트 삭제
        //적 죽을 때 효과 추가
    }

    private IEnumerator ScaleHPBar()
    {
        hpBarTransform.localScale = hpBarOriginalScale * 1.2f;
        yield return new WaitForSeconds(0.1f);
        hpBarTransform.localScale = hpBarOriginalScale;
    }
}
