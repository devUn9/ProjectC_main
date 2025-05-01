
using UnityEngine;

public class EffectTest : MonoBehaviour
{
    private GameObject player;

    void Start()
    {
        player = PlayerManager.instance.player;
    }


    void Update()
    {

        if (player != null)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Vector3 position = player.transform.position;
                EffectManager.Instance.PlayEffect(EffectType.SmokeShellEffect, position);

            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                Vector3 position = player.transform.position;

     
                Vector2 randomOffset = Random.insideUnitCircle * 0.15f ;

                Vector3 spawnPosition = position + new Vector3(randomOffset.x, randomOffset.y, 0f);

                EffectManager.Instance.PlayEffect(EffectType.BloodSplatterEffect, spawnPosition);
            }
        }
        else
        {
            Debug.LogWarning("플레이어를 찾을 수 없습니다.");
            player = PlayerManager.instance.player;
        }
    }

}

