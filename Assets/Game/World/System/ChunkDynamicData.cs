using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkDynamicData 
{
    private List<int> items = new List<int>();
    private List<Entity> entities = new List<Entity>();

    public void AddItem(int item) {
        throw new NotImplementedException();
    }

    public void AddEntity(Entity entity) {
        entities.Add(entity);
    }

    public void RemoveItem(int item) {
        throw new NotImplementedException();
    }

    public bool RemoveEntity(Entity entity) {
        return entities.Remove(entity);
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
    public void ForEachItem(Action<int> action) {
        for (int i = 0; i < items.Count; i++) {
            action.Invoke(items[i]);
        }
    }
}
