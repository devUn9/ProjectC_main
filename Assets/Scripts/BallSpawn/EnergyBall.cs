using UnityEngine;

public class EnergyBall : Ball
{
    private void Awake()
    {
        ballType = BallType.EnergyBall;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>())
        {
            Player player = collision.GetComponent<Player>();
            player.InitializeShield();
        }
        SoundManager.instance.PlayESFX(SoundManager.ESfx.SFX_EnergyBall);
        Destroy(gameObject);
    }
}
