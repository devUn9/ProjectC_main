using UnityEngine;

public class LightController : MonoBehaviour
{
    [SerializeField] private float activationDistance = 5f; // 플레이어와의 활성화 거리
    [SerializeField] private GameObject lightObject;       // 빛 오브젝트 (예: Light 2D)

    [SerializeField] private Player player; // 플레이어 오브젝트 참조

    void Start()
    {
        // 플레이어가 없으면 경고 출력
        if (player == null)
        {
            Debug.LogWarning("Player 오브젝트를 찾을 수 없습니다. 'Player' 태그를 확인하세요.");
            return;
        }

        // 처음에는 빛을 비활성화 상태로 시작 (필요에 따라 변경 가능)
        if (lightObject != null)
        {
            lightObject.SetActive(false);
        }
    }

    void Update()
    {
        // 플레이어가 없거나 빛 오브젝트가 없으면 실행 중단
        if (player == null || lightObject == null)
            return;

        // 현재 오브젝트와 플레이어 간의 거리 계산
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        // 거리가 activationDistance보다 작으면 빛 활성화, 아니면 비활성화
        if (distanceToPlayer <= activationDistance)
        {
            lightObject.SetActive(true);
        }
        else
        {
            lightObject.SetActive(false);
        }
    }

    // (선택 사항) Inspector에서 거리와 빛 오브젝트를 쉽게 확인할 수 있도록
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, activationDistance);
    }
}