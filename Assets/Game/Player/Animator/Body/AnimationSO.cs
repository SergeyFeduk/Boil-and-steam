using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationData", menuName = "ScriptableObjects/Animation")]
public class AnimationSO : ScriptableObject
{
    public int FPS;
    public List<Sprite> Frames;
}
