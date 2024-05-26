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
        
        if(_enemy.target != null)return;
        
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