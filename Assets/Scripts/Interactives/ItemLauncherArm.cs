using UnityEngine;

public class ItemLauncherArm : MonoBehaviour
{
    [SerializeField] private LauncherArmSkill launcherSkill;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //launcherSkill.SetSkillEnabled(true);
            gameObject.SetActive(false); // 아이템 비활성화
        }
    }

}
