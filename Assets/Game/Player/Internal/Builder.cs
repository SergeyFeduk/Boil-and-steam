using System.Collections.Generic;
using UnityEngine;
public class Builder {
    public bool isInBuildMode { get; private set; }
    private Entity selectedEntity;

    public void SetEntity(Entity entity) {
        selectedEntity = entity;
    }

    public void Init() {
        isInBuildMode = true;
        /*for (int i = 0; i < 10000; i++) {
            HandleBuilding();
        }*/
    }

    #region BuildingHandlers

    public void HandleBuilding() {
        Positioned positioned = selectedEntity.GetComponent<Positioned>();
        if (positioned == null) {
            HandleFreeBuilding();
            return;
        }
        HandlePositionedBuilding(positioned);
    }

    private void HandlePositionedBuilding(Positioned positioned) {
        Address address = World.inst.grid.GetAddressAtWorld(ScreenUtils.WorldMouse());
        Address origin = address - GetPositionedOrigin(positioned);
        List<Cell> cells = new List<Cell>();
        for (int x = 0; x < positioned.size.x; x++) {
            for (int y = 0; y < positioned.size.y; y++) {
                Cell cell = World.inst.grid.GetCellAt(new Address(origin.x + x, origin.y + y));
                cells.Add(cell);
                if (cell.occupied) return; //Cannot build because not enough space
            }
        }
        Buildable buildable = selectedEntity.GetComponent<Buildable>();
        TakeItems(buildable);

        for (int i = 0; i < cells.Count; i++) {
            cells[i].SetOccupation(true);
        }
        World.inst.entityManager.InstantiateEntityAt(address, selectedEntity, false);
    }

    private void HandleFreeBuilding() {
        throw new System.NotImplementedException();
    }

    #endregion

    // ToDo: Move this somewhere else
    private Address GetPositionedOrigin(Positioned positioned) {
        int x = Mathf.FloorToInt(positioned.size.x / 2.0f);
        int y = Mathf.FloorToInt(positioned.size.y / 2.0f);
        return new Address(x, y);
    }

    private void TakeItems(Buildable buildable) {
        if (buildable == null || buildable.requirements == null) return;
        for (int i = 0; i < buildable.requirements.items.Count; i++) {
            //Check if enough items and grab them
        }
    }
}
