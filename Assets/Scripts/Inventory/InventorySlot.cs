using UnityEngine;

[System.Serializable]
public class InventorySlot 
{
    public Item item;
    public int amount;

    public InventorySlot()
    {
        item = null;
        amount = 0;
    }
    
    public InventorySlot(Item item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }

    public bool IsEmpty()
    {
        return item == null;
    }
}
