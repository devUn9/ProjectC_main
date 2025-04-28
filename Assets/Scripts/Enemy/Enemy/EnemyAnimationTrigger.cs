using UnityEngine;

public class EnemyAnimationTrigger : MonoBehaviour
{
    private Enemy enemy => GetComponentInParent<Enemy>();

    private void AnimationTrigger()
    {
        enemy.AnimationTrigger();
    }

    private void AttackPistol()
    {
        Instantiate(enemy.bulletPrefab, enemy.transform.position, Quaternion.identity);
    }
}
