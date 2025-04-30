using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Transform slotGrid;
    [SerializeField] private GameObject slotPrefab;
    
    private Inventory playerInventory;
    private List<InventorySlotUI> slotUIs = new List<InventorySlotUI>();
    private InventorySlotUI selectedSlot;

    void Start()
    {
        playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();

        if(playerInventory != null)
        {
            playerInventory.OnInventoryChanged += UpdateInventoryUI;
            InitializeInventoryUI();
        }
        else
        {
            Debug.LogError("Player Inventory not found!");
        }
    }

    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    private void InitializeInventoryUI()
    {
        foreach(Transform child in slotGrid)
        {
            Destroy(child.gameObject);
        }
        slotUIs.Clear();

        for(int i = 0; i< playerInventory.GetInventorySize(); i++)
        {
            GameObject slotGO = Instantiate(slotPrefab, slotGrid);
            InventorySlotUI slotUI = slotGO.GetComponent<InventorySlotUI>();

            if(slotUI != null)
            {
                
            }
        }
        UpdateInventoryUI(playerInventory.GetInventorySlots());
    }

    private void UpdateInventoryUI(List<InventorySlot> slots)
    {
        for(int i = 0; i< slots.Count; i++)
        {
            if(i < slotUIs.Count)
            {
                slotUIs[i].UpdateSlot(slots[i]);
            }
        }
    }

    public void ToggleInventory()
    {
        bool isActive = !inventoryPanel.activeSelf;
        inventoryPanel.SetActive(isActive);
        
        Image inventoryImage = GetComponent<Image>();
        inventoryImage.enabled = isActive;
    }
}
