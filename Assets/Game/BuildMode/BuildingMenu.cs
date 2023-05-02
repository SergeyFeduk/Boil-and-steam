using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingMenu : MonoBehaviour
{
    [SerializeField] private List<BuildingSubmenu> subMenus = new List<BuildingSubmenu>();
    [Header("Imports")]
    [SerializeField] private GameObject subMenuPrefab;
    [SerializeField] private Transform subMenuHolder;
    [SerializeField] private Transform selectedMenuHolder;
    [SerializeField] private GameObject entityButtonPrefab;


    private Dictionary<GameObject, BuildingSubmenu> subMenuObjects = new Dictionary<GameObject, BuildingSubmenu>();

    private void Start() {
        Init();
    }

    private void Init() {
        for (int i = 0; i < subMenus.Count; i++) {
            GameObject currentSubmenu = Instantiate(subMenuPrefab, subMenuHolder);
            currentSubmenu.GetComponent<Image>().sprite = subMenus[i].icon;
            int x = i;
            currentSubmenu.GetComponent<Button>().onClick.AddListener(() => {
                SelectSubmenu(subMenus[x]); 
            });

            subMenuObjects.Add(currentSubmenu, subMenus[i]);
        }
    }

    private void SelectSubmenu(BuildingSubmenu submenu) {
        foreach (Transform child in selectedMenuHolder) {
            GameObject.Destroy(child.gameObject);
        }
        for (int i = 0; i < submenu.entities.Count; i++) {
            GameObject currentEntityButton = Instantiate(entityButtonPrefab, selectedMenuHolder);
            
        }
    }
}
