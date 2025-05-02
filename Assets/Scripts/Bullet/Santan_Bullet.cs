using UnityEngine;

public class Santan_Bullet : MonoBehaviour
{
    private float speed = 7f;
    private Vector2 vec2;

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
            Destroy(gameObject);
        }
    }
}
