using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateOnGround : PlayerStateBaseDefault
{
    public PlayerStateOnGround(Player player) : base(player)
    {
    }

    public override void Enter()
    {
        base.Enter();
        //Debug.Log("on g");
    }
    protected override void HandleState()
    {
        if(player.CheckWallRun(out RaycastHit raycastHit, out bool isRight) && !player.playerController.IsGround)
        {
            StateMachine<PlayerStateEnum>.Instance.ChangeState2(PlayerStateEnum.OnWallrun);
        }

    }
}
