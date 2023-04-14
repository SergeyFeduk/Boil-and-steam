using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldChunkManager 
{
    public void LoadChunk(int x, int y) {
        LoadChunk(new Address(x,y));
    }

    public void LoadChunk(Address address) {
        throw new System.NotImplementedException();
    }

    public void UnloadChunk(int x, int y) {
        UnloadChunk(new Address(x, y));
    }

    public void UnloadChunk(Address address) {
        throw new System.NotImplementedException();
    }

    public void GenerateChunk(Chunk chunk) {
        chunk.ForEachCell((Cell cell) => { });
        //throw new System.NotImplementedException();
    }
}
