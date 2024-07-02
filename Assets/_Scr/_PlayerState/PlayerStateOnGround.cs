using UnityEngine;
public class PlayerStateOnGround : PlayerStateBaseDefault
{
    private float delayWallrun; // for preventing player from wallrunnign real fast
    private float wallRunHoldTime; 
    private const float wallRunHoldTimerEnd = 0.12f;// wall holdTime to change state
    private float timeSinceMoving = 0;

    private float t;
    private float lastDirSqr;

    private Collider lastWall = null;
    //private bool? isRightLast = null;
    private Vector3 dir;
    private bool isHoldingAnyInput;

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
        dir = inputDirection;
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

        Vector3 igDir = dir;
        igDir = player.PlayerCamera.GetCameraRotTransform().InverseTransformDirection(igDir);
        igDir.x = 0;
        Debug.DrawRay(Vector3.zero, igDir, Color.red, Time.deltaTime);
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
        delayWallrun -= Time.deltaTime * 2.2f;
    }
    protected override float GetSpeed()
    {
        float additionalSpeed;
        if (isHoldingAnyInput)
        {
            additionalSpeed = player.GetSpeedCurve(timeSinceMoving);
            DebugUI.Instance.list[0].text = additionalSpeed.ToString();
        }
        else 
        {
            timeSinceMoving = 0;
            //later
            //t = timeSinceMoving;
            additionalSpeed = 1;
            //speed = player.GetSpeedStopCurve(t - timeSinceMoving);
        }
        return base.GetSpeed() + additionalSpeed;
    }
    protected override void HandleMove(Vector3 inputDirection)
    {
        Vector3 currentInput = inputDirection;
        bool isHoldingInput = currentInput.sqrMagnitude > lastDirSqr;
        currentInput = currentInput.sqrMagnitude < 1 ? currentInput : currentInput.normalized;
        isHoldingInput |= currentInput.sqrMagnitude + 0.01f >= 1;

        isHoldingAnyInput = isHoldingInput;
        lastDirSqr = inputDirection.sqrMagnitude;

        base.HandleMove(inputDirection);

        timeSinceMoving += isHoldingAnyInput ? Time.deltaTime : -Time.deltaTime;
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