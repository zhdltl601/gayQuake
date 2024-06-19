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
        
        if(_enemy.CanAction() == false)
            return;

        if (RunAway()) return;
                
        if (_enemy.target != null && Vector3.Distance(_enemy.transform.position , _enemy.target.position) <= _enemy.attackDistance)
        {
            _enemy.StateMachine.ChangeState(_enemy.AttackState);
            return;
        }
        
        /*if (_enemy.target != null && Vector3.Distance(_enemy.transform.position , _enemy.target.position) >= _enemy.minDistance)
        {
            _enemy.isAttackMove = true;
            _enemy.StateMachine.ChangeState(_enemy.MoveState);
            return;
        }*/
        
        if (_enemy.target != null)
        {
            _enemy.StateMachine.ChangeState(_enemy.MoveState);
            return;
        }
                

        Collider player = _enemy.IsPlayerDetected();
        if (player != null)
        {
            _enemy.target = player.transform;
        }
        
    }

    private bool RunAway()
    {
        if (_enemy.target != null && _enemy.runAwayCount > 0 &&Vector3.Distance(_enemy.transform.position, _enemy.target.position) <= _enemy.runAwayDistance)
        {
            if (_enemy.runAwayAble &&
                Physics.Raycast(_enemy.transform.position + new Vector3(0, 1, 0), -_enemy.transform.forward,
                    out RaycastHit hit,
                    _enemy.runAwayDistance) == false)
            {
                _enemy.StateMachine.ChangeState(_enemy.RunAwayState);
                return true;
            }
        }
        return false;
    }

    public override void Exit()
    {
        base.Exit();
    }
    
    
}