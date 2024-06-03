using UnityEngine;

public class EnemyDeadState : EnemyState
{
    private readonly int _dissolveHash = Shader.PropertyToID("_DissolveHeight");
    private bool _isDissolving = false; 

    public EnemyDeadState(Enemy enemy, Animator animator, string animBoolName) : base(enemy, animator, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        _enemy.Collider.enabled = false;
        _isDissolving = false;
    }

    public override void Update()
    {
        base.Update();
        
        if(endTriggerCalled && _isDissolving == false)
        {
            _isDissolving = true;
            _enemy.StartCoroutine(_enemy.StartDissolve(_dissolveHash));
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
