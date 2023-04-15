using System;
using System.Collections.Generic;
using UnityEngine;

public class ChunkDynamicData 
{
    private List<Entity> entities = new List<Entity>();

    public void AddEntity(Entity entity)
    {
        entities.Add(entity);
    }

    public void Unload() {
        ForEachEntity((Entity entity) => {
            entity.GetComponent<IUnloadable>()?.Unloaded(Time.time);
            entity.gameObject.SetActive(false);
        });
    }

    public void Load() {
        ForEachEntity((entity) => {
            entity.gameObject.SetActive(true);
            entity.GetComponent<IUnloadable>()?.Loaded(Time.time);
        });
    }

    public void ForEachEntity(Action<Entity> action) {
        for (int i = 0; i < entities.Count; i++) {
            action.Invoke(entities[i]);
        }
    }
}
