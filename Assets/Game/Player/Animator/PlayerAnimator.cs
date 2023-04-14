using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class PlayerAnimator : MonoBehaviour
{
    private const float minimalAcceptableVelocity = 0.01f;
    [SerializeField] private AnimationSO Standing, Walking;
    private SpriteRenderer SR;
    private AnimationSO currentAnimation;
    private Timer timer;
    
    private void Start()
    {
        timer = new Timer();
        SR = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float movementX = Input.GetAxisRaw("Horizontal");
        if ((movementX > minimalAcceptableVelocity && SR.flipX) || (movementX < -minimalAcceptableVelocity && !SR.flipX))
        {
            SR.flipX = !SR.flipX;
        }
        if(Mathf.Abs(movementX) > minimalAcceptableVelocity || Mathf.Abs(Input.GetAxisRaw("Vertical")) > minimalAcceptableVelocity)
        {
            if(currentAnimation != Walking)
            {
                ChangeAnimation(Walking);
            }
        }   
        else
        {
            if(currentAnimation != Standing)
            {
                ChangeAnimation(Standing);
            }
        }
    }
    private void ChangeAnimation(AnimationSO anim)
    {
        currentAnimation = anim;
        FrameSetter(anim, 0);
    }
    private async void FrameSetter(AnimationSO myanim, int currentframe) 
    {
        timer.SetFrequency(myanim.FPS);
        SR.sprite = myanim.Frames[currentframe];
        while (!timer.Execute())
        {
            if (myanim != currentAnimation) return;
            await Task.Yield();
        }
        currentframe++;
        currentframe %= currentAnimation.Frames.Count;
        FrameSetter(myanim, currentframe);
    }
}
