using UnityEngine;

public class EnemyMoveState : EnemyState
{
    private Vector3 attackMoveTarget;
    public EnemyMoveState(Enemy enemy, Animator animator, string animBoolName) : base(enemy, animator, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        _enemy.NavMeshAgent.isStopped = false;
        
        if (_enemy.isAttackMove)
        {
            if (Vector3.Distance(_enemy.transform.position , _enemy.target.position) >= _enemy.minDistance)
            {
                GetAttackMoveDir();
            }
        }
    }   

    public override void Update()
    {
        base.Update();

        float distance = Vector3.Distance(_enemy.transform.position, _enemy.target.position);
        
        if (_enemy.isAttackMove)
        {
            if (distance <= 2)
            {
                _enemy.isAttackMove = false;
                _enemy.StateMachine.ChangeState(_enemy.IdleState);
            }
            else
            {
                _enemy.NavMeshAgent.SetDestination(attackMoveTarget);
            }
            return;
        }
        
        
        bool isClose = distance <= _enemy.attackDistance;
        
        if(isClose && 
           _enemy.CanAction() && _enemy.IsObstacleInLine(100 , (_enemy.target.position - _enemy.transform.position).normalized) == false)
        {
            _enemy.StateMachine.ChangeState(_enemy.AttackState);
        }
        
        _enemy.NavMeshAgent.SetDestination(_enemy.target.position);
    }

   

    public override void Exit()
    {
        base.Exit();
        attackMoveTarget = Vector3.zero;
        
        _enemy.NavMeshAgent.isStopped = true;
        _enemy.runningAway = false;
    }
    
    private void GetAttackMoveDir()
    {
        Vector3 directionToTarget = (_enemy.target.position - _enemy.transform.position).normalized;
        Vector3 destination = _enemy.transform.position + directionToTarget * 3;
        
        attackMoveTarget = destination;
    }
}