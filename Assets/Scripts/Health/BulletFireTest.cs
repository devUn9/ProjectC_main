using UnityEngine;

public class BulletFireTest : MonoBehaviour
{
    public GameObject bulletPrefab;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;

            GameObject bullet = Instantiate(bulletPrefab);
            bullet.GetComponent<Health_Bullet>().Initialize(transform.position, mouseWorld);
        }
    }
}
