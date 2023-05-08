using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPanel : MonoBehaviour
{
    [SerializeField] private List<InventorySlot> Slots = new List<InventorySlot>(9);
    void Start()
    {
        UpdateSlotsFromInventory();
    }
    private void UpdateSlotsFromInventory()
    {
        for (int i = 0; i < Slots.Count; i++)
        {
            Slots[i].PanelUpdateItem(Player.inst.inventory.myItems[i]);
        }
    }
    private void UpdateInventoryFromSlots()
    {
        List<Item> items = new List<Item>();
        foreach (InventorySlot slot in Slots) items.Add(slot.item);
        Player.inst.inventory.UpdateItemsFromSlots(items);
    }
    private void OnEnable()
    {
        PlayerInventory.InventoryChanged += UpdateSlotsFromInventory;
        foreach(InventorySlot slot in Slots)
        {
            slot.SlotItemChanged += UpdateInventoryFromSlots;
        }
    }
    private void OnDisable()
    {
        PlayerInventory.InventoryChanged -= UpdateSlotsFromInventory;
        foreach (InventorySlot slot in Slots)
        {
            slot.SlotItemChanged -= UpdateInventoryFromSlots;
        }
    }
}