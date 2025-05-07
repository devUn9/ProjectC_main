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
    
    //rigidbody2D로는 도무지 안되서 일단 위치를 옮기는 식으로 구현
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 플레이어의 Transform 가져오기
            Transform playerTransform = collision.transform;

            // 넉백 방향 계산 (보스에서 플레이어로의 방향)
            Vector2 knockbackDirection = (playerTransform.position - transform.position).normalized;

            // 넉백 강도 설정
            float knockbackDistance = 1f; // 넉백 거리

            // 플레이어 위치를 넉백 방향으로 이동
            playerTransform.position += (Vector3)(knockbackDirection * knockbackDistance);
        }
    }
}
