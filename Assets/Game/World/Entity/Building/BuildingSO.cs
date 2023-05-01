using UnityEngine;
[CreateAssetMenu(fileName = "Building", menuName = "Resources/Building")]
public class BuildingSO : ScriptableObject
{
    [field: SerializeField] public string title { get; private set; }
    [field: SerializeField, Multiline] public string description { get; private set; }
    [field: SerializeField] public BuildRequirementsSO requirements { get; private set; }
}
