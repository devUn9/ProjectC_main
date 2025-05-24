using UnityEngine;

public class DamageBall : Ball
{
    private void Awake()
    {
        ballType = BallType.DamageBall;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>())
        {
            Player player = collision.GetComponent<Player>();
            PlayerStats stats = player.GetComponent<PlayerStats>();

            Transform pos = player.transform;

            stats.BallInteraction(damage);
            TouchEffect(pos);
        }
        Destroy(gameObject);
    }
}
