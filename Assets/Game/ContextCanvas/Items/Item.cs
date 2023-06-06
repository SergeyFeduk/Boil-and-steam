using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[System.Serializable]
public class Item
{
    public ItemData data;
    public int count;
    public static Item itemNull = new Item(null,0);
    public Item(ItemData d, int c)
    {
        data = d;
        count = c;
    }
}