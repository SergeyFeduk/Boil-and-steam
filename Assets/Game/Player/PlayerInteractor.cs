using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour {
    [field: SerializeField] public float interactionRadius { get; private set; }
    [SerializeField] private Entity exampleEntity;
    private Builder builder = new Builder();

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
        if (!isInRange()) return;
        List<GameObject> hitObjects = ScreenUtils.GetObjectsUnderMouse();
        for (int i = 0; i < hitObjects.Count; i++) {
            //May replace with responsibility chain
        }
    }

    private void HandleInteraction() {
        if (!isInRange()) return;
        List<GameObject> hitObjects = ScreenUtils.GetObjectsUnderMouse();
        for (int i = 0; i < hitObjects.Count; i++) {
            IInteractable interactable = hitObjects[i].GetComponent<IInteractable>();
            interactable?.Interact();
        }
    }

    private void Update() {
        HandleClick();
    }

    private void Start() {
        builder.SetEntity(exampleEntity);
    }
    private bool isInRange() {
        return Vector2.Distance(transform.position, ScreenUtils.WorldMouse()) <= interactionRadius;
    }
}
