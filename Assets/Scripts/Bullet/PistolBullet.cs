using System.Collections;
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
        DestroyTimer();
    }
    private void Update()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        transform.position += dirNo * speed * Time.deltaTime * TimeManager.Instance.timeScale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int layer = collision.gameObject.layer;
        EnemyStats _target = collision.GetComponent<EnemyStats>();
        Boss1Stats _targetboss = collision.GetComponent<Boss1Stats>();
        if (layer == LayerMask.NameToLayer("Enemy"))
        {
            player.stats.DoBulletDamage(_target);
        }
        if (collision.CompareTag("Boss1"))
        {
            player.stats.DoBulletDamage(_targetboss);
        }
        Destroy(gameObject);
    }

    internal void SetDamage(Stat damage)
    {
        this.damage = damage.GetValue();
    }

    private IEnumerator DestroyTimer()
    {
        float elapsedTime = 0f;

        while (elapsedTime < 2f)
        {
            // 현재 시간 스케일에 따라 경과 시간 계산
            elapsedTime += Time.deltaTime * TimeManager.Instance.timeScale;
            yield return null; // 다음 프레임까지 대기
        }
        Destroy(gameObject);
    }
}
