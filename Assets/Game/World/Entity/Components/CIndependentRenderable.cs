using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CIndependentRenderable : Renderable
{
    public Vector2 localPosition { get; set; }
    public override void Draw(Material material)
    {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        mpb.SetTexture("_MainTex", sprite.texture);
        Vector2 offset = new Vector2(0, sprite.pivot.y / sprite.rect.height) * visualSize;
        Matrix4x4 TRS = Matrix4x4.TRS(entity.position - entity.scale - offset + localPosition, Quaternion.Euler(0, 0, entity.rotation), visualSize);
        DrawQueue.inst.Queue(new SingleRenderQueue(TRS, ySorted ? (int)entity.position.y : renderOrder, mpb, material));
    }
}
