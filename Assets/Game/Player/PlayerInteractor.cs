using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour {
    [field: SerializeField] public float interactionRadius { get; private set; }
    [SerializeField] private Sprite wallSprite;
    public BuildRequirementsSO bsop;
    public static BuildRequirementsSO bso;
    public Sprite spritep;
    public static Sprite sprite;
    private readonly Builder builder = new Builder();

    private void OnValidate() {
        bso = bsop;
        sprite = spritep;
    }

    #region Handlers

    private void HandleClick() {
        if (ScreenUtils.IsMouseOverUI()) return;
        if (Input.GetMouseButtonDown(0)) {
            if (builder.isInBuildMode) {
                builder.HandleBuilding();
                return;
            }
            HandleAction();
        }
        if (Input.GetKeyDown(KeyCode.E)) {
            HandleInteraction();
        }
    }

    private void HandleAction() {
        if (!IsInInteractionRange()) return;
        List<GameObject> hitObjects = ScreenUtils.GetObjectsUnderMouse();
        for (int i = 0; i < hitObjects.Count; i++) {
            //May replace with responsibility chain
        }
    }

    private void HandleInteraction() {
        if (!IsInInteractionRange()) return;
        List<GameObject> hitObjects = ScreenUtils.GetObjectsUnderMouse();
        for (int i = 0; i < hitObjects.Count; i++) {
            Interactable interactable = hitObjects[i].GetComponent<Interactable>();
            interactable?.Interact();
        }
    }

    #endregion

    private bool IsInInteractionRange() {
        return Vector2.Distance(transform.position, ScreenUtils.WorldMouse()) <= interactionRadius;
    }

    private void Update() {
        HandleClick();
    }

    private void Start() {
        Wall exampleEntity = new Wall();
        Renderable renderable = exampleEntity.GetComponent<Renderable>();
        Positioned positioned = exampleEntity.GetComponent<Positioned>();
        renderable.sprite = wallSprite;
        positioned.size = new Vector2Int(1,1);
        renderable.visualSize = new Vector2(1, wallSprite.rect.size.y / wallSprite.rect.size.x);
        builder.SetEntity(exampleEntity);
        builder.Init();
    }
}
