using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "BareHandAnimation", menuName = "ScriptableObjects/HandsAnimation/BareHands")]
public class BareHandAnimation : ScriptableObject, IHandAnimation
{
    public GameObject hand;
    public float amplitudeX, amplitudeY, deltaX, deltaY;
    [Space]
    public float frequency;
    public float handVelocity;
    public float maxScale;
    public float scaleVelocity;

    private GameObject firstFocus,secondFocus, firstHand, secondHand;

    private Timer timer = new Timer();

    public bool stopFlag = true;
    private bool isStanding = true;
    public void InitAnimation(bool isRight)
    {
        if (isRight == (deltaX < 0)) deltaX *= -1;
        stopFlag = false;

        firstHand = Instantiate(hand);
        secondHand = Instantiate(hand);
        firstFocus = new GameObject();
        secondFocus = new GameObject();

        firstHand.transform.parent = Player.inst.animator.handsAnimator.transform;
        secondHand.transform.parent = Player.inst.animator.handsAnimator.transform;
        firstFocus.transform.parent = Player.inst.animator.handsAnimator.transform;
        secondFocus.transform.parent = Player.inst.animator.handsAnimator.transform;

        firstFocus.transform.localPosition = new Vector3(-amplitudeX + deltaX, amplitudeY + deltaY);
        secondFocus.transform.localPosition = new Vector3(amplitudeX + deltaX, amplitudeY + deltaY);
        firstHand.transform.localPosition = new Vector3(-amplitudeX + deltaX, amplitudeY + deltaY);
        secondHand.transform.localPosition = new Vector3(amplitudeX + deltaX, amplitudeY + deltaY);

        firstHand.AddComponent<HandMover>().Run(firstFocus,handVelocity,scaleVelocity);
        secondHand.AddComponent<HandMover>().Run(secondFocus, handVelocity, scaleVelocity);
    }
    public void InvokeStandingAnimation()
    {
        isStanding = true;
        MoveFocusesVerticaly();
    }
    public void InvokeWalkingAnimation()
    {
        isStanding = false;
        MoveFocusesHorizontaly();
    }
    public void FinishAnimation()
    {
        Destroy(firstHand);
        Destroy(secondHand);
        Destroy(firstFocus);
        Destroy(secondFocus);
        stopFlag = true;
    }
    public void Flip()
    {
        deltaX *= -1;
        firstFocus.transform.Translate(2 * deltaX,0,0);
        secondFocus.transform.Translate(2 * deltaX, 0, 0);
    }
    public void PushActiveAnimation(Vector2 pos)
    {
        firstHand.GetComponent<HandMover>().startFlag = false;
        firstHand.transform.localPosition = firstHand.transform.parent.InverseTransformPoint(pos);
        firstHand.transform.localScale = Vector3.one*maxScale;
        firstHand.GetComponent<HandMover>().startFlag = true;
    }
    private async void MoveFocusesVerticaly()
    {
        if (firstFocus == null || secondFocus == null) return;
        firstFocus.transform.localPosition = new Vector3(-amplitudeX + deltaX, amplitudeY + deltaY);
        secondFocus.transform.localPosition = new Vector3(amplitudeX + deltaX, amplitudeY + deltaY);
        timer.SetFrequency(frequency);
        while (!timer.Execute())
        {
            if (stopFlag || !isStanding) return;
            await Task.Yield();
        }
        if (firstFocus == null || secondFocus == null) return;
        firstFocus.transform.localPosition = new Vector3(-amplitudeX + deltaX, -amplitudeY + deltaY);
        secondFocus.transform.localPosition = new Vector3(amplitudeX + deltaX, -amplitudeY + deltaY);
        timer.SetFrequency(frequency);
        while (!timer.Execute())
        {
            if (stopFlag || !isStanding) return;
            await Task.Yield();
        }
        MoveFocusesVerticaly();
    }
    private async void MoveFocusesHorizontaly()
    {
        if (firstFocus == null || secondFocus == null) return;
        firstFocus.transform.localPosition = new Vector3(-amplitudeX + deltaX, deltaY);
        secondFocus.transform.localPosition = new Vector3(amplitudeX + deltaX, deltaY);
        timer.SetFrequency(frequency);
        while (!timer.Execute())
        {
            if (stopFlag || isStanding) return;
            await Task.Yield();
        }
        if (firstFocus == null || secondFocus == null) return;
        firstFocus.transform.localPosition = new Vector3(-amplitudeX + deltaX, deltaY);
        secondFocus.transform.localPosition = new Vector3(amplitudeX + deltaX, deltaY);
        timer.SetFrequency(frequency);
        while (!timer.Execute())
        {
            if (stopFlag || isStanding) return;
            await Task.Yield();
        }
        MoveFocusesHorizontaly();
    }
    public void ShotDown() => stopFlag = true;
}
public class HandMover : MonoBehaviour
{
    private float velocity;
    private float scaleVelocity;
    private GameObject focus;
    public bool startFlag = false;
    public void Run(GameObject obj, float value, float value2)
    {
        focus = obj;
        velocity = value;
        scaleVelocity = value2;
        startFlag = true;
    }
    private void FixedUpdate()
    {
        if(startFlag)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, focus.transform.localPosition, velocity);
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, scaleVelocity);
        }
    }
}
