using System.Collections;
using UnityEngine;

//안쓰나 본데 이 스크립트?

public class Knockback_Player : MonoBehaviour
{

    void Start()
    {

    }


    void Update()
    {

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Bullet"))
        {
            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("Player"))
        {
            if (collision.GetComponent<PlayerStats>() != null)
            {
                PlayerStats _target = collision.GetComponent<PlayerStats>();
                Player player = collision.GetComponent<Player>();

                if (_target != null)
                {
                    _target.TakeDamage(30);
                    player.SetupKnockbackDir(gameObject.transform, 10f);
                }
            }
        }

    }
}
