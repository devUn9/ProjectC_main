using UnityEngine;

[System.Serializable]
public class Item 
{
    public int id;
    public string itemName;
    public string description;
    public Sprite icon;
    public ItemType itemType;
    public bool isStackable;
    public int maxStackSize =1;
    
    public enum ItemType
    {
        Weapon
    }

    public Item(int id, string name, string desc, Sprite icon, ItemType type, bool stackale = false, 
    int maxStackSize = 1)
    {
        this.id = id;
        this.itemName = name;
        this.description = desc;
        this.icon = icon;
        this.itemType = type;
        this.isStackable = stackale;
        this.maxStackSize = maxStackSize;
    }
    
}
