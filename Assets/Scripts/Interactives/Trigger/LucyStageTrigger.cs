using Unity.VisualScripting;
using UnityEngine;

public class LucyStageTrigger : MonoBehaviour
{
    [SerializeField] private Player player; // PlayerSkill 참조
    [SerializeField] private MinigameObject minigameObject;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            minigameObject.TriggerOn();
        }
    }
}