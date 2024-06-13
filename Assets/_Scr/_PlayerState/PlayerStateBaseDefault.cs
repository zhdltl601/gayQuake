using UnityEngine;
public abstract class PlayerStateBaseDefault : State
{
    protected Player player;
    protected PlayerStateBaseDefault(Player player)
    {
        this.player = player;
    }
    public override void Enter()
    {
        base.Enter();
        player.Mov += HandleMove;
        player.OnJump += HandleOnJump;
    }
    public override void Exit()
    {
        base.Exit();
        player.Mov -= HandleMove;
        player.OnJump -= HandleOnJump;
    }
    public override void Update()
    {
        base.Update();
    }
    protected virtual void HandleMove(Vector3 inputDirection)
    {
        HandleState();
        Vector3 result = GetDirection(inputDirection);
        float speed = GetSpeed();
        float gravityMultiplier = GetGravitiyMultiplier();
        player.PlayerApplyMovement(result, speed, gravityMultiplier);
    }
    protected virtual void HandleOnJump()
    {
    }
    protected void Jump()
    {
        player.SetYVal(player.jumpForce);
    }
    protected virtual float GetGravitiyMultiplier()
    {
        return 1.2f;
    }
    protected virtual float GetSpeed()
    {
        return player.speed;
    }
    protected virtual Vector3 GetDirection(Vector3 inputDirection)
    {
        Vector3 direction = inputDirection;
        direction = direction.magnitude < 1 ? direction : direction.normalized; // needs optimazation

        return direction;
    }
    protected virtual void HandleState()
    {

    }
    /*
Ps
-Def
//viewmodel difference?
//
--OnGround { fuck2 }
--OnDash   { fuck3 }
--OnJump   { jump  }
--OnWallrun{ Lean to R/L }
-Dead
*/
}
