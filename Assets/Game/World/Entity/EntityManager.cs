using System.Collections.Generic;
using UnityEngine;

public class EntityManager 
{
    private readonly List<Entity> entities = new List<Entity>();

    public Entity InstantiateEntityAt(Address address, Entity entityPrefab) {
        Entity entity = InstantiateEntity(entityPrefab);
        entity.transform.position = address.AsVector();
        Chunk chunk = World.inst.grid.GetOrGenerateChunkAtCell(address);
        chunk.GetCDD().AddEntity(entity);
        return entity;
    }

    public Entity InstantiateEntity(Entity entityPrefab) {
        GameObject entityObject = Object.Instantiate(entityPrefab.gameObject, World.inst.transform);
        Entity entity = entityObject.GetComponent<Entity>();
        entity.Init();
        entities.Add(entity);
        return entity;
    }

    public void DecomposeEntity(Entity entity) {
        entities.Remove(entity);
        Object.Destroy(entity.gameObject);
    }
}
