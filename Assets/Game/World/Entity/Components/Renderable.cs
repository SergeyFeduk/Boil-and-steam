using UnityEngine;
[System.Serializable]
public class Renderable : Component {
    public bool ySorted { get; set; }
    public int renderOrder { get; set; }
    public Sprite sprite { get; set; }
    public Vector2 visualSize { get; set; }

    public void RecalculateVisualSize() {
        visualSize = new Vector2(1, sprite.rect.size.y / sprite.rect.size.x);
    }
    public virtual void Draw(Material material) 
    {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        mpb.SetTexture("_MainTex", sprite.texture);
        Vector2 offset = new Vector2(0, sprite.pivot.y / sprite.rect.height) * visualSize;
        Matrix4x4 TRS = Matrix4x4.TRS(entity.position - entity.scale - offset, Quaternion.Euler(0, 0, entity.rotation), visualSize);
        DrawQueue.inst.Queue(new SingleRenderQueue(TRS, ySorted ? (int)entity.position.y : renderOrder, mpb, material));
    }
}