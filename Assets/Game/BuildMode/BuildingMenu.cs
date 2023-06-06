using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingMenu : MonoBehaviour {
    private const float subMenuIconSize = 100;
    [SerializeField] private List<BuildingSubmenu> subMenus = new List<BuildingSubmenu>();
    [Header("Imports")]
    [SerializeField] private GameObject subMenuPrefab;
    [SerializeField] private Transform subMenuHolder;
    [SerializeField] private Transform selectedMenuHolder;
    [SerializeField] private GameObject entityButtonPrefab;

    private EntitySerializer serializer = new EntitySerializer();
    private BuildingSubmenu selectedSubmenu;

    private void ChangeBuildMode(bool value) {
        subMenuHolder.gameObject.SetActive(value);
        selectedMenuHolder.gameObject.SetActive(value);
    }

    private void Init() {
        for (int i = 0; i < subMenus.Count; i++) {
            GameObject currentSubmenu = Instantiate(subMenuPrefab, subMenuHolder);
            InitSubmenu(subMenus[i], currentSubmenu, i);
        }
        LoadEntities();
        Player.inst.interactor.SubscribeOnBuildModeChange(ChangeBuildMode);
    }

    private void LoadEntities() {
        for (int i = 0; i < subMenus.Count; i++) {
            List<TextAsset> entityFiles = subMenus[i].buildngSubmenuData.entityFiles;
            subMenus[i].entities = new List<Entity>();
            for (int k = 0; k < entityFiles.Count; k++) {
                TextAsset entityFile = entityFiles[k];
                Entity entity = serializer.Deserialize(entityFile.text) as Entity;
                subMenus[i].entities.Add(entity);
            }
        }
    }

    private void SelectSubmenu(BuildingSubmenu submenu) {
        foreach (Transform child in selectedMenuHolder) {
            GameObject.Destroy(child.gameObject);
        }
        if (submenu == selectedSubmenu) {
            selectedSubmenu = null;
            return;
        }
        for (int i = 0; i < submenu.entities.Count; i++) {
            CreateEntityButton(submenu, i);
        }
        selectedSubmenu = submenu;
    }
    private void CreateEntityButton(BuildingSubmenu submenu, int index) {
        GameObject currentEntityButton = Instantiate(entityButtonPrefab, selectedMenuHolder);
        currentEntityButton.GetComponent<Button>().onClick.AddListener(() => {
            Player.inst.interactor.SetBuilderEntity(submenu.entities[index]);
        });
        Image entityImage = currentEntityButton.transform.GetChild(0).GetComponent<Image>();
        if (submenu.entities[index].icon == null) return;
        entityImage.sprite = submenu.entities[index].icon;
        float aspectRatio = entityImage.sprite.rect.size.y / entityImage.sprite.rect.size.x;
        entityImage.rectTransform.SetWidth(subMenuIconSize / aspectRatio);
    }

    private void InitSubmenu(BuildingSubmenu submenu, GameObject currentSubmenu, int index) {
        submenu.gameObject = currentSubmenu;
        submenu.image = currentSubmenu.transform.GetChild(0).GetComponent<Image>();
        submenu.button = currentSubmenu.GetComponent<Button>();
        Sprite sprite = submenu.buildngSubmenuData.icon;
        submenu.image.sprite = sprite;
        if (sprite != null) {
            float aspectRatio = sprite.rect.size.y / sprite.rect.size.x;
            submenu.image.rectTransform.SetWidth(subMenuIconSize / aspectRatio);
        }
        submenu.button.onClick.AddListener(() => {
            SelectSubmenu(subMenus[index]);
        });
    }

    private void Start() {
        Init();
    }
}
