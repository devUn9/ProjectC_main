using UnityEngine;

public class PlayerInventoryController : MonoBehaviour
{
    private Inventory inventory;

    private void Awake()
    {
        inventory = GetComponent<Inventory>();
        if(inventory == null)
        {
            inventory = gameObject.AddComponent<Inventory>();
        }
    }

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}
