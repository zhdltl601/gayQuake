﻿using UnityEngine;

public class EnemyAttackState : EnemyState
{
    public EnemyAttackState(Enemy enemy, Animator animator, string animBoolName) : base(enemy, animator, animBoolName)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
        
    }

    public override void Update()
    {
        base.Update();

        
        if (endTriggerCalled)
        {
            _enemy.StateMachine.ChangeState(_enemy.IdleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        
        _enemy.lastAttackTime = Time.time;
    }

   
}