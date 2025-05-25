using UnityEngine;

public enum BallType
{
    DamageBall,
    EnergyBall
}

public class Ball : MonoBehaviour
{
    public BallType ballType;

    public int damage;
    public float shield;

    public GameObject target;
    [SerializeField] private float speed;
    [SerializeField] protected GameObject effectPrefab;
    
    private Rigidbody2D rb;
    [SerializeField] private Animator anim;

    Vector3 dir;
    Vector3 target2 = new Vector3 (50, 300, 0);


    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        dir = (target2 - transform.position).normalized;
    }

    void Update()
    {
        anim.speed = TimeManager.Instance.timeScale;
        transform.Translate (dir * speed * Time.deltaTime * TimeManager.Instance.timeScale);
    }
    public void TouchEffect(Transform pos)
    {
        GameObject effect = Instantiate(effectPrefab, pos);
        effect.transform.SetParent(pos);
    }
}
