using UnityEngine;

public class ItemSandevistan : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            Player player = collider.GetComponent<Player>();
            player.skill.isSandevistanUsable = true; // 플레이어의 스킬 사용 가능 상태를 true로 설정

            //launcherSkill.SetSkillEnabled(true);
            gameObject.SetActive(false); // 아이템 비활성화
        }
    }
}
