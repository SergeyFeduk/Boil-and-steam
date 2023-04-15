using UnityEngine;
using System.Threading.Tasks;

public class PlayerAnimator : MonoBehaviour {
    private const float minimalAcceptableVelocity = 0.01f;
    [SerializeField] private AnimationSO standingAnimation, walkingAnimation;
    private SpriteRenderer spriteRenderer => GetComponent<SpriteRenderer>();
    private AnimationSO currentAnimation;
    private Timer timer = new Timer();
    private bool stopFlag;

    private void Update() {
        Vector2 movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if ((movement.x > minimalAcceptableVelocity && spriteRenderer.flipX) || (movement.x < -minimalAcceptableVelocity && !spriteRenderer.flipX)) {
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }
        bool isMoving = Mathf.Abs(movement.x) > minimalAcceptableVelocity || Mathf.Abs(movement.y) > minimalAcceptableVelocity;
        AnimationSO targetAnimationSO = isMoving ? walkingAnimation : standingAnimation;
        if (currentAnimation != targetAnimationSO) {
            ChangeAnimation(targetAnimationSO);
        }
    }
    private void ChangeAnimation(AnimationSO anim) {
        currentAnimation = anim;
        FrameSetter(anim, 0);
    }
    private async void FrameSetter(AnimationSO myanim, int currentframe) {
        timer.SetFrequency(myanim.FPS);
        spriteRenderer.sprite = myanim.Frames[currentframe];
        while (!timer.Execute()) {
            if (myanim != currentAnimation || stopFlag) return;
            await Task.Yield();
        }
        currentframe++;
        currentframe %= currentAnimation.Frames.Count;
        FrameSetter(myanim, currentframe);
    }
    private void OnApplicationQuit() {
        stopFlag = true;
    }
}
