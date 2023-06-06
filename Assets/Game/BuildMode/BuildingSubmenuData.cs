using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "BuildingSubmenuData", menuName = "ScriptableObjects/BuildingSubmenuData")]
public class BuildingSubmenuData : ScriptableObject {
    [field: SerializeField] public string label { get; set; }
    [field: SerializeField] public Sprite icon { get; set; }
    [field: SerializeField] public List<TextAsset> entityFiles { get; set; }
}

[System.Serializable]
public class BuildingSubmenu
{
    public Image image { get; set; }
    public Button button { get; set; }
    public GameObject gameObject { get; set; }
    public List<Entity> entities { get; set; }
    [field: SerializeField] public BuildingSubmenuData buildngSubmenuData { get; set; }
}

