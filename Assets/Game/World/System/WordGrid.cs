using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class WordGrid 
{
    public Dictionary<Address, Chunk> grid = new Dictionary<Address, Chunk>();
    public Dictionary<Address, Chunk> unloadedGrid = new Dictionary<Address, Chunk>();

    public WorldChunkManager chunkManager = new WorldChunkManager();

    public WorldTilemapRenderer tilemapRenderer;

    public void Init(WorldTilemapRenderer worldTilemapRenderer) {
        tilemapRenderer = worldTilemapRenderer;
    }

    public Chunk GetOrGenerateChunk(Address address) {
        Chunk chunk;
        if (!grid.TryGetValue(address, out chunk)) {
            if (unloadedGrid.TryGetValue(address, out chunk)) {
                chunk = LoadChunk(address);
            } else {
                chunk = GenerateChunk(address);
            }
        }
        return chunk;
    }

    public Chunk GetOrGenerateChunk(int x, int y) {
        return GetOrGenerateChunk(new Address(x,y));
    }

    public Chunk GetOrGenerateChunkAtCell(Address address) {
        return GetOrGenerateChunk(GetChunkAddress(address));
    }

    public Address GetAddressAtWorld(Vector2 worldPosition) {
        return new Address(Mathf.FloorToInt((worldPosition.x + 0.5f) / GlobalSettings.inst.main.cellSize), 
                           Mathf.FloorToInt((worldPosition.y + 0.5f) / GlobalSettings.inst.main.cellSize));
    }

    public Address GetChunkAddress(Address cellAddress) {
        return new Address(Mathf.RoundToInt(cellAddress.x / (float)GlobalSettings.inst.main.chunkSize), Mathf.RoundToInt(cellAddress.y / (float)GlobalSettings.inst.main.chunkSize));
    }

    public void PlaceEntityAt(Address address, Entity entity) {
        World.inst.entityManager.InstantiateEntityAt(address, entity);
    }

    public Cell GetCellAt(Address address) {
        Chunk chunk = GetOrGenerateChunkAtCell(address);
        return chunk.GetCellAtGlobal(address);
    }

    public void UnloadChunk(Address address) {
        Chunk chunk = GetChunk(address);
        chunk.Unload();
        unloadedGrid.Add(address, chunk);
        grid.Remove(address);
    }
    private Chunk LoadChunk(Address address) {
        Chunk chunk = unloadedGrid[address];
        unloadedGrid.Remove(address);
        grid.Add(address, chunk);
        chunk.Load();
        return chunk;
    }

    public void DrawGizmos() {
        foreach (KeyValuePair<Address, Chunk> entry in grid) {
            entry.Value.ForEachCell((Cell cell) => {
                Gizmos.color = cell.occupied == true ? Color.red : Color.white;
                Vector2 position = (cell.address.AsVector() + new Vector2(0.5f, 0.5f)) * new Vector2(1, -1) * (int)GlobalSettings.inst.main.cellSize + 
                                   (entry.Value.GetAddress().AsVector() - new Vector2(0.5f, -0.5f)) * GlobalSettings.inst.main.chunkSize;
                Gizmos.DrawCube(position, Vector3.one * 0.5f);
            });
        }
    }

    public void UpdateTilemap() {
        tilemapRenderer.UpdateMesh(grid.Values.ToList());
    }

    private Chunk GetChunk(Address address) {
        grid.TryGetValue(address, out Chunk chunk);
        return chunk;
    }

    private Chunk GenerateChunk(Address address) {
        Chunk chunk = new Chunk();
        chunk.Init(address);
        chunkManager.GenerateChunk(chunk);
        grid.Add(address, chunk);
        return chunk;
    }

}
