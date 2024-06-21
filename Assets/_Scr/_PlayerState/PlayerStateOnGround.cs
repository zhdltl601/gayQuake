using UnityEngine;
public class PlayerStateOnGround : PlayerStateBaseDefault
{
    private float delayWallrun;
    private float wallRunHoldTime;
    private const float wallRunHoldTimerEnd = 0.12f;
    //private bool? isRightLast = null;
    private Collider lastWall = null;
    private Vector3 dir;
    private float timeSinceMoving = 0;
    public PlayerStateOnGround(Player player) : base(player)
    {
    }
    public override void Enter()
    {
        base.Enter();
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
        bool isOnWall = player.CheckWall(out RaycastHit r, out bool isRight, out Collider col);
        bool isGround = player.playerController.IsGround;
        bool isPlWallrunable = isOnWall && !isGround;

        bool isWallRunDealyEnded = wallRunHoldTime > wallRunHoldTimerEnd && delayWallrun < 0;
        //bool isSameDirection = isRightLast == isRight;
        bool isSameWall = lastWall == col;

        Vector3 igDir = dir;
        igDir = player.playerCamera.GetCameraRotTransform().InverseTransformDirection(igDir);
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
        return base.GetSpeed() + player.GetSpeedCurve(timeSinceMoving);
    }
    protected override void HandleMove(Vector3 inputDirection)
    {
        base.HandleMove(inputDirection);
        bool isMoving = inputDirection.sqrMagnitude > 0.25f;
        timeSinceMoving += isMoving ? Time.deltaTime : -Time.deltaTime;
        timeSinceMoving = Mathf.Clamp(timeSinceMoving, 0, 1);
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
        bool isGround = player.playerController.IsGround;
        if (isGround)
        {
            Jump();
            delayWallrun = 0.2f;
        }
    }
}