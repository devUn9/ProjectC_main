using UnityEngine;

public class Health_Bullet : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 10;

    private Vector3 direction;
    private float angle;

    // 발사 시 마우스 위치 기준 방향을 설정
    public void Initialize(Vector3 startPos, Vector3 mouseWorldPos)
    {
        transform.position = startPos;

        direction = (mouseWorldPos - startPos).normalized;
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);

        Destroy(gameObject, 2f);
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Health_Entity enemy = collision.GetComponent<Health_Entity>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }

    // 외부에서 데미지 설정 가능
    public void SetDamage(int value)
    {
        damage = value;
    }
}
