using UnityEngine;

public class EnemySightTest : MonoBehaviour
{
    [SerializeField] private float detectionRange = 15f;
    private GameObject player;
    private EffectController sightEffect;
    private bool isPlayerInRange = false;

    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogWarning("플레이어를 찾을 수 없습니다.");
                return;
            }
        }

        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance <= detectionRange && !isPlayerInRange)
        {
            sightEffect = EffectManager.Instance.PlayEffectFollow(EffectType.EnemySightEffect, transform,new Vector3(0f,-1.2f,0f));
            isPlayerInRange = true;
        }
        else if (distance > detectionRange && isPlayerInRange)
        {
            sightEffect?.DestroyEffect(); 
            sightEffect = null;
            isPlayerInRange = false;
        }
    }
}