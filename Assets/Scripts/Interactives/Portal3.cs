using Unity.Cinemachine;
using UnityEngine;

public class Portal3 : MonoBehaviour
{
    [SerializeField] private Transform outPoint; // 플레이어가 도착할 위치
    [SerializeField] private GameObject player;
    [SerializeField] private CinemachineVirtualCameraBase virtualCamera; // Cinemachine 가상 카메라
    [SerializeField] private LayerMask mapLayerMask; // 맵이 속한 레이어만 필터링

    [Header("포털 활성화 상태")]
    [SerializeField] private bool startActive = false; // 포털 초기 상태 (활성화/비활성화)

    [Header("포털 메터리얼")]
    [SerializeField] private Material inactiveMaterial; // 비활성화 상태 메터리얼
    [SerializeField] private Material activeMaterial; // 활성화 상태 메터리얼

    [Header("카메라 경계는 자동 설정됨")]
    [SerializeField] private BoxCollider2D targetBoundingShape;

    private CinemachineConfiner2D confiner; // Cinemachine Confiner 2D 컴포넌트
    private bool playerIsInTrigger = false;
    private bool isTriggerActivated = false; // DialogueTrigger가 실행되었는지 확인
    private SpriteRenderer spriteRenderer; // 포털의 SpriteRenderer
    private GameObject inactiveMinimapGFX; // 비활성화 상태 미니맵 GFX
    private GameObject activeMinimapGFX; // 활성화 상태 미니맵 GFX

    private void Awake()
    {
        // SpriteRenderer 초기화
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer 컴포넌트가 포털에 없습니다!");
        }

        // 자식 오브젝트에서 미니맵 GFX 찾기
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            if (child.gameObject.name == "Minimap_Portal_Red_GFX")
            {
                inactiveMinimapGFX = child.gameObject;
            }
            else if (child.gameObject.name == "Minimap_Portal_Green_GFX")
            {
                activeMinimapGFX = child.gameObject;
            }
        }

        // 초기 상태 설정
        isTriggerActivated = startActive;
        if (startActive)
        {
            // 활성화 상태 초기화
            if (spriteRenderer != null && activeMaterial != null)
            {
                spriteRenderer.material = activeMaterial;
            }
            else
            {
                Debug.LogWarning("Active Material이 지정되지 않았습니다.");
            }

            if (inactiveMinimapGFX != null)
            {
                inactiveMinimapGFX.SetActive(false);
            }
            else
            {
                Debug.LogWarning("Minimap_Portal_Red_GFX 오브젝트를 자식에서 찾을 수 없습니다.");
            }

            if (activeMinimapGFX != null)
            {
                activeMinimapGFX.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Minimap_Portal_Green_GFX 오브젝트를 자식에서 찾을 수 없습니다.");
            }
        }
        else
        {
            // 비활성화 상태 초기화
            if (spriteRenderer != null && inactiveMaterial != null)
            {
                spriteRenderer.material = inactiveMaterial;
            }
            else
            {
                Debug.LogWarning("Inactive Material이 지정되지 않았습니다.");
            }

            if (inactiveMinimapGFX != null)
            {
                inactiveMinimapGFX.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Minimap_Portal_Red_GFX 오브젝트를 자식에서 찾을 수 없습니다.");
            }

            if (activeMinimapGFX != null)
            {
                activeMinimapGFX.SetActive(false);
            }
            else
            {
                Debug.LogWarning("Minimap_Portal_Green_GFX 오브젝트를 자식에서 찾을 수 없습니다.");
            }
        }

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
        if (playerIsInTrigger && isTriggerActivated && Input.GetKeyDown(KeyCode.Space))
        {
            MovePlayer();
        }
    }

    public void ActivatePortal()
    {
        if (isTriggerActivated) return; // 이미 활성화된 경우 무시

        isTriggerActivated = true;

        // 메터리얼 변경
        if (spriteRenderer != null && activeMaterial != null)
        {
            spriteRenderer.material = activeMaterial; // 활성화 메터리얼로 변경
        }
        else
        {
            Debug.LogWarning("Active Material이 지정되지 않았습니다.");
        }

        // 미니맵 GFX 상태 변경
        if (inactiveMinimapGFX != null)
        {
            inactiveMinimapGFX.SetActive(false); // 비활성화 상태 GFX 비활성화
        }
        if (activeMinimapGFX != null)
        {
            activeMinimapGFX.SetActive(true); // 활성화 상태 GFX 활성화
        }

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