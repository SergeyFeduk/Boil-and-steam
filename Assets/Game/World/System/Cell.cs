public class Cell 
{
    public Address address { get; private set; }
    public Positioned positioned { get; private set; }
    public bool occupied { get; private set; }
    public TileSprite tile { get; private set; }

    public Cell(Address address) {
        this.address = address;
        SetTileSprite(TileSprite.Grass);
    }

    public void SetOccupation(bool value) {
        occupied = value;
    }

    public void SetTileSprite(TileSprite sprite) {
        tile = sprite;
    }

}
