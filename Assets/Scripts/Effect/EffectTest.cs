
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
        }
        else
        {
            Debug.LogWarning("�÷��̾ ã�� �� �����ϴ�.");
            player = PlayerManager.instance.player;
        }
    }

}
