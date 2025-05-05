using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class EnemyAnimationTrigger : MonoBehaviour
{
    private Enemy enemy => GetComponentInParent<Enemy>();
    private bool hasFired = false;

    private void AnimationTrigger()
    {
        enemy.AnimationTrigger();
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
}
