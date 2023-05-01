using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Positioned : Component {
    public Address origin { get; set; }
    public List<Address> occupiedPositions { get; set; }
    public Vector2Int size { get; set; }
}