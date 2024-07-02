using System.Collections;
using UnityEngine;
public class PlayerStateOnWallrun : PlayerStateBaseDefault
{
    private float timerSinceEnter;
    private float gravityMultiplier;
    private Vector3 currentDir;
    private RaycastHit raycastHit;
    public PlayerStateOnWallrun(Player player) : base(player)
    {
    }
    public override void Enter()
    {
        base.Enter();
        player.SetYVal(player.jumpForce * 0.3f);
        timerSinceEnter = 0;
        player.CheckWall(out raycastHit, out bool isRight);
        player.PlayerAnimator.camAnimator.Play("OnWall");

        currentDir = -raycastHit.normal;
        currentDir.Normalize();

        player.PlayerViewmodel.WallRun(isRight ? player.wallrunViewmodelAngle : -player.wallrunViewmodelAngle);
        gravityMultiplier = 0;

    }
    protected override void HandleOnJump()
    {
        StateMachine<PlayerStateEnum>.Instance.ChangeState(PlayerStateEnum.OnGround);
        player.SetYVal(player.jumpForce);
    }
    protected override float GetGravitiyMultiplier()
    {
        gravityMultiplier += Time.deltaTime * player.gravityMultiOnWall;
        return gravityMultiplier;
    }
    protected override float GetSpeed()
    {
        timerSinceEnter += Time.deltaTime;
        return player.speedWall * player.GetWallrunCurve(timerSinceEnter);
    }
    protected override Vector3 GetDirection(Vector3 inputDirection)
    {
        Vector3 igVector = inputDirection;
        igVector = player.PlayerCamera.GetCameraRotTransform().InverseTransformDirection(igVector);
        igVector.x = 0f;
        bool isNotOnlyHoldingInputdrectionX = igVector.sqrMagnitude > 0.15f;
        if (!isNotOnlyHoldingInputdrectionX)
        {
            StateMachine<PlayerStateEnum>.Instance.ChangeState(PlayerStateEnum.OnGround);
            player.AddForce(currentDir.normalized, 4);
        }
        Vector3 forwardVector = Vector3.ProjectOnPlane(inputDirection, raycastHit.normal);
        forwardVector.Normalize();
        return forwardVector;
    }
    protected override void HandleState()
    {
        Vector3 pForward = player.PlayerCamera.GetCameraRotTransform().forward;
        pForward.y = 0;
        pForward.Normalize();

        float angle = Vector3.Angle(pForward, currentDir); //need optimazation
        bool isOver = angle > 90 + player.allowedWallrunAngleMax || angle < 90 - player.allowedWallrunAngleMin;
        if (isOver || !player.CheckWall(out raycastHit, ref currentDir))
        {
            StateMachine<PlayerStateEnum>.Instance.ChangeState(PlayerStateEnum.OnGround);
        }
        if (player.PlayerController.IsGround)
        {
            StateMachine<PlayerStateEnum>.Instance.ChangeState(PlayerStateEnum.OnGround);
            player.AddForce(currentDir, 2.1f);
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.PlayerViewmodel.WallRun(0);
        float wallrunCurveClamped = player.GetWallrunCurve(timerSinceEnter);
        wallrunCurveClamped = Mathf.Clamp(wallrunCurveClamped, 1, 1.4f);
        player.AddForce(-currentDir, 2.1f * wallrunCurveClamped);

        float wallrunCurveClampedForward = Mathf.Clamp(wallrunCurveClamped, 0.5f, 1.6f);
        Vector3 forwardVector = player.PlayerCamera.GetCameraRotTransform().forward;
        forwardVector.y = 0;
        forwardVector.Normalize();
        player.AddForce(forwardVector, wallrunCurveClampedForward * 5.6f);
        player.SetYVal(1);
    }
}
