using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class EntityRenderer {
    private List<Renderable> renderables = new List<Renderable>();
    private Mesh quadMesh;
    private Material material;

    private bool sortBeforeRender = false;

    public void SetMaterial(Material material) {
        this.material = material;
    }

    public void AddRenderable(Renderable renderable) {
        renderables.Add(renderable);
        sortBeforeRender = true;
    }

    public void RemoveRenderable(Renderable renderable) {
        renderables.Remove(renderable);
        sortBeforeRender = true;
    }

    public void Render() {
        if (sortBeforeRender) {
            RegenerateSortedRenderables();
            sortBeforeRender = false;
        }
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        for (int i = 0; i < renderables.Count; i++) {
            Renderable renderable = renderables[i];
            Entity entity = renderable.entity;

            mpb.SetTexture("_MainTex", renderable.sprite.texture);
            Vector2 offset = new Vector2(0, renderable.sprite.pivot.y / renderable.sprite.rect.height) * renderable.visualSize;
            Matrix4x4 TRS = Matrix4x4.TRS(entity.position - entity.scale - offset , Quaternion.Euler(0, 0, entity.rotation), renderable.visualSize);

            Graphics.DrawMesh(quadMesh, TRS, material, 0, ScreenUtils.cam, 0, mpb);
        }
    }

    public void Init() {
        quadMesh = MeshUtils.CreateMesh();
        MeshUtils.CreateMeshArrays(1, out Vector3[] verticies, out int[] triangles, out Vector2[] uvs);
        MeshUtils.AddToMeshArrays(verticies, triangles, uvs, 0, Vector3.one, 0, Vector3.one, Vector2.zero, Vector2.one);
        quadMesh.vertices = verticies;
        quadMesh.triangles = triangles;
        quadMesh.uv = uvs;
    }

    private void RegenerateSortedRenderables() {
        renderables = renderables.OrderBy(i => i.entity.position.y).ToList();
        renderables.Reverse();
    }
}
