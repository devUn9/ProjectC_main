using UnityEngine;

public class EffectTest : MonoBehaviour
{

    void Start()
    {
        
    }


    void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null && Input.GetKeyDown(KeyCode.Q))
        {
            Vector3 position = player.transform.position;
            EffectManager.Instance.PlayEffect(EffectType.SmokeShellEffect, position);
        }
        else
        {
            Debug.LogWarning("�÷��̾ ã�� �� �����ϴ�.");
        }
    }
}
