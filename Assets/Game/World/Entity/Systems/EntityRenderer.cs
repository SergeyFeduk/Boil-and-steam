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

    public void AddRenderable(Entity entity) {
        Renderable renderable = entity.GetComponent<Renderable>();
        renderables.Add(renderable);
        CIndependentRenderable iRenderable = entity.GetComponent<CIndependentRenderable>();
        if(iRenderable != null)
        {
            renderables.Add(iRenderable);
            iRenderable.RecalculateVisualSize();
        }
        renderable.RecalculateVisualSize();
        sortBeforeRender = true;
    }


    public void RemoveRenderable(Entity entity) {
        Renderable renderable = entity.GetComponent<Renderable>();
        renderables.Remove(renderable);
        sortBeforeRender = true;
        CIndependentRenderable iRenderable = entity.GetComponent<CIndependentRenderable>();
        if (iRenderable != null)
        {
            renderables.Remove(iRenderable);
        }
        sortBeforeRender = true;
    }

    public void Render() {
        if (sortBeforeRender) {
            RegenerateSortedRenderables();
            sortBeforeRender = false;
        }
        
        for (int i = 0; i < renderables.Count; i++) {
            Renderable renderable = renderables[i];
            renderable.Draw(material);
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
