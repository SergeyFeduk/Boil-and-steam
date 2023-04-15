using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public enum TileSprite {
    Grass,
    Sand,
    DryGrass,
    Dirt
}

[System.Serializable]
public struct TileGraphic {
    public TileSprite name;
    public Vector2Int uv0Pos;
    public Vector2Int uv1Pos;
}

[RequireComponent(typeof(MeshFilter))]
public class WorldTilemapRenderer : MonoBehaviour {
    private Vector2 chunkOffset = new Vector2(0.5f, 0.5f);
    [SerializeField] private List<TileGraphic> tileGraphic = new List<TileGraphic>();
    [SerializeField] private Vector2Int tilemapSize;
    [SerializeField] private bool runAsync;
    private Mesh mesh;
    private Mesh swapMesh;
    int asyncMeshId = 0;
    private MeshFilter meshFilter => GetComponent<MeshFilter>();

    public async void UpdateMesh(List<Chunk> chunks) {
        if (runAsync) {
            asyncMeshId = Mathf.Abs(asyncMeshId - 1);
            Mesh workingMesh = asyncMeshId == 0 ? mesh : swapMesh;
            workingMesh.Clear();
            (Vector3[] vertices, int[] triangles, Vector2[] uvs) = await Task.Run(() => AsyncMeshUpdate(chunks));
            SetMeshArrays(workingMesh, vertices, triangles, uvs);
            meshFilter.mesh = workingMesh;
            return;
        }
        NormalMeshUpdate(chunks);
    }

    #region MeshUpdate
    private void NormalMeshUpdate(List<Chunk> chunks) {
        mesh.Clear();
        MeshUtils.CreateMeshArrays(chunks.Count * GlobalSettings.inst.main.chunkSize * GlobalSettings.inst.main.chunkSize,
            out Vector3[] vertices, out int[] triangles, out Vector2[] uvs);
        int index = 0;
        for (int i = 0; i < chunks.Count; i++) {
            chunks[i].ForEachCell((cell) => {
                AddCellToMesh(cell, chunks[i], ref index, ref vertices, ref triangles, ref uvs);
            });
        }
        SetMeshArrays(mesh, vertices, triangles, uvs);
    }

    private async Task<(Vector3[] vertices, int[] triangles, Vector2[] uvs)> AsyncMeshUpdate(List<Chunk> chunks) {
        MeshUtils.CreateMeshArrays(chunks.Count * GlobalSettings.inst.main.chunkSize * GlobalSettings.inst.main.chunkSize,
            out Vector3[] vertices, out int[] triangles, out Vector2[] uvs);
        int index = 0;
        for (int i = 0; i < chunks.Count; i++) {
            chunks[i].ForEachCell((cell) => {
                AddCellToMesh(cell, chunks[i], ref index, ref vertices, ref triangles, ref uvs);
            });
            await Task.Yield();
        }
        return (vertices, triangles, uvs);
    }

    private void AddCellToMesh(Cell cell, Chunk chunk, ref int index, ref Vector3[] vertices, ref int[] triangles, ref Vector2[] uvs) {
        Vector2 cellSize = Vector2.one * GlobalSettings.inst.main.cellSize;
        Vector2 cellPosition = cell.address.AsVector() + cellSize * 0.5f + GlobalSettings.inst.main.cellSize * GlobalSettings.inst.main.chunkSize
        * (chunk.GetAddress().AsVector() - chunkOffset);
        TileGraphic graphic = tileGraphic.Find(i => i.name == cell.tile);
        MeshUtils.AddToMeshArrays(vertices, triangles, uvs, index, cellPosition, 0, cellSize,
            (Vector2)graphic.uv0Pos / tilemapSize, (Vector2)graphic.uv1Pos / tilemapSize);
        index++;
    }

    private void SetMeshArrays(Mesh mesh, Vector3[] vertices, int[] triangles, Vector2[] uvs) {
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
    }
    #endregion

    private void Awake() {
        mesh = MeshUtils.CreateMesh(); swapMesh = MeshUtils.CreateMesh();
        meshFilter.mesh = mesh;
    }
}
