using UnityEngine;

public class EnemyMoveState : EnemyState
{
    public EnemyMoveState(Enemy enemy, Animator animator, string animBoolName) : base(enemy, animator, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        _enemy.NavMeshAgent.isStopped = false;

    }

    public override void Update()
    {
        base.Update();

        bool isClose = Vector3.Distance(_enemy.transform.position, _enemy.target.position) <= _enemy.attackDistance;
        
        if (_enemy.runningAway && isClose)
        {
            _enemy.target = null;
            _enemy.StateMachine.ChangeState(_enemy.IdleState);
            return;
        }
        
        if(isClose && 
           _enemy.CanAttack() && _enemy.IsObstacleInLine(100 , (_enemy.target.position - _enemy.transform.position).normalized) == false)
        {
            _enemy.StateMachine.ChangeState(_enemy.AttackState);
        }
        
        _enemy.NavMeshAgent.SetDestination(_enemy.target.position);
    }
    
    public override void Exit()
    {
        base.Exit();
        _enemy.NavMeshAgent.isStopped = true;
        _enemy.runningAway = false;
    }
}