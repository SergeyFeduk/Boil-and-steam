using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildRequirement", menuName = "Resources/BuildRequirement")]
public class BuildRequirementsSO : ScriptableObject
{
    [field: SerializeField] public List<Item> items { get; private set; }
}
