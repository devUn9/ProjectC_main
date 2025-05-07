
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

            if (Input.GetKeyDown(KeyCode.E))
            {
                Vector3 position = player.transform.position;

     
                Vector2 randomOffset = Random.insideUnitCircle * 0.15f ;

                Vector3 spawnPosition = position + new Vector3(randomOffset.x, randomOffset.y, 0f);

                EffectManager.Instance.PlayEffect(EffectType.BloodSplatterEffect, spawnPosition);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0f; // 2D 게임이므로 z값은 0으로 고정

                EffectManager.Instance.PlayEffect(EffectType.SmokeShellEffect, mousePos, 3.0f);

            }

        }
        else
        {
            Debug.LogWarning("플레이어를 찾을 수 없습니다.");
            player = PlayerManager.instance.player;
        }
    }

}

