using UnityEngine;
using System.Collections;

public class NPCPortalTrigger : MonoBehaviour
{
    [SerializeField] private Portal3 targetPortal; // 연결된 포털
    

    private bool isPlayerInTrigger = false; // 플레이어가 트리거 안에 있는지
    private bool isTriggered = false; // 트리거가 이미 실행되었는지

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어가 트리거 영역에 들어왔는지 확인
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 플레이어가 트리거 영역에서 나갔는지 확인
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
        }
    }

    private void Update()
       
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            // 플레이어가 트리거 안에 있고, 스페이스바를 누르면 포털 활성화 시도
            if (isPlayerInTrigger && !isTriggered)
            {
                targetPortal.ActivatePortal();
            }
        }
       
    }

  

   

    private void OnValidate()
    {
        // Inspector에서 포털이 지정되었는지 확인
        if (targetPortal == null)
        {
            Debug.LogWarning("NPCPortalTrigger에 targetPortal이 지정되지 않았습니다.", this);
        }
     
    }
}