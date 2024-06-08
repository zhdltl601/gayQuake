using UnityEngine;

public class EnemyAttackState : EnemyState
{
    public EnemyAttackState(Enemy enemy, Animator animator, string animBoolName) : base(enemy, animator, animBoolName)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
        _enemy.NavMeshAgent.isStopped = true;

    }

    public override void Update()
    {
        base.Update();
        
        LookPlayer();
        
        if (endTriggerCalled)
        {
            _enemy.StateMachine.ChangeState(_enemy.IdleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        _enemy.NavMeshAgent.isStopped = false;
    }

    private void LookPlayer()
    {
        Vector3 target = _enemy.target.position - _enemy.transform.position;
        target.y = 0;
        Quaternion targetRot = Quaternion.Lerp(_enemy.transform.rotation,Quaternion.LookRotation(target) , 10 * Time.deltaTime);

        _enemy.transform.rotation = targetRot;
    }
}