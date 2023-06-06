using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawQueue : MonoBehaviour {
    List<RenderQueue> queue = new List<RenderQueue>();
    public static DrawQueue inst { get; private set; }
    public Mesh quadMesh { get; private set; }

    public void Queue(RenderQueue queueElement) {
        int index = queue.BinarySearch(queueElement, new ValueComparer<RenderQueue>(i => i.renderOrder));
        if (index < 0) index = ~index;
        queue.Insert(index, queueElement);
    }

    public void Render() {
        queue = queue.OrderBy((i) => i.renderOrder).ToList();
        for (int i = queue.Count; i --> 0;) {
            queue[i].Draw();
        }
        queue.Clear();
    }

    private void Init() {
        quadMesh = MeshUtils.CreateMesh();
        MeshUtils.CreateMeshArrays(1, out Vector3[] verticies, out int[] triangles, out Vector2[] uvs);
        MeshUtils.AddToMeshArrays(verticies, triangles, uvs, 0, Vector3.one, 0, Vector3.one, Vector2.zero, Vector2.one);
        quadMesh.vertices = verticies;
        quadMesh.triangles = triangles;
        quadMesh.uv = uvs;
    }

    private void LateUpdate() {
        Render();
    }

    private void Awake() {

        if (inst != null && inst != this) {
            Destroy(this);
        } else {
            inst = this;
            Init();
        }
    }
}

public class RenderQueue {
    public int renderOrder { get; set; }
    public virtual void Draw() { }
}

public class SingleRenderQueue : RenderQueue {
    public Matrix4x4 matrix { get; set; }
    public MaterialPropertyBlock materialPropertyBlock { get; set; }
    public Material material { get; set; }
    public SingleRenderQueue(Matrix4x4 matrix, int renderOrder, MaterialPropertyBlock materialPropertyBlock, Material material) {
        this.matrix = matrix;
        this.renderOrder = renderOrder;
        this.materialPropertyBlock = materialPropertyBlock;
        this.material = material;
    }
    public override void Draw() {
        Graphics.DrawMesh(DrawQueue.inst.quadMesh, matrix, material, 0, ScreenUtils.cam, 0, materialPropertyBlock);
    }
}