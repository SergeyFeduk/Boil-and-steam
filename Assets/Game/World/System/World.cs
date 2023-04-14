using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public static World inst { get; private set; }
    public WordGrid grid { get; private set; }
    public EntityManager entityManager { get; private set; }
    public WorldGenerator worldGenerator { get; private set; }
    [field: SerializeField] public WorldTilemapRenderer worldTilemapRenderer { get; private set; }

    public void Save() {
        throw new System.NotImplementedException();
    }

    public void Load() {
        throw new System.NotImplementedException();
    }

    private void Init() {
        grid = new WordGrid();
        grid.Init(worldTilemapRenderer);
        entityManager = new EntityManager();
        worldGenerator = new WorldGenerator();
        grid.GetOrGenerateChunk(0, 0);
        grid.GetOrGenerateChunk(-1, -1);
    }

    private void Awake() {

        if (inst != null && inst != this) {
            Destroy(this);
        } else {
            inst = this;
        }
    }

    private void Start() {
        Init();
    }

    private void Update() {
        worldGenerator.Update();
    }

    private void OnDrawGizmosSelected() {
        if (grid == null) return;
        grid.DrawGizmos();
        //worldGenerator.DrawGizmos();
    }
}
