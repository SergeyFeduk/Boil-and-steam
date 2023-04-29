using UnityEngine;

public class PlayerAnimator : MonoBehaviour {
    [field: SerializeField] public  BodyAnimator bodyAnimator { get; private set; }
    [field: SerializeField] public HandsAnimator handsAnimator { get; private set; }
    private bool isRight = true;
    private bool isMoving = false;
    public void Start()
    {
        handsAnimator.ChangeState(HandsAnimator.States.BareHands); //потом перенести
        bodyAnimator.InvokeStanding();
        handsAnimator.InvokeStanding();
    }
    public void Update()
    {
        Vector2 movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if ((transform.InverseTransformPoint(ScreenUtils.WorldMouse()).x > 0 && !isRight) || (transform.InverseTransformPoint(ScreenUtils.WorldMouse()).x <= 0 - Player.minimalAcceptableVelocity && isRight))
        {
            handsAnimator.Flip();
            bodyAnimator.Flip();
            isRight = !isRight;
        }
        bool isMovingNow = Mathf.Abs(movement.x) > Player.minimalAcceptableVelocity || Mathf.Abs(movement.y) > Player.minimalAcceptableVelocity;
        if(!isMovingNow && isMoving)
        {
            isMoving = false;
            bodyAnimator.InvokeStanding();
            handsAnimator.InvokeStanding();
        }
        else if(isMovingNow && !isMoving)
        {
            isMoving = true;
            bodyAnimator.InvokeWalking();
            handsAnimator.InvokeWalking();
        }
    }
}
