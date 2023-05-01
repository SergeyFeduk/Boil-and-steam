using System;
using System.Collections.Generic;
using UnityEngine;

public class ChunkDynamicData {
    private List<Entity> entities = new List<Entity>();

    public void AddEntity(Entity entity) {
        entities.Add(entity);
    }

    public void RemoveEntity(Entity entity) {
        entities.Remove(entity);
    }

    public void ForEachEntity(Action<Entity> action) {
        for (int i = 0; i < entities.Count; i++) {
            action.Invoke(entities[i]);
        }
    }
}
