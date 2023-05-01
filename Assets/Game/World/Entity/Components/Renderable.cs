using UnityEngine;
[System.Serializable]
public class Renderable : Component {
    public bool ySorted { get; set; }
    public Sprite sprite { get; set; }
    public Vector2 visualSize { get; set; }
}