using System;
using UnityEngine;

public class Chunk {
    private Cell[,] cells;
    private Address address;
    private ChunkDynamicData cdd;

    public void Init(Address address) {
        this.address = address;
        int chunkSize = GlobalSettings.inst.main.chunkSize;
        cdd = new ChunkDynamicData();

        cells = new Cell[chunkSize, chunkSize];
        for (int x = 0; x < chunkSize; x++) {
            for (int y = 0; y < chunkSize; y++) {
                Address cellAddress = new Address(x, y);
                cells[x, y] = new Cell(cellAddress);
            }
        }
    }

    public Address GetAddress() {
        return address;
    }

    public Cell GetCell(int x, int y) {
        return cells[x, y];
    }

    public ChunkDynamicData GetCDD() {
        return cdd;
    }

    public Cell GetCellAtGlobal(Address globalAddress) {
        int halfSize = Mathf.FloorToInt(GlobalSettings.inst.main.chunkSize / 2.0f);
        Address sizedAddress = address * GlobalSettings.inst.main.chunkSize;
        Address localRTAddress = new Address(sizedAddress.x - halfSize, sizedAddress.y + halfSize);
        Address localCellAddress = new Address(-localRTAddress.x + globalAddress.x, localRTAddress.y - globalAddress.y);
        return cells[localCellAddress.x, localCellAddress.y];
    }

    public void ForEachCell(Action<int, int> action) {
        for (int x = 0; x < cells.GetLength(0); x++) {
            for (int y = 0; y < cells.GetLength(0); y++) {
                action.Invoke(x, y);
            }
        }
    }

    public void ForEachCell(Action<Cell> action) {
        for (int x = 0; x < cells.GetLength(0); x++) {
            for (int y = 0; y < cells.GetLength(0); y++) {
                action.Invoke(GetCell(x, y));
            }
        }
    }

    public void Unload()
    {
        cdd.Unload();
    }

    public void Load()
    {
        cdd.Load();
    }
}
