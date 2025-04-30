using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int InventorySize = 20;
    [SerializeField] private List<InventorySlot> slots;
    public event Action<List<InventorySlot>> OnInventoryChanged;

    private void Awake()
    {
        InitializeInventory();
    }

    private void InitializeInventory()
    {
        slots = new List<InventorySlot>(InventorySize);
        for(int i = 0; i< InventorySize; i++)
        {
            slots.Add(new InventorySlot());
        }
    }

    public List<InventorySlot> GetInventorySlots()
    {
        return slots;
    }

    public int GetInventorySize()
    {
        return InventorySize;
    }
}
