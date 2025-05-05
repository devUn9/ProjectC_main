using System.Collections;
using UnityEngine;

public class PlayerAnimationTrigger : MonoBehaviour
{
    private Player player => GetComponentInParent<Player>();
    private bool hasAttacked = false;

    private void AnimationTrigger()
    {
        player.AnimationTrigger();
    }

    private void AttackPistol()
    {
        if (hasAttacked) return; // 이미 호출된 경우 실행하지 않음
        hasAttacked = true;
        GameObject bulletObj = Instantiate(player.bulletPrefab, player.transform.position, Quaternion.identity);
        PistolBullet bullet = bulletObj.GetComponent<PistolBullet>();

        if (bullet != null)
        {
            bullet.Initialize(player);
        }
        // 일정 시간 후 다시 호출 가능하도록 초기화
        StartCoroutine(ResetHasFired());
    }

    private IEnumerator ResetHasFired()
    {
        yield return new WaitForSeconds(0.1f); // 원하는 시간 설정
        hasAttacked = false;
    }
}
