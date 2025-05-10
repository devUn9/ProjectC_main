using UnityEngine;

public class EnemyMeleeAttackController : MonoBehaviour
{
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        animator.speed = TimeManager.Instance.timeScale;
    }
}
