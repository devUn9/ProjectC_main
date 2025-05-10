using Unity.Mathematics;
using UnityEngine;

public class EnemyGranade : MonoBehaviour
{
    private Transform player;
    public GameObject explorePrefab;
    private float speed = 1f;
    private float arcHeight = 5f;
    private float AttackCheckRadius = 1f;
    private int GranadeDamage = 50;
    private float knockbackforce = 5f;

    private Vector2 startPoint;
    private Vector2 targetPoint;
    private float time;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        startPoint = transform.position;
        targetPoint = player.position;
        time = 0f;
    }


    void Update()
    {
        time += Time.deltaTime * speed;

        Move();

        if (time >= 1f)
        {
            Explode();
        }

    }

    public void SetDirection(Vector2 _targetPoint)
    {
        targetPoint = _targetPoint;
    }

    private void Move()
    {
        float x = Mathf.Lerp(startPoint.x, targetPoint.x, time);
        float y = Mathf.Lerp(startPoint.y, targetPoint.y, time) + arcHeight * Mathf.Sin(Mathf.Clamp01(time) * Mathf.PI);
        transform.position = new Vector2(x, y);
    }

    private void Explode()
    {
        GameObject explore = Instantiate(explorePrefab, transform.position, Quaternion.identity);
        Destroy(explore, 0.3f);
        SoundManager.instance.PlayESFX(SoundManager.ESfx.SFX_GrenadeExplosion);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, AttackCheckRadius);

        foreach (Collider2D collider in colliders)
        {
            if (collider.GetComponent<PlayerStats>() != null)
            {
                PlayerStats _target = collider.GetComponent<PlayerStats>();
                Player player = collider.GetComponent<Player>();

                if (_target != null)
                {
                    _target.TakeDamage(GranadeDamage);
                    player.SetupKnockbackDir(gameObject.transform, knockbackforce);
                    SoundManager.instance.PlayESFX(SoundManager.ESfx.SFX_HurtSound);
                }
            }
        }
        Destroy(gameObject);
    }
}
