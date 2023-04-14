using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : Entity, IBuildable, IPositioned {
    [field: SerializeField] public BuildRequirementsSO requirements { get; set; }
    public Address origin { get; set; }
    public List<Address> occupiedPositions { get; set; }
    [field: SerializeField] public Vector2Int size { get; set; }
    public override void Init() {
        
    }
}
