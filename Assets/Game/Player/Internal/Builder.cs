using System.Collections.Generic;
using UnityEngine;
public class Builder 
{
    public bool isInBuildMode = true;
    private Entity selectedEntity;

    public void SetEntity(Entity entity) {
        selectedEntity = entity;
    }

    public void HandleBuilding() {
        IPositioned positioned = selectedEntity.GetComponent<IPositioned>();
        if (positioned == null) {
            HandleFreeBuilding();
        } else {
            HandlePositionedBuilding(positioned);
        }
        
    }

    private void HandlePositionedBuilding(IPositioned positioned) {
        Address address = World.inst.grid.GetAddressAtWorld(ScreenUtils.WorldMouse());
        Address origin = address - GetPositionedOrigin(positioned);
        List<Cell> cells = new List<Cell>();
        for (int x = 0; x < positioned.size.x; x++) {
            for (int y = 0; y < positioned.size.y; y++) {
                Address cellAddress = new Address(origin.x + x, origin.y + y);
                Cell cell = World.inst.grid.GetCellAt(cellAddress);
                cells.Add(cell);
                if (cell.occupied) return; //Cannot build because not enough space
            }
        }
        IBuildable buildable = selectedEntity.GetComponent<IBuildable>();
        TakeItems(buildable);

        //Build building very very cringe
        for (int i = 0; i < cells.Count; i++) {
            cells[i].SetOccupation(true);
        }
        World.inst.grid.PlaceEntityAt(address, selectedEntity);
    }

    // ToDo: Move this the fuck out of this place
    private Address GetPositionedOrigin(IPositioned positioned) {
        int x = Mathf.FloorToInt(positioned.size.x / 2.0f);
        int y = Mathf.FloorToInt(positioned.size.y / 2.0f);
        return new Address(x, y);
    }

    private void HandleFreeBuilding() {
        throw new System.NotImplementedException();
    }

    private void TakeItems(IBuildable buildable) {
        if (buildable == null || buildable.requirements == null) return;
        for (int i = 0; i < buildable.requirements.items.Count; i++) {
            //Check if enough items and grab them
        }
    }
}
