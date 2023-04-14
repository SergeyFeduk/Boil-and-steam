using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildRequirement", menuName = "Resources/BuildRequirement")]
public class BuildRequirementsSO : ScriptableObject
{
    //TODO insert items here
    [field: SerializeField] public List<int> items { get; private set; }
}
