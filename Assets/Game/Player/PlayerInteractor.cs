using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour {
    [field: SerializeField] public float interactionRadius { get; private set; }
    [SerializeField] private Entity exampleEntity;
    private readonly Builder builder = new Builder();

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
            IInteractable interactable = hitObjects[i].GetComponent<IInteractable>();
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
        builder.Init();
        builder.SetEntity(exampleEntity);
    }
}
