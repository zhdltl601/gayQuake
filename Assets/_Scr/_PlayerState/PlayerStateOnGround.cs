using UnityEngine;
public class PlayerStateOnGround : PlayerStateBaseDefault
{
    public PlayerStateOnGround(Player player) : base(player)
    {
    }
    public override void Enter()
    {
        base.Enter();
    }
    protected override Vector3 GetDirection(Vector3 inputDirection)
    {
        return base.GetDirection(inputDirection) + inputDirection.normalized * player.GetDashCurve();
    }
    protected override void HandleOnJump()
    {
        bool isGround = player.playerController.IsGround;
        if (isGround) Jump();
        else if (player.CheckWall())
        {
            StateMachine<PlayerStateEnum>.Instance.ChangeState(PlayerStateEnum.OnWallrun);
        }
    }
}