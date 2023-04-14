using System;
using UnityEngine;

public abstract class Entity : MonoBehaviour, ICloneable {
    public abstract void Init();
    public virtual void Decompose() {
        World.inst.entityManager.DecomposeEntity(this);
    }
    public object Clone() {
        return this.MemberwiseClone();
    }
}
