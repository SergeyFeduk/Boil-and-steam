using UnityEngine;

public class World : MonoBehaviour
{
    public static World inst { get; private set; }
    public WordGrid grid { get; private set; }
    public EntityManager entityManager { get; private set; }
    public WorldGenerator worldGenerator { get; private set; }
    [field: SerializeField] public WorldTilemapRenderer worldTilemapRenderer { get; private set; }
    [SerializeField] private Material entityMaterial;

    private void Init()
    {
        grid = new WordGrid();
        grid.Init(worldTilemapRenderer);
        entityManager = new EntityManager();
        entityManager.Init();
        entityManager.SetRendererMaterial(entityMaterial);
        worldGenerator = new WorldGenerator();
    }

    private void Update() {
        worldGenerator.Update();
    }
    private void LateUpdate() {
        entityManager.Update();
    }

    private void Start() {
        Init();
    }

    private void Awake() {

        if (inst != null && inst != this) {
            Destroy(this);
        } else {
            inst = this;
        }
    }

    private void OnDrawGizmosSelected() {
        if (grid == null) return;
        grid.DrawGizmos();
    }
}
