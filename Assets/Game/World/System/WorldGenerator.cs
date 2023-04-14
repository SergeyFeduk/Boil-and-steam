using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator
{
    private readonly List<Address> addresses = new List<Address>();
    private readonly List<Address> deleteSubjects = new List<Address>();

    public void Update() {
        LoadChunksInBox();
    }

    private void LoadChunksInBox() {
        addresses.Clear();
        Vector2 pos = Player.inst.transform.position;
        Vector2 size = GlobalSettings.inst.main.loadedChunksRect;
        int chunkSize = GlobalSettings.inst.main.chunkSize;

        int xb = Mathf.FloorToInt(pos.x - size.x + chunkSize / 2);
        int xt = Mathf.FloorToInt(pos.x + size.x + chunkSize / 2);
        int yb = Mathf.FloorToInt(pos.y - size.y + chunkSize / 2);
        int yt = Mathf.FloorToInt(pos.y + size.y + chunkSize / 2);

        for (int x = xb; x < xt; x += chunkSize) {
            for (int y = yb; y < yt; y += chunkSize) {
                addresses.Add(World.inst.grid.GetChunkAddress(new Address(x, y)));
            }
        }

        UnloadChunksExcept(addresses, World.inst.grid.GetChunkAddress(new Address(xb,yb)), World.inst.grid.GetChunkAddress(new Address(xt,yt)));
        
    }

    private void UnloadChunksExcept(List<Address> addresses, Address bottomAddress, Address topAddress) {
        int changes = 0;
        //Collect irrelevant chunks
        deleteSubjects.Clear();
        foreach (KeyValuePair<Address, Chunk> entry in World.inst.grid.grid) {
            if (entry.Key.x < bottomAddress.x || entry.Key.x > topAddress.x ||
                entry.Key.y < bottomAddress.y || entry.Key.y > topAddress.y) {
                deleteSubjects.Add(entry.Key);
            }
        }
        changes += deleteSubjects.Count;
        //Unload irrelevant chunks
        for (int i = 0; i < deleteSubjects.Count; i++) {
            World.inst.grid.UnloadChunk(deleteSubjects[i]);
        }
        for (int i = 0; i < addresses.Count; i++) {
            if (!World.inst.grid.grid.ContainsKey(addresses[i])) {
                World.inst.grid.GetOrGenerateChunk(addresses[i]);
                changes++;
            }
        }
        if (changes == 0) return;
        World.inst.grid.UpdateTilemap();
    }

    public void DrawGizmos() {
        for (int i = 0; i < addresses.Count; i++) {
            Gizmos.DrawCube(addresses[i].AsVector(), Vector3.one);
        }
    }
}
