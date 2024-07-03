using Unity.VisualScripting;
using UnityEngine;

public class EnemyIdleState : EnemyState
{
    public EnemyIdleState(Enemy enemy, Animator animator, string animBoolName) : base(enemy, animator, animBoolName)
    {
        
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
            //Debug.DrawRay(_enemy.transform.position + new Vector3(0, 1, 0), (_enemy.runAwayTrm.position - _enemy.transform.position).normalized, Color.green, 1);
        if (_enemy.target != null && _enemy.runAwayCount > 0 &&Vector3.Distance(_enemy.transform.position, _enemy.target.position) <= _enemy.runAwayDistance)
        {
            if (_enemy.runAwayAble &&
                Physics.Raycast(_enemy.transform.position + new Vector3(0, 1, 0), (_enemy.runAwayTrm.position - _enemy.transform.position).normalized,
                    out RaycastHit hit,
                    _enemy.runAwayDistance) == false)
            {
                _enemy.StateMachine.ChangeState(_enemy.RunAwayState);
                return true;
            }
        }
        return false;
    }
}