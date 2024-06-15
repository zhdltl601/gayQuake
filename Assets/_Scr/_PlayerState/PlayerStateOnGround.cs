using UnityEngine;
public class PlayerStateOnGround : PlayerStateBaseDefault
{
    private float delayWallrun;
    private float wallRunHoldTime;
    private float wallRunHoldTimerEnd = 0.12f;
    //private bool? isRightLast = null;
    private Collider lastWall = null;
    public PlayerStateOnGround(Player player) : base(player)
    {
    }
    public override void Enter()
    {
        base.Enter();
        delayWallrun = 0.4f;
        wallRunHoldTime = 0;
    }
    protected override Vector3 GetDirection(Vector3 inputDirection)
    {
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

        //isRightLast = isGround ? null : isRightLast;
        lastWall = isGround ? null : lastWall;

        if (isPlWallrunable && isWallRunDealyEnded && !isSameWall)
        {
            StateMachine<PlayerStateEnum>.Instance.ChangeState(PlayerStateEnum.OnWallrun);
            lastWall = col;
            //this.isRightLast = isRight;
        }
        wallRunHoldTime = isOnWall ? wallRunHoldTime : 0;
        wallRunHoldTime += isPlWallrunable? Time.deltaTime : 0;
        delayWallrun -= Time.deltaTime;
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