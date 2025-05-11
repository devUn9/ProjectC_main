
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
            float angle = 0f;
            Vector3 position = player.transform.position;
            Quaternion rotation;


            if (Input.GetKeyDown(KeyCode.Y))
            {
     
                Vector2 randomOffset = Random.insideUnitCircle * 0.15f ;

                Vector3 spawnPosition = position + new Vector3(randomOffset.x, randomOffset.y, 0f);

                EffectManager.Instance.PlayEffect(EffectType.BloodSplatterEffect, spawnPosition);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0f;
                EffectManager.Instance.PlayEffect(EffectType.SmokeShellEffect, mousePos, 0.8f);

            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                rotation = Quaternion.Euler(0f, 0f, angle);
                EffectManager.Instance.PlayEffect(EffectType.SlashEffect, position, 1f, rotation);

            }

            if (Input.GetKeyDown(KeyCode.U))
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0f;

                EffectManager.Instance.PlayEffect(EffectType.GravitonSurgeEffect, mousePos, 0.2f);

            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0f;
                EffectManager.Instance.PlayEffect(EffectType.BulletHitEffect, mousePos, 1f);

            }


        }
        else
        {
            Debug.LogWarning("플레이어를 찾을 수 없습니다.");
            player = PlayerManager.instance.player;
        }
    }

}

