using System.Collections;
using NUnit.Framework.Constraints;
using UnityEngine;

public class GenerateWall : MonoBehaviour
{
    public float respawnTime = 5f; // 재생성 대기 시간
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    void Start()
    {
        // 벽의 초기 위치와 회전 저장
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    // 외부에서 이 함수를 호출해서 벽을 부숨
    public void BreakWall()
    {
        // 벽 비활성화
        gameObject.SetActive(false);
        // 일정 시간 후 재생성
        Invoke(nameof(RespawnWall), respawnTime);
    }

    private void RespawnWall()
    {
        // 위치/회전 원래대로 (필요 시)
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        // 벽 활성화
        gameObject.SetActive(true);
    }
}
