using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuildingSubmenu
{
    [field: SerializeField] public string name { get; set; }
    [field: SerializeField] public Sprite icon { get; set; }
    [field: SerializeField] public List<int> entities { get; set; }
}
