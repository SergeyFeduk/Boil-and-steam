public class WorldChunkManager {
    public void GenerateChunk(Chunk chunk) {
        chunk.ForEachCell((Cell cell) => { });
    }
}
