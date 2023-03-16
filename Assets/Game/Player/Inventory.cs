using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    /*
    [SerializeField] private List<Item> myItems = new List<Item>(6);
    [SerializeField] private InventoryUICell[] cells = new InventoryUICell[6];
    [SerializeField] private ItemSO emptyData;
    private Item emptyItem;
    private void Start()
    {
        emptyItem = new Item(emptyData);
        for(int i = 0; i < 6; i++)
        {
            myItems.Add(emptyItem);
        }
        UpdateUI();
    }

    public bool CanItemBeAdded(ItemSO data)
    {
        for (int i = 0; i < myItems.Count; i++)
        {
            if (myItems[i].data == data && myItems[i].count != myItems[i].data.stack)
            {
                return true;
            }
        }
        if(myItems.Contains(emptyItem))
        {
            return true;
        }
        return false;
    }
    public void AddItem(Item item)
    {
        bool T = true;
        for(int i = 0; i < myItems.Count; i++)
        {
            if (myItems[i].data == item.data && myItems[i].count != myItems[i].data.stack)
            {
                myItems[i].ChangeCount(1);
                T = false;
                break;
            }
        }
        if(T)
        {
            myItems.Remove(emptyItem);
            myItems.Add(item);
        }
        UpdateUI();
    }
    public bool CanItemsBeTaked(Item item)
    {
        int howmanyneedtotake = item.count;
        for (int i = 0; i < myItems.Count; i++)
        {
            if (myItems[i].data == item.data)
            {
                howmanyneedtotake -= myItems[i].count;
            }
        }
        return howmanyneedtotake <= 0;
    }
    public void DeleteItems(Item item)
    {
        int needs = item.count;
        for (int i = 0; i < myItems.Count; i++)
        {
            if (myItems[i].data == item.data)
            {
                if(needs - myItems[i].count <= 0)
                {
                    myItems[i].ChangeCount(-needs);
                    break;
                }
                else
                {
                    needs -= myItems[i].count;
                    myItems[i].ChangeCount(-myItems[i].count);
                }
            }
        }
        UpdateUI();
    }
    public void UpdateUI()
    {
        myItems.Sort(new NameItemComparer());
        for(int j = 0; j < myItems.Count; j++)
        {
            if(myItems[j].count == 0)
            {
                myItems[j] = emptyItem;
            }
        }
        for(int i = 0; i < myItems.Count; i++)
        {
            cells[i].SetItem(myItems[i]);
            cells[i].UpdateImage();
        }
    }
}
public class Item
{
    public readonly ItemSO data;
    public int count { private set; get; }
    public void ChangeCount(int value)
    {
        if(count + value > data.stack)
        {
            count = data.stack;
        }
        else if(count - value < 0)
        {
            count = 0;
        }
        else
        {
            count += value;
        }
    }
    public Item(ItemSO itemData, int CoUnt = 1)
    {
        data = itemData;
        count = CoUnt;
    }
}
class NameItemComparer : IComparer<Item>
{
    public int Compare(Item o1, Item o2)
    {
        return o1.data.itemName.CompareTo(o2.data.itemName);
    }
    */
}
