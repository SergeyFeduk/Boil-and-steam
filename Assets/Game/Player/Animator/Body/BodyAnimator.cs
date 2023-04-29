using UnityEngine;
using System.Threading.Tasks;
public class BodyAnimator : MonoBehaviour
{
    [SerializeField] private AnimationSO standingAnimation, walkingAnimation;
    private SpriteRenderer spriteRenderer => GetComponent<SpriteRenderer>();
    private Timer timer = new Timer();
    private bool stopFlag = false;
    private bool isStanding = true;

    public void InvokeStanding()
    {
        isStanding = true;
        FrameSetter(standingAnimation, 0);
    }
    public void InvokeWalking()
    {
        isStanding = false;
        FrameSetter(walkingAnimation, 0);
    }
    public void Flip()
    {
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }
    private async void FrameSetter(AnimationSO myanim, int currentframe)
    {
        timer.SetFrequency(myanim.FPS);
        spriteRenderer.sprite = myanim.Frames[currentframe];
        while (!timer.Execute())
        {
            if (((standingAnimation == myanim) != isStanding) || stopFlag) return;
            await Task.Yield();
        }
        currentframe++;
        currentframe %= myanim.Frames.Count;
        FrameSetter(myanim, currentframe);
    }
    private void OnApplicationQuit()
    {
        stopFlag = true;
    }
}
