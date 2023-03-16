using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CreateBuildRequirement", menuName = "ScriptableObjects")]
public class BuildRequirementsSO : ScriptableObject
{
    //[field: SerializeField] public List<ItemSO> items { get; private set; }
    [field: SerializeField] public List<int> counts { get; private set; } //Cause prototype
}
