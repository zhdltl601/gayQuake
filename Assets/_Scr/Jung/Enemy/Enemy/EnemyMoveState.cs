using UnityEngine;

public class EnemyMoveState : EnemyState
{
    public EnemyMoveState(Enemy enemy, Animator animator, string animBoolName) : base(enemy, animator, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
       
    }

    public override void Update()
    {
        base.Update();
        
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
    }
}