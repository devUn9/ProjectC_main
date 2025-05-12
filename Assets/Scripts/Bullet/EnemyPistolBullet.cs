using System;
using System.Collections;
using UnityEngine;
//using static UnityEditor.PlayerSettings;

public class EnemyPistolBullet : MonoBehaviour
{
    [SerializeField] private float speed;
    private Enemy enemy;
    Vector2 startPos;
    Transform tr;
    Vector3 dir;

    float angle;
    Vector3 dirNo;

    public void Initialize(Enemy _enemy)
    {
        enemy = _enemy;
    }

    private void Start()
    {
        tr = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        startPos = transform.position;
        Vector3 Pos = new Vector3(startPos.x, startPos.y, 0);

        dir = tr.position - Pos;

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
        PlayerStats _target = collision.GetComponent<PlayerStats>();
        if (layer == LayerMask.NameToLayer("Player"))
        {
            enemy.stats.DoBulletDamage(_target);
        }
        Destroy(gameObject);
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
