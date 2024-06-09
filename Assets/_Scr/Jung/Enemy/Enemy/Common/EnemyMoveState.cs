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
        
        if(Vector3.Distance(_enemy.transform.position , _enemy.target.position) <= _enemy.attackDistance && _enemy.CanAttack())
        {
            _enemy.StateMachine.ChangeState(_enemy.AttackState);
        }
        
        _enemy.NavMeshAgent.SetDestination(_enemy.target.position);
        LookTarget();
    }

    private void LookTarget()
    {
        Vector3 target = _enemy.target.position - _enemy.transform.position;
        target.y = 0;
        _enemy.transform.rotation = Quaternion.LookRotation(target);
    }

    public override void Exit()
    {
        base.Exit();
        _enemy.NavMeshAgent.isStopped = true;
    }
}