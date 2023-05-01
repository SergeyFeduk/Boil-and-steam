using UnityEngine;
[System.Serializable]
public class Unloadable : Component {
    public virtual void Loaded(float time) { }
    public virtual void Unloaded(float time) { }
}