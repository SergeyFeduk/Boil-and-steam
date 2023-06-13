using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[System.Serializable]
public class Inventory
{
    public List<Item> myItems = new List<Item>();
    public UnityAction InventoryChanged;
    public Inventory(int capacity)
    {
        for(int i = 0; i < capacity; i++)
        {
            myItems.Add(Item.itemNull);
        }
    }
    public bool CheckIfEnoughItems(Item item)
    {
        int count = item.count;
        for (int i = 0; i < myItems.Count; i++)
        {
            if (myItems[i].data == item.data && myItems[i].count >= count)
            {
                return true;
            }
            else
            {
                count -= myItems[i].count;
            }
        }
        return false;
    }
    public bool CanFitItems(Item item) // Можно требовать больше стака предмета
    {
        int count = item.count;
        for (int i = 0; i < myItems.Count; i++)
        {
            if (myItems[i].data == item.data && (myItems[i].data.maxStack - myItems[i].count) >= count)
            {
                return true;
            }
            else if (myItems[i].data == item.data && (myItems[i].data.maxStack - myItems[i].count) < count)
            {
                count -= (myItems[i].data.maxStack - myItems[i].count);
            }
            if (myItems[i] == Item.itemNull)
            {
                if (item.data.maxStack < count)
                {
                    count -= item.data.maxStack;
                }
                else
                {
                    return true;
                }
            }
        }
        return false;
    }
    public void TakeItems(Item item)
    {
        int count = item.count;
        for (int i = 0; i < myItems.Count; i++)
        {
            if (myItems[i].data == item.data)
                if (myItems[i].count <= count)
                {
                    count -= myItems[i].count;
                    myItems[i] = Item.itemNull;
                }
                else
                {
                    myItems[i].count -= count;
                }
        }
        if (InventoryChanged != null) InventoryChanged.Invoke();
        if (count != 0) Debug.Log("Кто-то забрал из инвентаря не столько, сколько надо");
    }
    public int GetItemCount(ItemData data)
    {
        int count = 0;
        for (int i = 0; i < myItems.Count; i++)
        {
            if (myItems[i].data == data)
            {
                count += myItems[i].count;
            }
        }
        return count;
    }
    public int TryAddItems(Item item)
    {
       
        int countToFit = item.count;
        for (int i = 0; i < myItems.Count; i++)
        {
            if (myItems[i].data == item.data && (myItems[i].data.maxStack - myItems[i].count) >= countToFit)
            {
                myItems[i].count += countToFit;
                if (InventoryChanged != null) InventoryChanged.Invoke();
                return 0;
                
            }
            else if(myItems[i].data == item.data && (myItems[i].data.maxStack - myItems[i].count) < countToFit)
            {
                countToFit -= (myItems[i].data.maxStack - myItems[i].count);
                myItems[i].count = myItems[i].data.maxStack;
            }
        }
        for (int i = 0; i < myItems.Count; i++)
        {
            if (myItems[i] == Item.itemNull)
            {
                if (item.data.maxStack < countToFit)
                {
                    countToFit -= item.data.maxStack;
                    myItems[i] = new Item(item.data, item.data.maxStack);
                }
                else
                {
                    myItems[i] = new Item(item.data, countToFit);
                    if(InventoryChanged != null) InventoryChanged.Invoke();
                    return 0;
                }
            }

        }
        if (InventoryChanged != null) InventoryChanged.Invoke();
        return countToFit;
    }
    public void UpdateItemsFromSlots(List<Item> items)
    {
        for (int i = 0; i < myItems.Count; i++)
        {
            myItems[i] = items[i];
        }
        if (InventoryChanged != null) InventoryChanged.Invoke();
    }
}
class NameItemComparer : IComparer<Item>
{
    public int Compare(Item o1, Item o2)
    {
        return o1.data.itemName.CompareTo(o2.data.itemName);
    }
}
