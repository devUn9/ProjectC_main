using System.ComponentModel.Design;
using UnityEngine;

public class Boss1 : MonoBehaviour
{
    public Transform player;
    private Animator ani;

    [Header("Movement")]
    private float speed = 1f;
    private float angle;
    private Vector3 dir;

    public GameObject[] FirePoints;
    public GameObject bulletPrefab;
    public GameObject[] RocketPoints;
    public GameObject rocketPrefab;

    private bool isMoving = true;
    private bool isFire = false;
    private bool isRocket = false;

    public enum LayerName
    {
        IdleLayer = 0,
        WalkLayer = 1,
        FireLayer = 2,
        RocketLayer = 3
    }

    void Start()
    {
        ani = GetComponent<Animator>();
    }


    void Update()
    {
        CheckDistance();
        AngleAnimation();
        HandleLayers();
    }

    private void CheckDistance()
    {
        if (Vector3.Distance(transform.position, player.position) < 10f)
        {

            isMoving = false;
            isFire = true;
        }
        else if (Vector3.Distance(transform.position, player.position) > 20f)
        {
            isMoving = false;
            isFire = false;
            isRocket = true;
        }
        else
        {
            isMoving = true;
            isFire = false;
        }
    }

    #region Movement
    private void HandleLayers()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
            ActivateLayer(LayerName.WalkLayer);
        }
        else if (isFire)
        {
            ActivateLayer(LayerName.FireLayer);
        }
        else if (isRocket)
        {
            ActivateLayer(LayerName.RocketLayer);
        }
        else
        {
            ActivateLayer(LayerName.IdleLayer);
        }
    }
    #endregion

    #region Attack
    private void LazerAttack()
    {

    }

    private void SantanInUp()
    {
        FirePoints[0].SetActive(true);

        int count = 20; // 총알 개수
        float intervalAngle = 90f / (count - 1); // 90도(0~90도)를 총알 개수로 나눔
        for (int i = 0; i < count; i++)
        {
            GameObject clone = Instantiate(bulletPrefab, FirePoints[0].transform.position, Quaternion.identity);

            // 90도 ~ 180도 사이의 각도 계산
            float angle = 45f + (i * intervalAngle);
            float x = Mathf.Cos(angle * Mathf.Deg2Rad);
            float y = Mathf.Sin(angle * Mathf.Deg2Rad);

            clone.GetComponent<Santan_Bullet>().Move(new Vector2(x, y));
        }
    }

    private void SantanOutUp()
    {
        FirePoints[0].SetActive(false);
    }

    private void SantanInDown()
    {
        FirePoints[1].SetActive(true);

        int count = 20; // 총알 개수
        float intervalAngle = 90f / (count - 1); // 90도(-90~-180도)를 총알 개수로 나눔

        for (int i = 0; i < count; i++)
        {
            GameObject clone = Instantiate(bulletPrefab, FirePoints[1].transform.position, Quaternion.identity);

            // -90도 ~ -180도 사이의 각도 계산
            float angle = -45f - (i * intervalAngle);
            float x = Mathf.Cos(angle * Mathf.Deg2Rad);
            float y = Mathf.Sin(angle * Mathf.Deg2Rad);

            clone.GetComponent<Santan_Bullet>().Move(new Vector2(x, y));
        }
    }

    private void SantanOutDown()
    {
        FirePoints[1].SetActive(false);
    }

    private void SantanInLeft()
    {
        FirePoints[2].SetActive(true);

        int count = 20; // 총알 개수
        float intervalAngle = 90f / (count - 1); // 90도(135~225도)를 총알 개수로 나눔

        for (int i = 0; i < count; i++)
        {
            GameObject clone = Instantiate(bulletPrefab, FirePoints[2].transform.position, Quaternion.identity);

            // 135도 ~ 225도 사이의 각도 계산
            float angle = 135f + (i * intervalAngle);
            float x = Mathf.Cos(angle * Mathf.Deg2Rad);
            float y = Mathf.Sin(angle * Mathf.Deg2Rad);

            clone.GetComponent<Santan_Bullet>().Move(new Vector2(x, y));
        }
    }

    private void SantanOutLeft()
    {
        FirePoints[2].SetActive(false);
    }

    private void SantanInRight()
    {
        FirePoints[3].SetActive(true);

        int count = 20; // 총알 개수
        float intervalAngle = 90f / (count - 1); // 90도(45~135도)를 총알 개수로 나눔

        for (int i = 0; i < count; i++)
        {
            GameObject clone = Instantiate(bulletPrefab, FirePoints[3].transform.position, Quaternion.identity);

            // 45도 ~ 135도 사이의 각도 계산
            float angle = -45f + (i * intervalAngle);
            float x = Mathf.Cos(angle * Mathf.Deg2Rad);
            float y = Mathf.Sin(angle * Mathf.Deg2Rad);

            clone.GetComponent<Santan_Bullet>().Move(new Vector2(x, y));
        }
    }

    private void SantanOutRight()
    {
        FirePoints[3].SetActive(false);
    }

    private void RocketInUp()
    {
        RocketPoints[0].SetActive(true);
    }

    private void RocketOutUp()
    {
        RocketPoints[0].SetActive(false);
    }

    private void RocketInDown()
    {
        RocketPoints[1].SetActive(true);
    }

    private void RocketOutDown()
    {
        RocketPoints[1].SetActive(false);
    }

    private void RocketInLeft()
    {
        RocketPoints[2].SetActive(true);
    }

    private void RocketOutLeft()
    {
        RocketPoints[2].SetActive(false);
    }

    private void RocketInRight()
    {
        RocketPoints[3].SetActive(true);
    }

    private void RocketOutRight()
    {
        RocketPoints[3].SetActive(false);
    }
    
    #endregion

    #region Animation
    private void ActivateLayer(LayerName layerName)
    {
        for (int i = 0; i < ani.layerCount; i++)
        {
            ani.SetLayerWeight(i, 0);
        }
        ani.SetLayerWeight((int)layerName, 1);
    }

    void AngleAnimation()
    {
        dir = player.position - transform.position;
        angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        if (angle > -45 && angle <= 45)
        {
            //x
            ani.SetFloat("x", 1);
            ani.SetFloat("y", 0);
        }
        else if (angle > 45 && angle <= 135)
        {
            //y
            ani.SetFloat("x", 0);
            ani.SetFloat("y", 1);
        }
        else if (angle > 135 && angle <= 180 || angle <= -135)
        {
            //-x
            ani.SetFloat("x", -1);
            ani.SetFloat("y", 0);
        }
        else if (angle > -135 && angle <= -45)
        {
            //-y
            ani.SetFloat("x", 0);
            ani.SetFloat("y", -1);
        }
    }
    #endregion


}
