using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ItemData", menuName = "Resources/ItemData")]
public class ItemData : ScriptableObject
{
    public Sprite icon;
    public string itemName;
    public string description;
    public int maxStack;
}
