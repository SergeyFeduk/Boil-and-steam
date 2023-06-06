using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Buildable : Component
{
    public int number { get; set; }
    public float fnumber { get; set; }
    public double dnumber { get; set; }
    public string text { get; set; } = "";
    public bool boolean { get; set; }
    public BuildRequirementsSO requirements { get; set; }
}
