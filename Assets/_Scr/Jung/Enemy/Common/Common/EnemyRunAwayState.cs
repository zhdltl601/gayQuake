using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class EnemyRunAwayState : EnemyState
{
    private Vector3 runAwayTargetPos;
    
    public EnemyRunAwayState(Enemy enemy, Animator animator, string animBoolName) : base(enemy, animator, animBoolName)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
        
        _enemy.NavMeshAgent.isStopped = false;
        _enemy.runningAway = true;
        
        runAwayTargetPos = _enemy.runAwayTrm.position;

        _enemy.NavMeshAgent.SetDestination(runAwayTargetPos);
    }

    public override void Update()
    {
        base.Update();
        Debug.DrawRay(_enemy.transform.position + new Vector3(0,1,0), -(_enemy.transform.position - runAwayTargetPos).normalized * 3, Color.green, Time.deltaTime);
        if (Vector3.Distance(_enemy.transform.position, runAwayTargetPos) <= 1.5f || 
            Physics.Raycast(_enemy.transform.position, -(_enemy.transform.position - runAwayTargetPos).normalized, 3f))
        {
           _enemy.StateMachine.ChangeState(_enemy.IdleState);
        }
        //else
        //{
        //    _enemy.NavMeshAgent.SetDestination(runAwayTargetPos);
        //}
    }

    public override void Exit()
    {
        base.Exit();

        //_enemy.transform.rotation = Quaternion.LookRotation(_enemy.target.position);
        
        _enemy.NavMeshAgent.isStopped = true;
        _enemy.runningAway = false;
        _enemy.runAwayCount--;
    }
}
