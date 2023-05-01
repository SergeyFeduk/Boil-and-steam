using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public ItemData data;
    public int count;
    public Item()
    {
        data = null;
        count = 0;
    }
    public Item(ItemData d, int c)
    {
        data = d;
        count = c;
    }
}
