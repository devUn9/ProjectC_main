using System.Collections;
using UnityEngine;

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
        Destroy(collision.gameObject);
    }
}
