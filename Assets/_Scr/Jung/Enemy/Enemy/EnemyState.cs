using UnityEngine;

public class EnemyState
{
    protected Enemy _enemy;
    protected Animator _animator;
    protected string _animBoolName;
    
    protected bool endTriggerCalled;
    
    public EnemyState( Enemy enemy, Animator animator, string animBoolName)
    {
        _enemy = enemy;
        _animator = animator;
        _animBoolName = animBoolName;
    }

    public virtual void Enter()
    {
        _animator.SetBool(_animBoolName , true);
        endTriggerCalled = false;
    }

    public virtual void Update()
    {
        
    }

    public virtual void Exit()
    {
        _animator.SetBool(_animBoolName , false);
    }
}