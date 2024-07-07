using System;
using UnityEngine;
public abstract class PlayerStateBaseDefault : State
{
    protected Player player;
    protected Vector3 viewmodelHintPos;
    protected Vector3 vel = Vector3.zero;
    protected PlayerStateBaseDefault(Player player)
    {
        this.player = player;
    }
    public override void Enter()
    {
        base.Enter();
        player.Mov += HandleMove;
        player.OnJump += HandleOnJump;
        player.OnDash += HandleOnDash;
        player.OnViewmodelHintPos += HandleOnViewmodel;
    }
    public override void Exit()
    {
        base.Exit();
        player.Mov -= HandleMove;
        player.OnJump -= HandleOnJump;
        player.OnDash -= HandleOnDash;
        player.OnViewmodelHintPos -= HandleOnViewmodel;
    }
    protected virtual void HandleOnViewmodel(Vector3 obj)
    {
        viewmodelHintPos = Vector3.SmoothDamp(viewmodelHintPos, obj, ref vel, Time.deltaTime * 0.3f);
        player.PlayerViewmodel.SetViewmodelHintPosition(viewmodelHintPos.x * player.viewmodelBobbingX, 0, viewmodelHintPos.z * player.viewmodelBobbingX);
        player.PlayerViewmodel.UpdateViewmodel();
    }
    protected virtual void HandleMove(Vector3 inputDirection)
    {
        HandleState();
        Vector3 result = GetDirection(inputDirection);
        float speed = GetSpeed();
        
        float gravityMultiplier = GetGravitiyMultiplier();
        float forceMulti = GetForceMultiplier();
        player.PlayerApplyMovement(result, speed, gravityMultiplier, forceMulti);
    }

    protected virtual float GetForceMultiplier()
    {
        return player.GetForceVectorCurve();
    }
    protected virtual void HandleOnJump()
    {
    }
    protected virtual void HandleOnDash()
    {
    }
    protected void Jump()
    {
        player.SetYVal(player.jumpForce);
    }
    protected void Dash()
    {
        player.PlayerAnimator.camAnimator.Play("OnDash", -1, 0);
    }
    protected virtual float GetGravitiyMultiplier()
    {
        return 1;
    }
    protected virtual float GetSpeed()
    {
        return player.Speed;
    }
    protected virtual Vector3 GetDirection(Vector3 inputDirection)
    {
        Vector3 direction = inputDirection;
        direction = direction.sqrMagnitude < 1 ? direction : direction.normalized; // needs optimazation

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
