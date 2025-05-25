using UnityEngine;

public class EnergyShield : MonoBehaviour
{
    [SerializeField] public int shieldCount;
    [SerializeField] private Player player;

    private Animator anim;
    
    private void Start()
    {
        anim = GetComponent<Animator>();
        player = GetComponentInParent<Player>();
    }

    //private void OnEnable()
    //{
    //    SoundManager.instance.LoopESFX(SoundManager.ESfx.SFX_EnergyShield);
    //}

    private void Update()
    {
        anim.speed = TimeManager.Instance.timeScale;
        if (shieldCount <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Ball ball = collision.GetComponent<Ball>();
        if (ball != null)
        {
            Debug.Log("DamageBall 감지됨");
            if (ball.ballType == BallType.DamageBall)
                shieldCount -= 1;
            else if(ball.ballType == BallType.EnergyBall)
                shieldCount = 3;

            ball.TouchEffect(player.transform);
        }
    }


}
