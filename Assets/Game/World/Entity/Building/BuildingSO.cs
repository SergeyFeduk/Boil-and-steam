using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Building", menuName = "Resources/Building")]
public class BuildingSO : ScriptableObject
{
    [field: SerializeField] public string title { get; private set; }
    [field: SerializeField, Multiline] public string description { get; private set; }
}
