using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GravitonExplodeController : MonoBehaviour
{
    [Header("Graviton Surge Info")]
    [SerializeField] private LayerMask enemyLayer;    // 적 레이어
    [SerializeField] private float explodeRadius;       // 서지 반경
    [SerializeField] private Vector2 explodePoint;       // 서지 반경
    private PlayerStats playerStats;    // 플레이어 스탯
    private float surgeDuration;

    public void Initialize(PlayerStats _playerStats, float _radius, Vector3 _explosionPoint, float _surgeDuration)
    {
        playerStats = _playerStats;
        explodeRadius = _radius;
        explodePoint = _explosionPoint;
        surgeDuration = _surgeDuration;
    }


    void Start()
    {
        
    }

    void Update()
    {
        

    }
    private void AnimationAttackTrigger()
    {
        Collider2D[] collisions = Physics2D.OverlapCircleAll(transform.position, explodeRadius, enemyLayer);

        foreach (Collider2D collision in collisions)
        {
            if (collision.GetComponent<Enemy>())
            {
                Enemy enemy = collision.GetComponent<Enemy>();
                EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
                if (enemyStats != null)
                {
                    playerStats.DoGravitonSurgeDamage(enemyStats);
                    // 적에게 데미지 적용
                }
            }
        }

        foreach (Collider2D collision in collisions)
        {
            Vector2 collisionPos = new Vector2(collision.transform.position.x, collision.transform.position.y);
            Vector2 dir = explodePoint - collisionPos;
            collision.transform.Translate(dir * 0.2f);
        }

        Destroy(gameObject, surgeDuration);
    }

    
}
