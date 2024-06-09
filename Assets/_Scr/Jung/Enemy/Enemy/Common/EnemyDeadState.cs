using UnityEngine;

public class EnemyDeadState : EnemyState
{

    private bool isDissolving = false;
    public EnemyDeadState(Enemy enemy, Animator animator, string animBoolName) : base(enemy, animator, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        _enemy.Collider.enabled = false;
    }

    public override void Update()
    {
        base.Update();
        if (endTriggerCalled && isDissolving == false)
        {
            isDissolving = true;
            _enemy.Dissolve();
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
