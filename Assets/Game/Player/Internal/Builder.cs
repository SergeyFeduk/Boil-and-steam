using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Builder {
    public bool isInBuildMode { get; private set; }
    public UnityEvent<bool> changedBuildMode;

    private Entity selectedEntity;
    private Mesh quadMesh;
    private Material material;
    private Color normalBlueprintColor;
    private Color redBlueprintColor;
    MaterialPropertyBlock mpb;

    private Vector2 blueprintPosition, blueprintTargetPosition;
    private float blueprintSnapSpeed;

    public void SetEntity(Entity entity) {
        selectedEntity = entity;
        blueprintPosition = blueprintTargetPosition = ScreenUtils.WorldMouse();
        Renderable renderable = selectedEntity.GetComponent<Renderable>();
        if (renderable == null) return;
        renderable.RecalculateVisualSize();
    }


    public void Init(Material material, float blueprintSnapSpeed, Color normalBlueprintColor, Color redBlueprintColor) {
        mpb = new MaterialPropertyBlock();
        this.material = material;
        this.blueprintSnapSpeed = blueprintSnapSpeed;
        this.normalBlueprintColor = normalBlueprintColor;
        this.redBlueprintColor = redBlueprintColor;

        quadMesh = MeshUtils.CreateMesh();
        MeshUtils.CreateMeshArrays(1, out Vector3[] verticies, out int[] triangles, out Vector2[] uvs);
        MeshUtils.AddToMeshArrays(verticies, triangles, uvs, 0, Vector3.one, 0, Vector3.one, Vector2.zero, Vector2.one);
        quadMesh.vertices = verticies;
        quadMesh.triangles = triangles;
        quadMesh.uv = uvs;
        changedBuildMode = new UnityEvent<bool>();
        changedBuildMode.Invoke(false);
    }

    #region BuildingHandlers

    public bool HandleBuilding() {
        if (selectedEntity == null) return false;
        Positioned positioned = selectedEntity.GetComponent<Positioned>();
        if (positioned == null) {
            return HandleFreeBuilding();
            
        }
        return HandlePositionedBuilding(positioned);
    }

    private bool HandlePositionedBuilding(Positioned positioned) {
        Address address = World.inst.grid.GetAddressAtWorld(ScreenUtils.WorldMouse());
        Address origin = address - GetPositionedOrigin(positioned);
        List<Cell> cells = new List<Cell>();
        for (int x = 0; x < positioned.size.x; x++) {
            for (int y = 0; y < positioned.size.y; y++) {
                Cell cell = World.inst.grid.GetCellAt(new Address(origin.x + x, origin.y + y));
                cells.Add(cell);
                if (cell.occupied) return false; //Cannot build because not enough space
            }
        }
        Buildable buildable = selectedEntity.GetComponent<Buildable>();
        TakeItems(buildable);

        for (int i = 0; i < cells.Count; i++) {
            cells[i].SetOccupation(true);
        }
        World.inst.entityManager.InstantiateEntityAt(address, selectedEntity, false);
        return true;
    }

    private bool HandleFreeBuilding() {
        throw new System.NotImplementedException();
        //return false;
    }

    #endregion

    private void RenderBlueprint() {
        if (selectedEntity == null) return;
        Renderable renderable = selectedEntity.GetComponent<Renderable>();
        if (renderable == null) return;
        blueprintTargetPosition = World.inst.grid.GetAddressAtWorld(ScreenUtils.WorldMouse()).AsVector();
        blueprintPosition = Vector2.Lerp(blueprintPosition, blueprintTargetPosition, Time.deltaTime * blueprintSnapSpeed);

        mpb.SetTexture("_MainTex", renderable.sprite.texture);
        bool occupied = World.inst.grid.GetCellAt(World.inst.grid.GetAddressAtWorld(blueprintPosition)).occupied;
        bool useRed = occupied || !Player.inst.interactor.IsAddressInInteractionRange();
        mpb.SetColor("_Color", useRed ? redBlueprintColor : normalBlueprintColor);
        Vector2 offset = new Vector2(0, renderable.sprite.pivot.y / renderable.sprite.rect.height) * renderable.visualSize + new Vector2(0,0.001f);
        Matrix4x4 matrix = Matrix4x4.TRS(blueprintPosition - selectedEntity.scale - offset, Quaternion.Euler(0, 0, selectedEntity.rotation), renderable.visualSize);
        DrawQueue.inst.Queue(new SingleRenderQueue(matrix,80000, mpb, material));
    }

    private void TakeItems(Buildable buildable) {
        if (buildable == null || buildable.requirements == null) return;
        for (int i = 0; i < buildable.requirements.items.Count; i++) {
            //Check if enough items and grab them
        }
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.B)) {
            isInBuildMode = !isInBuildMode;
            changedBuildMode.Invoke(isInBuildMode);
        }
        if (!isInBuildMode) return;
        if (Input.GetMouseButtonDown(1)) {
            selectedEntity = null;
        }
        RenderBlueprint();
    }

    // ToDo: Move this somewhere else
    private Address GetPositionedOrigin(Positioned positioned) {
        int x = Mathf.FloorToInt(positioned.size.x / 2.0f);
        int y = Mathf.FloorToInt(positioned.size.y / 2.0f);
        return new Address(x, y);
    }
}
