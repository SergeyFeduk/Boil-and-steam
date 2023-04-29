using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHandAnimation
{
    public abstract void InitAnimation(bool isRight);
    public abstract void FinishAnimation();
    public abstract void InvokeStandingAnimation();
    public abstract void InvokeWalkingAnimation();
    public abstract void PushActiveAnimation(Vector2 pos);
    public abstract void Flip();
    public abstract void ShotDown();
}
