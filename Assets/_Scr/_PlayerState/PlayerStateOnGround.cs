using UnityEngine;
public class PlayerStateOnGround : PlayerStateBaseDefault
{
    private float delayWallrun; // for preventing player from wallrunning real fast
    private float wallRunHoldTime; //timer
    private const float wallRunHoldTimerEnd = 0.12f;// wall holdTime to change state

    private float timeSinceMoving = 0; //timer 2

    private Collider lastWall = null;
    //private bool? isRightLast = null;

    private Vector3 inputDir;

    public PlayerStateOnGround(Player player) : base(player)
    {
    }
    public override void Enter()
    {
        base.Enter();
        timeSinceMoving = 0;
        delayWallrun = 0.25f;
        wallRunHoldTime = 0;
    }
    protected override Vector3 GetDirection(Vector3 inputDirection)
    {
        if (player.IsHoldingAnyInput)
        {
            inputDirection.Normalize();
            return inputDirection + inputDirection * player.GetDashCurve();
        }
        return base.GetDirection(inputDirection) + inputDirection.normalized * player.GetDashCurve();
    }
    protected override void HandleState()
    {
        base.HandleState();
        bool isOnWall = player.CheckWall(out _, out _, out Collider col);
        bool isGround = player.PlayerController.IsGround;
        bool isPlWallrunable = isOnWall && !isGround;

        bool isWallRunDealyEnded = wallRunHoldTime > wallRunHoldTimerEnd && delayWallrun < 0;
        bool isSameWall = lastWall == col;
        //bool isSameDirection = isRightLast == isRight;

        Vector3 igDir = inputDir;
        igDir = player.PlayerCamera.GetCameraRotTransform().InverseTransformDirection(igDir);
        igDir.x = 0;
        //Debug.DrawRay(Vector3.zero, igDir, Color.red, Time.deltaTime);
        bool isNotOnlyHoldingInputdrectionX = igDir.sqrMagnitude > 0.15f;

        //isRightLast = isGround ? null : isRightLast;

        lastWall = isGround ? null : lastWall;

        if (isPlWallrunable && isWallRunDealyEnded && !isSameWall && isNotOnlyHoldingInputdrectionX)
        {
            StateMachine<PlayerStateEnum>.Instance.ChangeState(PlayerStateEnum.OnWallrun);
            lastWall = col;
            //this.isRightLast = isRight;
        }
        wallRunHoldTime = isOnWall ? wallRunHoldTime : 0;
        wallRunHoldTime += isPlWallrunable? Time.deltaTime * 2.2f : 0;
        delayWallrun -= Time.deltaTime;
    }
    protected override float GetSpeed()
    {
        float additionalSpeed = 1;
        if (player.IsHoldingAnyInput)
        {
            additionalSpeed = player.GetSpeedCurve(timeSinceMoving);
        }
        return base.GetSpeed() * additionalSpeed;
    }
    protected override void HandleMove(Vector3 inputDirection)
    {
        inputDir = inputDirection;

        base.HandleMove(inputDirection);

        timeSinceMoving += player.IsHoldingAnyInput ? Time.deltaTime : - Time.deltaTime;
        timeSinceMoving = Mathf.Clamp(timeSinceMoving, 0, 0.5f);
    }
    protected override void HandleOnDash()
    {
        Dash();
    }
    protected override float GetGravitiyMultiplier()
    {
        return player.gravityMultiOnGround;
    }
    protected override void HandleOnJump()
    {
        bool isGround = player.PlayerController.IsGround;
        if (isGround) Jump();
    }
}