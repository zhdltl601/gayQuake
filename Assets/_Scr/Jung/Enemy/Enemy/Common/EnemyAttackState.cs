using UnityEngine;

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
        
        LookPlayer();
        
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

    private void LookPlayer()
    {
        Vector3 target = _enemy.target.position - _enemy.transform.position;
        target.y = 0;
        
        //Quaternion targetRot = Quaternion.Lerp(_enemy.transform.rotation, , 10 * Time.deltaTime);

        _enemy.transform.rotation = Quaternion.LookRotation(target);
    }
}