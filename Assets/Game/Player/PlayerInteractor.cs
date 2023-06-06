using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInteractor : MonoBehaviour {
    [field: SerializeField] public float interactionRadius { get; private set; }
    [Header("Builder")]
    [SerializeField] private Material blueprintMaterial;
    [SerializeField] private float blueprintSnapSpeed;
    [SerializeField] private Color normalBlueprintColor;
    [SerializeField] private Color redBlueprintColor;
    [SerializeField] private Address address;
    private readonly Builder builder = new Builder();

    public void SetBuilderEntity(Entity entity) {
        builder.SetEntity(entity);
    }

    public void SubscribeOnBuildModeChange(UnityAction<bool> call) {
        builder.changedBuildMode.AddListener(call);
    }

    #region Handlers

    private void HandleClick() {
        if (ScreenUtils.IsMouseOverUI() || !IsAddressInInteractionRange()) return;
        if (Input.GetMouseButtonDown(0)) {
            if (builder.isInBuildMode) {
                if (builder.HandleBuilding()) {
                    Player.inst.animator.handsAnimator.InvokeActive(ScreenUtils.WorldMouse());
                }
                return;
            }
            HandleAction();
            
        }
        if (Input.GetKeyDown(KeyCode.E)) {
            HandleInteraction();
        }
    }

    private void HandleAction() {
        if (!IsAddressInInteractionRange()) return;
        List<GameObject> hitObjects = ScreenUtils.GetObjectsUnderMouse();
        for (int i = 0; i < hitObjects.Count; i++) {
            //May replace with responsibility chain
        }
    }

    private void HandleInteraction() {
        if (!IsAddressInInteractionRange()) return;
        List<GameObject> hitObjects = ScreenUtils.GetObjectsUnderMouse();
        for (int i = 0; i < hitObjects.Count; i++) {
            Interactable interactable = hitObjects[i].GetComponent<Interactable>();
            interactable?.Interact();
        }
    }

    #endregion

    public bool IsAddressInInteractionRange() {
        return Vector2.Distance(transform.position, World.inst.grid.GetAddressAtWorld(ScreenUtils.WorldMouse()).AsVector()) <= interactionRadius;
    }

    public bool IsInInteractionRange() {
        return Vector2.Distance(transform.position, ScreenUtils.WorldMouse()) <= interactionRadius;
    }

    private void LateUpdate() {
        HandleClick();
        builder.Update();
    }

    private void Awake() {
        builder.Init(blueprintMaterial, blueprintSnapSpeed, normalBlueprintColor, redBlueprintColor);
    }
}
