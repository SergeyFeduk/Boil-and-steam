using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItem : Entity
{
    public WorldItem()
    {
        components = new Dictionary<System.Type, Component>();
        AddComponent(typeof(Renderable));
        AddComponent(typeof(CWorldItem));
    }
    public override void Init()
    {
        base.Init();
    }
}
