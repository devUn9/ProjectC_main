using Unity.Cinemachine;
using UnityEngine;

public class Portal2 : MonoBehaviour
{
    [SerializeField] private Transform outPoint; // 플레이어가 도착할 위치
    [SerializeField] private GameObject player;
    [SerializeField] private CinemachineVirtualCameraBase virtualCamera; // Cinemachine 가상 카메라
    [SerializeField] private LayerMask mapLayerMask; // 맵이 속한 레이어만 필터링


    [Header("카메라 경계 자동 설정")]
    [SerializeField] private BoxCollider2D targetBoundingShape;

    private CinemachineConfiner2D confiner; // Cinemachine Confiner 2D 컴포넌트
    private bool playerIsInTrigger = false;
    private bool isTriggerActivated = false; // DialogueTrigger가 실행되었는지 확인

    private void Awake()
    {
        if (virtualCamera != null)
        {
            confiner = virtualCamera.GetComponent<CinemachineConfiner2D>();
            if (confiner == null)
                Debug.LogError("CinemachineConfiner2D 컴포넌트가 가상 카메라에 없습니다!");

            virtualCamera.Follow = player.transform;
        }
        else
        {
            Debug.LogError("Virtual Camera가 지정되지 않았습니다!");
        }
    }

    private void Start()
    {
        if (targetBoundingShape == null && outPoint != null)
        {
            AssignBoundingShapeFromOutPoint();
        }
    }

    private void AssignBoundingShapeFromOutPoint()
    {
        Collider2D hit = Physics2D.OverlapPoint(outPoint.position, mapLayerMask);
        if (hit != null && hit is BoxCollider2D box)
        {
            targetBoundingShape = box;
            Debug.Log("타겟 바운딩 셰이프 자동 할당: " + box.name);
        }
        else
        {
            Debug.LogWarning("outPoint 위치에서 BoxCollider2D를 찾을 수 없습니다.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == player)
            playerIsInTrigger = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == player)
            playerIsInTrigger = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MovePlayer();
        }
    }

    public void ActivatePortal()
    {
        isTriggerActivated = true;
        Debug.Log("포털이 활성화되었습니다.");
    }

    private void MovePlayer()
    {
        if (outPoint != null)
        {
            player.transform.position = outPoint.position;

            if (confiner != null && targetBoundingShape != null)
            {
                confiner.BoundingShape2D = targetBoundingShape;
                confiner.InvalidateBoundingShapeCache();
                virtualCamera.ForceCameraPosition(player.transform.position, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("Confiner 또는 Target Bounding Shape가 설정되지 않았습니다.");
            }
        }
    }
}