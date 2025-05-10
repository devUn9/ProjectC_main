using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class EnemyAnimationTrigger : MonoBehaviour
{
    private Enemy enemy => GetComponentInParent<Enemy>();
    //private EnemyState EnemyState => GetComponentInParent<EnemyState>();
    private bool hasFired = false;
    private Vector3 dir;
    private float distance;

    private void AnimationTrigger()
    {
        enemy.AnimationTrigger();
    }


    private void AnimationMeleeAttackTrigger()
    {
        dir = EnemyToPlayerDirection();
        //enemy.meleeAttackPrefab.transform.right = dir.normalized;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        
        enemy.meleeAttackAngle = Quaternion.Euler(0f, 0f, angle-90f);

        Instantiate(enemy.meleeAttackPrefab, enemy.transform.position, enemy.meleeAttackAngle);
        Collider2D collision = Physics2D.OverlapCircle(enemy.transform.position, enemy.meleeAttackRadius, enemy.playerLayer);
        PlayerStats playerStats = collision.GetComponent<PlayerStats>();
        enemy.stats.DoMeleeDamage(playerStats);
        StartCoroutine(ResetHasFired());
    }

    private void AttackPistol()
    {
        if (hasFired) return; // 이미 호출된 경우 실행하지 않음
        hasFired = true;
        GameObject bulletObj = Instantiate(enemy.bulletPrefab, enemy.transform.position, Quaternion.identity);
        EnemyPistolBullet bullet = bulletObj.GetComponent<EnemyPistolBullet>();

        if (bullet != null)
        {
            bullet.Initialize(enemy);
        }
        // 일정 시간 후 다시 호출 가능하도록 초기화
        StartCoroutine(ResetHasFired());
    }

    private IEnumerator ResetHasFired()
    {
        yield return new WaitForSeconds(0.1f); // 원하는 시간 설정
        hasFired = false;
    }

    private void AnimationDestroy()
    {
        Destroy(gameObject);
    }

    public float EnemyToPlayerDistance()
    {
        Player player = GameObject.FindAnyObjectByType<Player>();

        float distance = Vector3.Distance(enemy.transform.position, player.transform.position);
        return distance;
    }

    public Vector3 EnemyToPlayerDirection()
    {
        Player player = GameObject.FindAnyObjectByType<Player>();
        Vector3 direction = player.transform.position - enemy.transform.position;
        Vector3 normalizeDir = direction.normalized;
        return normalizeDir;
    }
}
