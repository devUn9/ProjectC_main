using UnityEngine;

public class PortalActivationTrigger : MonoBehaviour
{
    [SerializeField] private Portal3 targetPortal; // 연결된 포털
    [SerializeField] private bool disableAfterTrigger = true; // true면 재사용 불가
    [SerializeField] private LayerMask enemyLayer; // 몬스터 레이어 마스크
    [SerializeField] private BoxCollider2D mapCollider; // 정사각형 맵의 BoxCollider2D

    private bool isTriggered = false; // 트리거가 이미 실행되었는지 확인
    private bool allEnemyCleared = false; // 몬스터가 모두 제거되었는지 여부

    private void Awake()
    {
        // mapCollider가 지정되지 않은 경우, 자체 BoxCollider2D 사용
        if (mapCollider == null)
        {
            mapCollider = GetComponent<BoxCollider2D>();
            if (mapCollider == null)
            {
                Debug.LogError("PortalActivationTrigger에 BoxCollider2D가 없습니다!", this);
            }
        }
    }

    //private void OnEnable()
    //{
    //    // 몬스터 제거 이벤트 구독
    //    Enemy.OnEnemyRemoved += CheckMonstersCleared;
    //}

    //private void OnDisable()
    //{
    //    // 몬스터 제거 이벤트 구독 해제
    //    Enemy.OnEnemyRemoved -= CheckMonstersCleared;
    //}

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어가 트리거 영역에 들어왔는지 확인
        if (other.CompareTag("Player") && !isTriggered)
        {
            TryActivatePortal();
        }
    }

    private void CheckMonstersCleared()
    {
        // 몬스터가 모두 제거되었는지 확인
        allEnemyCleared = !AreMonstersInMap();
        if (allEnemyCleared && !isTriggered)
        {
            TryActivatePortal();
        }
    }

    private bool AreMonstersInMap()
    {
        if (mapCollider == null) return false;

        // 맵 영역 내 몬스터 레이어 오브젝트 확인
        Collider2D[] colliders = Physics2D.OverlapBoxAll(
            mapCollider.bounds.center,
            mapCollider.bounds.size,
            0f,
            enemyLayer
        );

        // 몬스터가 하나라도 있으면 true 반환
        return colliders.Length > 0;
    }

    private void TryActivatePortal()
    {
        // 포털이 지정되어 있고, 비활성화 상태이며, 몬스터가 없으면 활성화
        if (targetPortal != null && !targetPortal.IsActive && allEnemyCleared)
        {
            targetPortal.ActivatePortal();
            if (disableAfterTrigger)
            {
                isTriggered = true; // 재실행 방지
                gameObject.SetActive(false);
                Debug.Log("트리거가 비활성화되었습니다.");
            }
        }
        else if (targetPortal == null)
        {
            Debug.LogWarning("Target Portal이 지정되지 않았습니다.", this);
        }
    }

    private void OnValidate()
    {
        // Inspector에서 필수 참조가 누락되었는지 확인
        if (targetPortal == null)
            Debug.LogWarning("PortalActivationTrigger에 targetPortal이 지정되지 않았습니다.", this);
        if (enemyLayer == 0)
            Debug.LogWarning("PortalActivationTrigger에 몬스터 레이어가 지정되지 않았습니다.", this);
        if (mapCollider == null)
            Debug.LogWarning("PortalActivationTrigger에 mapCollider가 지정되지 않았습니다.", this);
    }
}