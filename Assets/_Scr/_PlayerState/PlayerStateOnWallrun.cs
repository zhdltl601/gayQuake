using System.Collections;
using UnityEngine;
public class PlayerStateOnWallrun : PlayerStateBaseDefault
{
    private float gravityMultiplier;
    private Vector3 currentDir;
    private RaycastHit raycastHit;
    public PlayerStateOnWallrun(Player player) : base(player)
    {
    }
    public override void Enter()
    {
        bool isRight = true;
        player.CheckWall(out raycastHit);

        currentDir = -raycastHit.normal;
        currentDir.Normalize();

        player.playerViewmodel.WallRun(isRight ? 30 : -30);
        gravityMultiplier = 0;
        player.StartCoroutine(Cor_MoveTowardsToWall(raycastHit.point, raycastHit.normal));
        Current = Update;
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
    }
    protected override float GetGravitiyMultiplier()
    {
        gravityMultiplier += Time.deltaTime * 0.5f * 0;
        return gravityMultiplier;
    }
    protected override float GetSpeed()
    {
        return player.speedWall;
    }
    protected override Vector3 GetDirection(Vector3 inputDirection)
    {
        Vector3 forwardVector = Vector3.ProjectOnPlane(inputDirection, raycastHit.normal);
        forwardVector = forwardVector.magnitude < 1 ? forwardVector : forwardVector.normalized;
        return forwardVector;
    }
    protected override void HandleState()
    {
        if (player.playerController.IsGround || !player.CheckWall(out raycastHit, ref currentDir))
        {
            StateMachine<PlayerStateEnum>.Instance.ChangeState(PlayerStateEnum.OnGround);
        }
    }
    protected override void HandleMove(Vector3 inputDirection)
    {
        base.HandleMove(inputDirection);
        Vector3 tar = raycastHit.point - new Vector3(0, raycastHit.point.y - player.transform.position.y, 0);
        tar += raycastHit.normal * 0.4f;
        //Debug.DrawRay(tar, Vector3.up, Color.yellow, 2);
        //Debug.DrawRay(player.transform.position, Vector3.up, Color.green, 2);
        
        Vector3 dir = tar - player.transform.position;
        Debug.DrawRay(tar, dir, Color.blue, 2);
        //player.AddMovementImpulse(dir, 1, 0);

    }
    public override void Exit()
    {
        base.Exit();
        player.playerViewmodel.WallRun(0);
    }
}
