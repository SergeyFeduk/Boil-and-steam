using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    private const int stackForNullItem = 0;
    private int startCapacity = 9;
    public List<Item> myItems = new List<Item>();

    public static UnityAction InventoryChanged;
    private void Start()
    {
        //for (int i = 0; i < startCapacity; i++) myItems.Add(Item.itemNull); //Тут возможно загрузить информацию из сохранения
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if(!ContextCanvasManager.inst.CheckPanel(panel.name))
            {
                ContextCanvasManager.inst.CreatePanel(panel);
            }
            else
            {
                ContextCanvasManager.inst.DestroyPanel(panel.name);
            }
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
            else
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
                if(myItems[i].count <= count)
                {
                    count -= myItems[i].count;
                    myItems[i] = Item.itemNull;
                }
                else
                {
                    myItems[i].count -= count;
                }
        }
        InventoryChanged.Invoke();
        if (count != 0) print("У игрока было недостаточно предметом, чтобы отнять их >:(");
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
                InventoryChanged.Invoke();
                return 0;
            }
            else
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
                    myItems[i] = new Item(item.data,item.data.maxStack);
                }
                else
                {
                    myItems[i] = new Item(item.data, countToFit);
                    InventoryChanged.Invoke();
                    return 0;
                }
            }

        }
        InventoryChanged.Invoke();
        return countToFit;
    }
    public void UpdateItemsFromSlots(List<Item> items)
    {
        for (int i = 0; i < myItems.Count; i++) 
        {
            myItems[i] = items[i];
        }
        InventoryChanged.Invoke();
    }
}
 
class NameItemComparer : IComparer<Item>
{
    public int Compare(Item o1, Item o2)
    {
        return o1.data.itemName.CompareTo(o2.data.itemName);
    }
}