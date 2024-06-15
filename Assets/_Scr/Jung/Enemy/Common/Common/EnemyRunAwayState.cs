using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class EnemyRunAwayState : EnemyState
{
    private Vector3 targetPos;
    
    public EnemyRunAwayState(Enemy enemy, Animator animator, string animBoolName) : base(enemy, animator, animBoolName)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
        
        _enemy.NavMeshAgent.isStopped = false;
        _enemy.runningAway = true;
        
        targetPos = _enemy.runAwayTrm.position;
    }

    public override void Update()
    {
        base.Update();

        if (Vector3.Distance(_enemy.transform.position, targetPos) <= 0.4f || 
            Physics.Raycast(_enemy.transform.position , _enemy.target.forward , 2f) == true)
        {
           _enemy.StateMachine.ChangeState(_enemy.IdleState);
        }
                        
        _enemy.NavMeshAgent.SetDestination(targetPos);
    }

    public override void Exit()
    {
        base.Exit();

        _enemy.transform.rotation = Quaternion.LookRotation(_enemy.target.position);
        
        _enemy.NavMeshAgent.isStopped = true;
        _enemy.runningAway = false;
        _enemy.runAwayCount--;
    }
}
