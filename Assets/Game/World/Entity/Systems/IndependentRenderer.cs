using System.Linq;
using System.Collections.Generic;
using UnityEngine;
/*
public class IndependentRenderer
{
    private List<CIndependentRenderable> renderables = new List<CIndependentRenderable>();
    private Mesh quadMesh;
    private Material material;

    private bool sortBeforeRender = false;

    public void SetMaterial(Material material)
    {
        this.material = material;
    }

    public void AddRenderable(Entity entity)
    {
        CIndependentRenderable renderable = entity.GetComponent<CIndependentRenderable>();
        renderables.Add(renderable);
        renderable.RecalculateVisualSize();
        sortBeforeRender = true;
    }

    public void RemoveRenderable(CIndependentRenderable renderable)
    {
        renderables.Remove(renderable);
        sortBeforeRender = true;
    }

    public void Render()
    {
        if (sortBeforeRender)
        {
            RegenerateSortedRenderables();
            sortBeforeRender = false;
        }
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        for (int i = 0; i < renderables.Count; i++)
        {
            CIndependentRenderable renderable = renderables[i];
            Entity entity = renderable.entity;

            
        }
    }
    public void Init()
    {
        quadMesh = MeshUtils.CreateMesh();
        MeshUtils.CreateMeshArrays(1, out Vector3[] verticies, out int[] triangles, out Vector2[] uvs);
        MeshUtils.AddToMeshArrays(verticies, triangles, uvs, 0, Vector3.one, 0, Vector3.one, Vector2.zero, Vector2.one);
        quadMesh.vertices = verticies;
        quadMesh.triangles = triangles;
        quadMesh.uv = uvs;
    }

    private void RegenerateSortedRenderables()
    {
        renderables = renderables.OrderBy(i => i.entity.position.y).ToList();
        renderables.Reverse();
    }
}
*/