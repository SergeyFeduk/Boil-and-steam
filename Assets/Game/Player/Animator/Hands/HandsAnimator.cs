using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class HandsAnimator : MonoBehaviour
{
    [SerializeField] private ScriptableObject[] animations;
    public enum States
    {
        None,
        BareHands,
    }

    private IHandAnimation currentAnimation;
    private bool isRight = true;
    private void Awake() => currentAnimation = (IHandAnimation)animations[0];
    public void ChangeState(States toStage)
    {
        currentAnimation.FinishAnimation();
        currentAnimation = (IHandAnimation)animations[(int)toStage];
        currentAnimation.InitAnimation(isRight);
    }
    public void InvokeStanding() => currentAnimation.InvokeStandingAnimation();
    public void InvokeWalking() => currentAnimation.InvokeWalkingAnimation();
    public void InvokeActive(Vector2 pos) => currentAnimation.PushActiveAnimation(pos);
    public void Flip()
    {
        isRight = !isRight;
        currentAnimation.Flip();
    }
    private void OnApplicationQuit() => currentAnimation.ShotDown();
}
