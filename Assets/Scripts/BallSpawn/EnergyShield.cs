using UnityEngine;
using static UnityEditor.PlayerSettings;

public class EnergyShield : MonoBehaviour
{
    [SerializeField] public int shieldCount;
    [SerializeField] private Player player;

    private void Awake()
    {
        gameObject.SetActive(false);
        player = GetComponentInParent<Player>();
    }

    private void Update()
    {
        if (shieldCount < 0)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<DamageBall>())
        {
            DamageBall ball = collision.GetComponent<DamageBall>();
            if(ball.ballType == BallType.DamageBall)
            {
                shieldCount -= 1;
            }
            ball.TouchEffect(player.transform);
        }
    }
}
