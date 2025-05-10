using UnityEngine;

public class Santan_Bullet : MonoBehaviour
{
    private float speed = 7f;
    private Vector2 vec2;
    public GameObject hitEffectPrefab;
    private int bulletDamage = 10;

    void Start()
    {

    }


    void Update()
    {
        transform.Translate(vec2 * speed * Time.deltaTime);
    }

    public void Move(Vector2 vec)
    {
        vec2 = vec;
        Destroy(gameObject, 2f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerStats _target = collision.GetComponent<PlayerStats>();
            GameObject hitEffect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            Destroy(hitEffect, 0.3f);

            if (_target != null)
            {
                _target.TakeDamage(bulletDamage);
            }

            Destroy(gameObject);
        }
    }
}
