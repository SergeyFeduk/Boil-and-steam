using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Wall : Entity{

    public Wall() {
        components = new Dictionary<System.Type, Component>();
        AddComponent(typeof(Buildable));
        AddComponent(typeof(Renderable));
        AddComponent(typeof(Positioned));
    }

    public override void Init() {
        base.Init();
    }
}
