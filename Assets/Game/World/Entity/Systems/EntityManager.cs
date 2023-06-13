using System.Collections.Generic;
using UnityEngine;

public class EntityManager {
    private readonly List<Entity> entities = new List<Entity>();
    public EntityRenderer renderer { get; private set; }
    public EntityLoader loader { get; private set; }
    public WorldItemManager worldItemManager { get; private set; }
    /*
    private Dictionary <Type,Action<Entity>> componentToSystemDictionary;
    private void Awake()
    {
        componentToSystemDictionary.Add(typeof(Renderable), renderer.AddRenderable);
    }
     */
    public void Init()
    {
        renderer = new EntityRenderer();
        renderer.Init();
        loader = new EntityLoader();
        worldItemManager = new WorldItemManager();
    }
    public void Update() {
        renderer.Render();
        worldItemManager.Update();
    }

    public Entity InstantiateEntityAt(Address address, Entity entityPrefab, bool clone) {
        Entity entity = InstantiateEntity(entityPrefab, clone);

        entity.position = address.AsVector();
        Positioned positioned = entity.GetComponent<Positioned>();
        if (positioned != null) {
            positioned.origin = address;
        }
        
        Chunk chunk = World.inst.grid.GetOrGenerateChunkAtCell(address);
        chunk.GetCDD().AddEntity(entity);
        return entity;
    }

    public Entity InstantiateEntity(Entity entityPrefab, bool clone) {
        Entity entity = (Entity)entityPrefab.Clone();
        if (!clone) { entity.Init(); }
        PassEntity(entity);
        return entity;
    }

    public void DecomposeEntity(Entity entity) {
        entities.Remove(entity);
        Renderable renderable = entity.GetComponent<Renderable>();
        CIndependentRenderable iRenderable = entity.GetComponent<CIndependentRenderable>();
        if (renderable != null || iRenderable != null) renderer.RemoveRenderable(entity);
        CWorldItem worldItem = entity.GetComponent<CWorldItem>();
        if (worldItem != null) worldItemManager.RemoveWorldItem(entity);
    }

    public void SetRendererMaterial(Material material) {
        renderer.SetMaterial(material);
    }

    private void PassEntity(Entity entity) {
        entities.Add(entity);
        Renderable renderable = entity.GetComponent<Renderable>();
        CIndependentRenderable iRenderable = entity.GetComponent<CIndependentRenderable>();
        if (renderable != null || iRenderable != null) renderer.AddRenderable(entity);
        CWorldItem worldItem = entity.GetComponent<CWorldItem>();
        if (worldItem != null) worldItemManager.AddWorldItem(entity);
    }
}
