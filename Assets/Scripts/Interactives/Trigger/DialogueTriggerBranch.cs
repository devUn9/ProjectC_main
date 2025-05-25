using Unity.VisualScripting;
using UnityEngine;

public class DialogueTriggerBranch : MonoBehaviour
{
    [SerializeField] private DialogueManagerTest dialogueManagerTest;
    [SerializeField] private bool disableAfterTrigger = true; // true면 재사용 불가
    [SerializeField] private int csvFileIndexNormal = 0; // Graviton 비활성 시 사용할 CSV 파일 인덱스
    [SerializeField] private int csvFileIndexGraviton = 0; // Graviton 활성 시 사용할 CSV 파일 인덱스
    [SerializeField] private Player player; // PlayerSkill 참조



   


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // playerSkill.isGravitonUsable에 따라 CSV 인덱스 선택
            int selectedCsvIndex = player.skill.isGravitonUsable ? csvFileIndexGraviton : csvFileIndexNormal;
            dialogueManagerTest.StartDialogue(selectedCsvIndex); // 선택된 CSV 인덱스 전달

            if (disableAfterTrigger)
            {
                Destroy(gameObject);
              
                Debug.Log("트리거를 제거.");
            }
        }
    }
}