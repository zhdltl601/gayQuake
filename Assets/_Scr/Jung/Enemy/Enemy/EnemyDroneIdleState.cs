using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDroneIdleState : EnemyDroneBaseState
{
    public EnemyDroneIdleState(EnemyDrone enemyDrone, Animator animator) : base(enemyDrone, animator)
    {
        
    }
    
    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
        
        Collider player = _enemyDrone.IsPlayerDetected();
        if (player != null)
        {
            _enemyDrone.target = player.transform;
            _enemyDrone.StateMachine.ChangeState(EnemyStateEnum.Move);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    
}