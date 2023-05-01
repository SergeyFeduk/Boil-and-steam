using System.Collections.Generic;
using UnityEngine;

public class EntityLoader
{
    public void LoadChunk(Chunk chunk) {
        ChunkDynamicData cdd = chunk.GetCDD();
        List<Entity> toAdd = new List<Entity>();
        List<Entity> toRemove = new List<Entity>();

        cdd.ForEachEntity((entity) => {
            Entity newEntity = World.inst.entityManager.InstantiateEntity(entity, true);
            Unloadable unloadable = newEntity.GetComponent<Unloadable>();
            unloadable?.Loaded(Time.time);
            toRemove.Add(entity);
            toAdd.Add(newEntity);
        });
        for (int i = 0; i < toRemove.Count; i++) {
            cdd.RemoveEntity(toRemove[i]);
        }
        for (int i = 0; i < toAdd.Count; i++) {
            cdd.AddEntity(toAdd[i]);
        }
    }

    public void UnloadChunk(Chunk chunk) {
        ChunkDynamicData cdd = chunk.GetCDD();
        cdd.ForEachEntity((Entity entity) => {
            Unloadable unloadable = entity.GetComponent<Unloadable>();
            unloadable?.Unloaded(Time.time);
            entity.Decompose();
        });
    }
}
