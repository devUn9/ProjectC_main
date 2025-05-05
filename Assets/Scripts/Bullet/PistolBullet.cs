using UnityEngine;

public class PistolBullet : MonoBehaviour
{
    [SerializeField] private float speed;
    private Player player;
    public int damage;
    Vector2 MousePos;
    Transform tr;
    Vector3 dir;

    float angle;
    Vector3 dirNo;

    public void Initialize(Player _player)
    {
        player = _player;
    }
    private void Start()
    {
        tr = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 Pos = new Vector3(MousePos.x, MousePos.y, 0);
        dir = Pos - tr.position;

        //바라보는 각도구하기
        angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        //normalized 단위벡터
        dirNo = new Vector3(dir.x, dir.y, 0).normalized;
        Destroy(gameObject, 2f);
    }
    private void Update()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        transform.position += dirNo * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int layer = collision.gameObject.layer;
        EnemyStats _target = collision.GetComponent<EnemyStats>();
        if (layer == LayerMask.NameToLayer("Enemy"))
        {
            player.stats.DoBulletDamage(_target);
        }
        Destroy(gameObject);
    }

    internal void SetDamage(Stat damage)
    {
        this.damage = damage.GetValue();
    }
}
