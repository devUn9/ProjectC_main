using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, 
IEndDragHandler
{
    [SerializeField] private Image itemIcon;

    private InventorySlot slot;
    private int intslotIndex;

    public event Action<InventorySlotUI> OnSlotClicked;
    public static event Action<InventorySlotUI> OnBeginDragEvent;
    public static event Action<InventorySlotUI> OnEndDragEvent;

    void Start()
    {
        
    }

    
    public void UpdateSlot(InventorySlot newSlot)
    {
        slot = newSlot;

        if(slot.IsEmpty())
        {
            itemIcon.enabled= false;
        }
        else
        {
            itemIcon.sprite = slot.item.icon;
            itemIcon.enabled = true;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnSlotClicked?.Invoke(this);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        OnBeginDragEvent?.Invoke(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Handle dragging logic here if needed
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnEndDragEvent?.Invoke(this);
    }
}
