using System.Collections.Generic;
using UnityEngine;

public interface IPositioned {
    public Address origin { get; set; }
    public List<Address> occupiedPositions { get; set; }
    public Vector2Int size { get; set; }
}