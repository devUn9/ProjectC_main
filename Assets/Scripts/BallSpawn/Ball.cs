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

    Vector3 dir;
    Vector3 target2 = new Vector3 (50, 300, 0);

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        dir = (target2 - transform.position).normalized;
    }

    void Update()
    {
        transform.Translate (dir * speed * Time.deltaTime * TimeManager.Instance.timeScale);
    }
    public void TouchEffect(Transform pos)
    {
        GameObject effect = Instantiate(effectPrefab, pos);
        effect.transform.SetParent(pos);
    }
}
