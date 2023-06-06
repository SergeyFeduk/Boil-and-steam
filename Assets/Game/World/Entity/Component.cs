using System;
using UnityEngine;
using UnityEditor;

[Serializable]
public class Component : ICloneable {
    public Entity entity { get; set; }
    public virtual object Clone() {
        return MemberwiseClone();
    }
}