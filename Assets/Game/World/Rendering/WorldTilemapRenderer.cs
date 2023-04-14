using System;
using System.Threading.Tasks;
using System.Collections;
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
    [SerializeField] private bool async;
    private Mesh mesh;
    private Mesh swapMesh;
    int qid = 0;
    private MeshFilter meshFilter => GetComponent<MeshFilter>();

    private void Awake() {
        mesh = MeshUtils.CreateMesh();
        swapMesh = MeshUtils.CreateMesh();
        meshFilter.mesh = mesh;
    }

    public async void UpdateMesh(List<Chunk> chunks) {
        if (async) {
            qid = Mathf.Abs(qid - 1);
            Mesh workingMesh = qid == 0 ? mesh : swapMesh;
            workingMesh.Clear();
            (Vector3[] verticies, int[] triangles, Vector2[] uvs) targetMesh = await Task.Run(() => Work(workingMesh, chunks));
            workingMesh.vertices = targetMesh.verticies;
            workingMesh.triangles = targetMesh.triangles;
            workingMesh.uv = targetMesh.uvs;
            meshFilter.mesh = workingMesh;
            return;
        }
        NormalUpdate(chunks);
    }

    private void NormalUpdate(List<Chunk> chunks) {
        mesh.Clear();
        MeshUtils.CreateMeshArrays(chunks.Count * GlobalSettings.inst.main.chunkSize * GlobalSettings.inst.main.chunkSize, out Vector3[] verticies, out int[] triangles, out Vector2[] uvs);
        int index = 0;
        for (int i = 0; i < chunks.Count; i++) {
            chunks[i].ForEachCell((cell) => {
                Vector2 cellSize = Vector2.one * GlobalSettings.inst.main.cellSize;
                Vector2 cellPosition = cell.address.AsVector() + cellSize * 0.5f + GlobalSettings.inst.main.cellSize * GlobalSettings.inst.main.chunkSize * (chunks[i].GetAddress().AsVector() - chunkOffset);
                TileGraphic graphic = tileGraphic.Find(i => i.name == cell.tile);
                MeshUtils.AddToMeshArrays(verticies, triangles, uvs, index, cellPosition, 0, cellSize, (Vector2)graphic.uv0Pos / tilemapSize, (Vector2)graphic.uv1Pos / tilemapSize);
                index++;
            });
        }
        mesh.vertices = verticies;
        mesh.triangles = triangles;
        mesh.uv = uvs;
    }

    private async Task<(Vector3[] verticies, int[] triangles, Vector2[] uvs)> Work(Mesh workingMesh, List<Chunk> chunks) {
        MeshUtils.CreateMeshArrays(chunks.Count * GlobalSettings.inst.main.chunkSize * GlobalSettings.inst.main.chunkSize, out Vector3[] verticies, out int[] triangles, out Vector2[] uvs);
        int index = 0;
        for (int i = 0; i < chunks.Count; i++) {
            chunks[i].ForEachCell((cell) => {
                Vector2 cellSize = Vector2.one * GlobalSettings.inst.main.cellSize;
                Vector2 cellPosition = cell.address.AsVector() + cellSize * 0.5f + GlobalSettings.inst.main.cellSize * GlobalSettings.inst.main.chunkSize * (chunks[i].GetAddress().AsVector() - chunkOffset);
                TileGraphic graphic = tileGraphic.Find(i => i.name == cell.tile);
                MeshUtils.AddToMeshArrays(verticies, triangles, uvs, index, cellPosition, 0, cellSize, (Vector2)graphic.uv0Pos / tilemapSize, (Vector2)graphic.uv1Pos / tilemapSize);
                index++;
            });
            await Task.Yield();
        }
        return (verticies,triangles, uvs);
    }
}
