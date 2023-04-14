using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GlobalSettings", menuName = "Settings/GlobalSettings", order = 0)]
public class GlobalSettingsSO : ScriptableObject {
    [field: SerializeField] public Vector2 loadedChunksRect { get; private set; }
    [field: SerializeField] public int chunkSize { get; private set; }
    [field: SerializeField] public float cellSize { get; private set; }

    public void OnValidate() {
        if (chunkSize % 2 == 0) {
            Debug.LogWarning("Chunk size can't be even");
            chunkSize++;
        }
    }
}
