using System;
using System.Collections.Generic;
using UnityEngine;

public class Entity : ICloneable {
    public Vector2 position { get; set; }
    public float rotation { get; set; }
    public Vector2 scale { get; set; }

    public Dictionary<Type, Component> components { get; set; }

    public T GetComponent<T>() where T : Component {
        if (!components.ContainsKey(typeof(T))) return null;
        return (T)components[typeof(T)];
    }

    public void AddComponent(Type type) {
        if (components.ContainsKey(type)) return;
        Component component = (Component)Activator.CreateInstance(type);
        component.entity = this;
        components.Add(type, component);
    }

    public void AddComponent(Component component) {
        if (components.ContainsValue(component)) return;
        component.entity = this;
        components.Add(component.GetType(), component);
    }

    public void RemoveComponent(Type type) {
        components.Remove(type);
    }

    public virtual void Init() {
        position = Vector2.zero;
        rotation = 0;
        scale = Vector2.one;
    }

    public virtual void Decompose() {
        World.inst.entityManager.DecomposeEntity(this);
    }

    public virtual object Clone() {
        Entity cloned = (Entity)MemberwiseClone();
        cloned.components = new Dictionary<Type, Component>();
        foreach (KeyValuePair<Type, Component> kvp in components) {
            Component newComponent = (Component)kvp.Value.Clone();
            cloned.AddComponent(newComponent);
        }
        return cloned;
    }
}
