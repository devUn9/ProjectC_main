using System;
using UnityEngine;

public class Stage_Selector : MonoBehaviour
{
    public float interactionRange = 3.0f;
    public Transform player;
    public GameObject Interactive_Key;
    public GameObject StageSelect_UI;

    private bool isPlayerNearby = false;
    
    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        isPlayerNearby = distance <= interactionRange;

        InteractiveKeySet();

        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    private void InteractiveKeySet()
    {
        if(isPlayerNearby)
            Interactive_Key.SetActive(true);
        else
            Interactive_Key.SetActive(false);
    }

    private void Interact()
    {
        if (Interactive_Key != null)
        {
            StageSelect_UI.SetActive(true);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
