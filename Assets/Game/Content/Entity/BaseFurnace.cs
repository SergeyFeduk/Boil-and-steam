using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class BaseFurnace : Entity {
    // Start is called before the first frame update
    public BaseFurnace() {
        components = new Dictionary<System.Type, Component>();
        AddComponent(typeof(Buildable));
        AddComponent(typeof(Renderable));
        AddComponent(typeof(Positioned));
        AddComponent(typeof(Interactable));
        AddComponent(typeof(Unloadable));
    }

    public override void Init() {
        base.Init();
    }
}
