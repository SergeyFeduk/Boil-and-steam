using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell 
{
    //Local address
    public Address address { get; private set; }
    public IPositioned entity { get; private set; }
    public bool occupied { get; private set; }
    public TileSprite tile { get; private set; }

    public Cell(Address address) {
        this.address = address;
        SetTileSprite(TileSprite.Grass);
    }
    public Cell(int x, int y) {
        address = new Address(x, y);
    }

    public void SetOccupation(bool value) {
        occupied = value;
    }

    public void SetEntity(IPositioned entity) {
        this.entity = entity;
    }

    public void SetTileSprite(TileSprite sprite) {
        tile = sprite;
    }

}
