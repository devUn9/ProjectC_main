using UnityEngine;

public class EnemyCheckObjectActivator : MonoBehaviour
{
    [SerializeField] private GameObject targetObject; // 활성화할 대상 오브젝트
    [SerializeField] private bool disableAfterTrigger = true; // true면 트리거 재사용 불가
    [SerializeField] private LayerMask enemyLayer; // 몬스터 레이어 마스크
    [SerializeField] private BoxCollider2D mapCollider; // 맵 영역의 BoxCollider2D
    [SerializeField] private float checkInterval = 1f; // 몬스터 확인 간격 (초)

    private bool isTriggered = false; // 트리거가 이미 실행되었는지 여부
    private bool allEnemyCleared = false; // 모든 몬스터가 제거되었는지 여부
    private float checkTimer = 0f; // 몬스터 확인 타이머

    private void Awake()
    {
        // mapCollider가 지정되지 않은 경우, 현재 오브젝트의 BoxCollider2D 사용
        if (mapCollider == null)
        {
            mapCollider = GetComponent<BoxCollider2D>();
            if (mapCollider == null)
            {
                Debug.LogError("EnemyCheckObjectActivator에 BoxCollider2D가 없습니다!", this);
            }
        }
    }

    private void Start()
    {
        // 게임 시작 시 몬스터 상태 확인
        CheckMonstersCleared();
    }

    private void Update()
    {
        // 트리거가 실행되지 않았고 몬스터가 모두 제거되지 않은 경우 주기적으로 확인
        if (!isTriggered && !allEnemyCleared)
        {
            checkTimer += Time.deltaTime;
            if (checkTimer >= checkInterval)
            {
                checkTimer = 0f;
                CheckMonstersCleared();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어가 트리거 영역에 들어왔는지 확인
        if (other.CompareTag("Player") && !isTriggered)
        {
            TryActivateObject();
        }
    }

    private void CheckMonstersCleared()
    {
        // 모든 몬스터가 제거되었는지 확인
        allEnemyCleared = !AreMonstersInMap();
        if (allEnemyCleared && !isTriggered)
        {
            TryActivateObject();
        }
    }

    private bool AreMonstersInMap()
    {
        if (mapCollider == null) return false;

        // 맵 영역 내 몬스터 레이어의 오브젝트 확인
        Collider2D[] colliders = Physics2D.OverlapBoxAll(
            mapCollider.bounds.center,
            mapCollider.bounds.size,
            0f,
            enemyLayer
        );

        // 몬스터가 하나라도 있으면 true 반환
        return colliders.Length > 0;
    }

    private void TryActivateObject()
    {
        // 대상 오브젝트가 지정되어 있고, 비활성화 상태이며, 몬스터가 모두 제거된 경우 활성화
        if (targetObject != null && !targetObject.activeSelf && allEnemyCleared)
        {
            targetObject.SetActive(true);
            if (disableAfterTrigger)
            {
                isTriggered = true; // 트리거 재실행 방지
                gameObject.SetActive(false);
                Debug.Log("트리거가 비활성화되었습니다.");
            }
        }
        else if (targetObject == null)
        {
            Debug.LogWarning("대상 오브젝트가 지정되지 않았습니다.", this);
        }
    }

}