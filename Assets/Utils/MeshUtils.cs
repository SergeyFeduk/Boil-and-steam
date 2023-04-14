using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshUtils {

    private static Quaternion[] quaternionCache;
    private static void CacheQuaternion() {
        if (quaternionCache != null) return;
        quaternionCache = new Quaternion[360];
        for (int i = 0; i < 360; i++) {
            quaternionCache[i] = Quaternion.Euler(0, 0, i);
        }
    }

    private static Quaternion GetQuaternion(float rot) {
        int rotInt = Mathf.RoundToInt(rot);
        rotInt %= 360;
        if (rotInt < 0) rotInt += 360;
        if (quaternionCache == null) CacheQuaternion();
        return quaternionCache[rotInt];
    } 

    public static Mesh CreateMesh() {
        Mesh mesh = new Mesh();
        mesh.vertices = new Vector3[0];
        mesh.triangles = new int[0];
        mesh.uv = new Vector2[0];
        return mesh;
    }

    public static void CreateMeshArrays(int quadCount, out Vector3[] vertices, out int[] trainagles, out Vector2[] uvs) {
        vertices = new Vector3[4 * quadCount];
        trainagles = new int[6 * quadCount];
        uvs = new Vector2[4 * quadCount];
    }

    public static void AddToMeshArrays(Vector3[] verticies, int[] trainagles, Vector2[] uvs, int index, Vector3 position, float rot, Vector3 size, Vector2 uv0, Vector2 uv1) {
        int vInd = index * 4;
        int vInd0 = vInd;
        int vInd1 = vInd + 1;
        int vInd2 = vInd + 2;
        int vInd3 = vInd + 3;
        size *= 0.5f;
        if (size.x != size.y) {
            verticies[vInd0] = position + GetQuaternion(rot) * new Vector3(-size.x,  size.y);
            verticies[vInd1] = position + GetQuaternion(rot) * new Vector3(-size.x, -size.y);
            verticies[vInd2] = position + GetQuaternion(rot) * new Vector3( size.x, -size.y);
            verticies[vInd3] = position + GetQuaternion(rot) * size;
        } else {
            verticies[vInd0] = position + GetQuaternion(rot - 270) * size;
            verticies[vInd1] = position + GetQuaternion(rot - 180) * size;
            verticies[vInd2] = position + GetQuaternion(rot -  90) * size;
            verticies[vInd3] = position + GetQuaternion(rot -   0) * size;
        }

        int tInd = index * 6;
        trainagles[tInd + 0] = vInd0;
        trainagles[tInd + 1] = vInd3;
        trainagles[tInd + 2] = vInd1;

        trainagles[tInd + 3] = vInd1;
        trainagles[tInd + 4] = vInd3;
        trainagles[tInd + 5] = vInd2;

        uvs[vInd] = new Vector2(uv0.x, uv1.y);
        uvs[vInd + 1] = new Vector2(uv0.x, uv0.y);
        uvs[vInd + 2] = new Vector2(uv1.x, uv0.y);
        uvs[vInd + 3] = new Vector2(uv1.x, uv1.y);
    }
}
