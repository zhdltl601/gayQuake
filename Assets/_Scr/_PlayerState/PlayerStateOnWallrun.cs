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
        player.SetYVal(1.2f);
        timerSinceEnter = 0;
        player.CheckWall(out raycastHit, out bool isRight);
        player.playerAnimator.camAnimator.Play("OnWall");

        currentDir = -raycastHit.normal;
        currentDir.Normalize();

        player.playerViewmodel.WallRun(isRight ? 10 : -10);
        gravityMultiplier = 0;
        //player.StartCoroutine(Cor_MoveTowardsToWall(raycastHit.point, raycastHit.normal));
        //Current = Update;
    }
    private IEnumerator Cor_MoveTowardsToWall(Vector3 point, Vector3 normal)
    {
        Vector3 tar = point - new Vector3(0, point.y - player.transform.position.y, 0);
        tar += normal * 0.4f;
        while (Vector3.Distance(player.transform.position, tar) > 0.01f)
        {
            yield return new WaitForFixedUpdate();
            player.transform.position = Vector3.MoveTowards(player.transform.position, tar, Time.deltaTime * 0.4f);
        }
        base.Enter();
    }
    protected override void HandleOnJump()
    {
        StateMachine<PlayerStateEnum>.Instance.ChangeState(PlayerStateEnum.OnGround);
        player.SetYVal(6.5f);
    }
    protected override float GetGravitiyMultiplier()
    {
        gravityMultiplier += Time.deltaTime * 0.4f;
        return gravityMultiplier;
    }
    protected override float GetSpeed()
    {
        timerSinceEnter += Time.deltaTime;
        return player.speedWall * player.GetWallrunCurve(timerSinceEnter);
    }
    protected override Vector3 GetDirection(Vector3 inputDirection)
    {
        Vector3 forwardVector = Vector3.ProjectOnPlane(inputDirection, raycastHit.normal);
        forwardVector = forwardVector.sqrMagnitude < 1 ? forwardVector : forwardVector.normalized;
        return forwardVector;
    }
    protected override void HandleState()
    {
        Debug.DrawRay(player.transform.position, currentDir, Color.red, Time.deltaTime);
        Vector3 pForward = player.playerCamera.GetCameraRotTransform().forward;
        pForward.y = 0;
        pForward.Normalize();
        Debug.DrawRay(player.transform.position, pForward, Color.red, Time.deltaTime);

        float angle = Vector3.Angle(pForward, currentDir);
        bool isOver = angle > 90 + 35 || angle < 90 - 10;
        if (isOver)
        {
            StateMachine<PlayerStateEnum>.Instance.ChangeState(PlayerStateEnum.OnGround);
        }

        if (player.playerController.IsGround || !player.CheckWall(out raycastHit, ref currentDir))
        {
            StateMachine<PlayerStateEnum>.Instance.ChangeState(PlayerStateEnum.OnGround);
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.playerViewmodel.WallRun(0);
        player.AddForce(-currentDir, 7);
    }
}
