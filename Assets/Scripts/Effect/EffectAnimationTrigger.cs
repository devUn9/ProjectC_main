using UnityEngine;

public class EffectAnimationTrigger : MonoBehaviour
{
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        anim.speed = TimeManager.Instance.timeScale;   
    }

    private void AnimationDistroyTrigger()
    {
        Destroy(gameObject);
    }
}
