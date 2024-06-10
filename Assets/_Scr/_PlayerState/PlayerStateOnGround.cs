using UnityEngine;
public class PlayerStateOnGround : PlayerStateBaseDefault
{
    private float delayWallrun;
    private float wallRunTime;
    private float wallRunTimerEnd = 0.1f;
    private bool? isRight = null;
    private Collider lastWall;
    public PlayerStateOnGround(Player player) : base(player)
    {
    }
    public override void Enter()
    {
        base.Enter();
        delayWallrun = 0.4f;
        wallRunTime = 0;
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
        float wallRunPercent = wallRunTime / wallRunTimerEnd;
        this.isRight = isGround ? null : this.isRight;

        bool isSamewall = this.isRight == isRight && lastWall == col;

        if (isOnWall && !isGround && delayWallrun < 0 && wallRunPercent > 1 && !isSamewall)
        {
            StateMachine<PlayerStateEnum>.Instance.ChangeState(PlayerStateEnum.OnWallrun);
            lastWall = col;
            this.isRight = isRight;
        }
        wallRunTime = isOnWall ? wallRunTime : 0;
        wallRunTime += isOnWall && !isGround ? Time.deltaTime : 0;
    }
    public override void Update()
    {
        base.Update();
        delayWallrun -= Time.deltaTime;
        //PlayerUI.Instance.lists[0].text = wallRunTime.ToString();
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