using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerStateOnWallrun : PlayerStateBaseDefault
{
    public PlayerStateOnWallrun(Player player) : base(player)
    {
    }
    public override void Enter()
    {
        base.Enter();
        player.CheckWallRun(out RaycastHit raycastHit, out bool isRight);
        player.playerViewmodel.WallRun(isRight ? 30 : -30);
    }
    protected override Vector3 GetDirection(Vector3 inputDirection)
    {
        player.CheckWallRun(out RaycastHit raycastHit, out bool isRight); // need optimazation

        Vector3 forwardVector = Vector3.ProjectOnPlane(inputDirection, raycastHit.normal);
        //Transform camTrm = player.playerCamera.GetCameraRotTransform();
        //Debug.DrawRay(camTrm.position, inputDirection, Color.red);
        //Debug.DrawRay(camTrm.position, forwardVector, Color.green);
        forwardVector = forwardVector.magnitude < 1 ? forwardVector : forwardVector.normalized; // needs optimazation as well

        return forwardVector;
    }
    protected override void HandleState()
    {
        if (!player.CheckWallRun(out RaycastHit raycastHit, out bool isRight) || player.playerController.IsGround)
        {
            StateMachine<PlayerStateEnum>.Instance.ChangeState2(PlayerStateEnum.OnGround);
        }
    }
    public override void Exit()
    {
        base.Exit();
        player.playerViewmodel.WallRun(0);
    }
}
