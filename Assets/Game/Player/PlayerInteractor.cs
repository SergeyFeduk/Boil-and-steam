using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private float interactionRadius;
    [SerializeField] private int baseAttackDamage;
    [SerializeField] private int baseMinePower;
    /*
    private Entity selectedEntity;

    public void SetSelectedEntity(Entity newEntity) {
        selectedEntity = newEntity;
    }
    private void Update() {
        HandleClick();
    }

    private void HandleClick() {
        if (ScreenUtils.IsMouseOverUI()) return;
        if (Input.GetMouseButtonDown(0)) {
            if (selectedEntity != null) {
                Buildable buildable = selectedEntity.GetComponent<Buildable>();
                if (buildable != null) {
                    for (int i = 0; i < buildable.requirements.items.Count; i++) {
                        bool value = Player.inst.inventory.CanItemsBeTaked(new Item(buildable.requirements.items[i], buildable.requirements.counts[i]));
                        if (!value) return;
                    }
                    for (int i = 0; i < buildable.requirements.items.Count; i++) {
                        Player.inst.inventory.DeleteItems(new Item(buildable.requirements.items[i], buildable.requirements.counts[i]));
                    }
                }
                World.inst.TryPlaceEntity(ScreenUtils.GetWorldMousePosition(), selectedEntity);
                return;
            }
            HandleAction();
        }
        if (Input.GetKeyDown(KeyCode.E)) {
            HandleInteraction();
        }
        if (Input.GetMouseButtonDown(1)) {
            selectedEntity = null;
        }
    }

    private void HandleAction() {
        List<GameObject> hitObjects = GetObjectsUnderMouseInRadius();
        for (int i = 0; i < hitObjects.Count; i++) {
            if (hitObjects[i].GetComponent<Damagable>() != null) { Attack(hitObjects[i].GetComponent<Damagable>()); break; }
            if (hitObjects[i].GetComponent<Minable>() != null) { Mine(hitObjects[i].GetComponent<Minable>()); break; }
        }
    }

    private void HandleInteraction() {
        List<GameObject> hitObjects = GetObjectsUnderMouseInRadius();
        for (int i = 0; i < hitObjects.Count; i++) {
            if (hitObjects[i].GetComponent<Interactable>() == null) continue;
            Interact(hitObjects[i].GetComponent<Interactable>());
        }
    }

    private List<GameObject> GetObjectsUnderMouseInRadius() {
        Vector2 mousePosition = ScreenUtils.GetWorldMousePosition();
        if (Vector2.Distance(transform.position, mousePosition) > interactionRadius) return new List<GameObject>();
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition, ScreenUtils.camera.transform.position - (Vector3)mousePosition);
        return hits.Select(i => i.collider.gameObject).ToList();
    }

    private void Attack(Damagable damagable) {
        damagable.Damage(baseAttackDamage);
    }

    private void Mine(Minable minable) {
        minable.Mine(baseMinePower);
    }

    private void Interact(Interactable interactable) {
        interactable.Interact();
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
    */
}