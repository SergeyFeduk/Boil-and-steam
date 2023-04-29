using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NoneHand", menuName = "ScriptableObjects/HandsAnimation/NoneHands")]
public class NoneHandAnimation : ScriptableObject, IHandAnimation
{
    public void InitAnimation(bool isRight) { }
    public void FinishAnimation() { }
    public void InvokeStandingAnimation() { }
    public void InvokeWalkingAnimation() { }
    public void PushActiveAnimation(Vector2 pos) { }
    public void Flip() { }
    public void ShotDown() { }
}
