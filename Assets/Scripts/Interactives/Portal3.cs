using Unity.Cinemachine;
using UnityEngine;

public class Portal3 : MonoBehaviour
{
    [SerializeField] private Transform outPoint; // 플레이어가 도착할 위치
    [SerializeField] private GameObject player;
    [SerializeField] private CinemachineVirtualCameraBase virtualCamera; // Cinemachine 가상 카메라
    [SerializeField] private LayerMask mapLayerMask; // 맵이 속한 레이어만 필터링

    [Header("포털 상호작용 가능?, 활성화 상태")]
    [SerializeField] private bool stateActive = false; // 포털 초기 상태 (활성화/비활성화)

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
    private float lastInputTime = 0f; // 마지막 입력 시간
    [SerializeField] private float inputCooldown = 0.5f; // Spacebar 입력 쿨다운

    public bool IsActive => stateActive;

    private void Awake()
    {
        // SpriteRenderer 초기화
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer 컴포넌트가 포털에 없습니다!", this);
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
        isTriggerActivated = stateActive;
        UpdatePortalVisuals(stateActive);

        // Cinemachine 설정
        if (virtualCamera != null)
        {
            confiner = virtualCamera.GetComponent<CinemachineConfiner2D>();
            if (confiner == null)
                Debug.LogError("CinemachineConfiner2D 컴포넌트가 가상 카메라에 없습니다!", this);

            virtualCamera.Follow = player.transform;
        }
        else
        {
            Debug.LogError("Virtual Camera가 지정되지 않았습니다!", this);
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
            Debug.Log($"타겟 바운딩 셰이프 자동 할당: {box.name}");
        }
        else
        {
            Debug.LogWarning("outPoint 위치에서 BoxCollider2D를 찾을 수 없습니다.", this);
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
        // Time.unscaledTime을 사용하여 Time.timeScale = 0에서도 입력 처리
        if (playerIsInTrigger && isTriggerActivated && Input.GetKeyDown(KeyCode.Space) && Time.unscaledTime - lastInputTime >= inputCooldown)
        {
            lastInputTime = Time.unscaledTime;
            MovePlayer();
        }
    }

    public void ActivatePortal()
    {
        if (isTriggerActivated) return; // 이미 활성화된 경우 무시

        isTriggerActivated = true;
        stateActive = true; // 활성화 상태로 전환
        UpdatePortalVisuals(true);
        Debug.Log("포털이 활성화되었습니다.");
    }

    public void DeactivatePortal()
    {
        if (!isTriggerActivated) return; // 이미 비활성화된 경우 무시

        isTriggerActivated = false;
        stateActive = false; // 비활성화 상태로 전환
        UpdatePortalVisuals(false);
        Debug.Log("포털이 비활성화되었습니다.");
    }

    private void UpdatePortalVisuals(bool active)
    {
        if (active)
        {
            // 활성화 상태로 전환
            if (spriteRenderer != null && activeMaterial != null)
            {
                spriteRenderer.material = activeMaterial;
            }
            else
            {
                Debug.LogWarning("Active Material이 지정되지 않았습니다.", this);
            }

            if (inactiveMinimapGFX != null)
            {
                inactiveMinimapGFX.SetActive(false);
            }
            else
            {
                Debug.LogWarning("Minimap_Portal_Red_GFX 오브젝트를 자식에서 찾을 수 없습니다.", this);
            }

            if (activeMinimapGFX != null)
            {
                activeMinimapGFX.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Minimap_Portal_Green_GFX 오브젝트를 자식에서 찾을 수 없습니다.", this);
            }
        }
        else
        {
            // 비활성화 상태로 전환
            if (spriteRenderer != null && inactiveMaterial != null)
            {
                spriteRenderer.material = inactiveMaterial;
            }
            else
            {
                Debug.LogWarning("Inactive Material이 지정되지 않았습니다.", this);
            }

            if (inactiveMinimapGFX != null)
            {
                inactiveMinimapGFX.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Minimap_Portal_Red_GFX 오브젝트를 자식에서 찾을 수 없습니다.", this);
            }

            if (activeMinimapGFX != null)
            {
                activeMinimapGFX.SetActive(false);
            }
            else
            {
                Debug.LogWarning("Minimap_Portal_Green_GFX 오브젝트를 자식에서 찾을 수 없습니다.", this);
            }
        }
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
                Debug.LogWarning("Confiner 또는 Target Bounding Shape가 설정되지 않았습니다.", this);
            }
        }
        else
        {
            Debug.LogWarning("OutPoint가 지정되지 않았습니다.", this);
        }
    }

    private void OnValidate()
    {
        if (player == null) Debug.LogWarning("Player 오브젝트가 지정되지 않았습니다.", this);
        if (outPoint == null) Debug.LogWarning("OutPoint가 지정되지 않았습니다.", this);
        if (virtualCamera == null) Debug.LogWarning("Virtual Camera가 지정되지 않았습니다.", this);
        if (inactiveMaterial == null) Debug.LogWarning("Inactive Material이 지정되지 않았습니다.", this);
        if (activeMaterial == null) Debug.LogWarning("Active Material이 지정되지 않았습니다.", this);
    }
}