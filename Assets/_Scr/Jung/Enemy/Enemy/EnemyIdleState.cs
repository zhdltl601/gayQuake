using UnityEngine;

public class EnemyIdleState : EnemyState
{
    public EnemyIdleState(Enemy enemy, Animator animator, string animBoolName) : base(enemy, animator, animBoolName)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        if (_enemy.target != null && Vector3.Distance(_enemy.transform.position , _enemy.target.position) <= _enemy.attackDistance)
        {
            _enemy.StateMachine.ChangeState(_enemy.AttackState);
            return;
        }
        
        if (_enemy.target != null)
        {
            _enemy.StateMachine.ChangeState(_enemy.MoveState);
        }
        
        Collider player = _enemy.IsPlayerDetected();
        if (player != null)
        {
            _enemy.target = player.transform;
            _enemy.StateMachine.ChangeState(_enemy.MoveState);
        }
        
    }

    public override void Exit()
    {
        base.Exit();
    }
    
    
}