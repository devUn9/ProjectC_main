using Unity.Cinemachine;
using UnityEngine;
using System.Collections;

public class Portal3 : MonoBehaviour
{
    [SerializeField] private Transform outPoint; // 플레이어가 도착할 위치
    [SerializeField] private GameObject player;
    [SerializeField] private CinemachineVirtualCameraBase virtualCamera; // Cinemachine 가상 카메라
    [SerializeField] private LayerMask mapLayerMask; // 맵이 속한 레이어만 필터링
    [SerializeField] private bool stateActive = false; // 포털 초기 상태 (활성화/비활성화)
    [SerializeField] private Material inactiveMaterial; // 비활성화 상태 메터리얼
    [SerializeField] private Material activeMaterial; // 활성화 상태 메터리얼
    [SerializeField] private BoxCollider2D targetBoundingShape;
    [SerializeField] private float inputCooldown = 0.5f; // Spacebar 입력 쿨다운
    [SerializeField] private DialogueManagerTest dialogueManager; // DialogueManagerTest 직접 참조
    [SerializeField] private float postDialogueDelay = 1f; // 대화 종료 후 포털 활성화 지연 시간

    public CinemachineConfiner2D confiner; // Cinemachine Confiner 2D 컴포넌트
    private bool playerIsInTrigger = false;
    private bool isTriggerActivated = false; // DialogueTrigger가 실행되었는지 확인
    private SpriteRenderer spriteRenderer; // 포털의 SpriteRenderer
    private GameObject inactiveMinimapGFX; // 비활성화 상태 미니맵 GFX
    private GameObject activeMinimapGFX; // 활성화 상태 미니맵 GFX
    private float lastInputTime = 0f; // 마지막 입력 시간
    private bool wasActiveBeforeDialogue = false; // 대화 시작 전 포털 활성화 상태 저장
    private float dialogueEndTime = -1f; // 대화가 끝난 시간 (unscaledTime)

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
        CinemachineUpdate();

        // DialogueManagerTest가 할당되었는지 확인
        if (dialogueManager == null)
        {
            Debug.LogError("DialogueManagerTest가 인스펙터에서 지정되지 않았습니다!", this);
        }
    }

    public void CinemachineUpdate()
    {
        // Cinemachine 설정
        if (virtualCamera != null)
        {
            confiner = virtualCamera.GetComponent<CinemachineConfiner2D>();
            if (confiner == null)
                Debug.LogError("CinemachineConfiner2D 컴포넌트가 가상 카메라에 없습니다!", virtualCamera);
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

    private void Update()
    {
        // 대화 상태에 따라 포털 활성화/비활성화 관리
        if (dialogueManager != null)
        {
            if (dialogueManager.isDialogueActive && isTriggerActivated)
            {
                // 대화 중: 포털 비활성화
                wasActiveBeforeDialogue = isTriggerActivated;
                DeactivatePortal();
            }
            else if (!dialogueManager.isDialogueActive && !isTriggerActivated && wasActiveBeforeDialogue && dialogueEndTime < 0f)
            {
                // 대화 종료: 지연 후 포털 재활성화 시작
                dialogueEndTime = Time.unscaledTime;
                StartCoroutine(DelayedPortalActivation());
            }
        }

        // 대화가 활성화된 경우 또는 대화 종료 후 지연 시간 내에는 포털 상호작용 무시
        if (playerIsInTrigger && isTriggerActivated && Input.GetKeyDown(KeyCode.Space)
            && Time.unscaledTime - lastInputTime >= inputCooldown
            && (dialogueManager == null || !dialogueManager.isDialogueActive)
            && (dialogueEndTime < 0f || Time.unscaledTime >= dialogueEndTime + postDialogueDelay))
        {
            lastInputTime = Time.unscaledTime;
            MovePlayer();
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
            

            if (activeMinimapGFX != null)
            {
                activeMinimapGFX.SetActive(true);
            }
           
        }
        else
        {
            // 비활성화 상태로 전환
            if (spriteRenderer != null && inactiveMaterial != null)
            {
                spriteRenderer.material = inactiveMaterial;
            }

            if (inactiveMinimapGFX != null)
            {
                inactiveMinimapGFX.SetActive(true);
            }
           
            if (activeMinimapGFX != null)
            {
                activeMinimapGFX.SetActive(false);
            }
            
        }
    }

    public void MovePlayer()
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

    public void AssignBoundingShapeFromOutPoint()
    {
        Collider2D hit = Physics2D.OverlapPoint(outPoint.position, mapLayerMask);
        if (hit != null && hit is BoxCollider2D box)
        {
            targetBoundingShape = box;
            //Debug.Log($"타겟 바운딩 셰이프 자동 할당: {box.name}");
        }
        else
        {
            //Debug.LogWarning("outPoint 위치에서 BoxCollider2D를 찾을 수 없습니다.", this);
        }
    }

    private IEnumerator DelayedPortalActivation()
    {
        yield return new WaitForSecondsRealtime(postDialogueDelay); // 1초 대기 (unscaled 시간)
        if (!dialogueManager.isDialogueActive && wasActiveBeforeDialogue)
        {
            ActivatePortal();
        }
        dialogueEndTime = -1f; // 대기 완료 후 초기화
    }

    private void OnValidate()
    {
        if (player == null) Debug.LogWarning("Player 오브젝트가 지정되지 않았습니다.", this);
        if (outPoint == null) Debug.LogWarning("OutPoint가 지정되지 않았습니다.", this);
        if (virtualCamera == null) Debug.LogWarning("Virtual Camera가 지정되지 않았습니다.", this);
        if (inactiveMaterial == null) Debug.LogWarning("Inactive Material이 지정되지 않았습니다.", this);
        if (activeMaterial == null) Debug.LogWarning("Active Material이 지정되지 않았습니다.", this);
        if (dialogueManager == null) Debug.LogWarning("DialogueManagerTest가 인스펙터에서 지정되지 않았습니다.", this);
    }
}