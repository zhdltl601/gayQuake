using UnityEngine;

public class EnemyDroneBaseState : State
{
    protected EnemyDrone _enemyDrone;
    protected Animator _animator;
    
   
    
    public EnemyDroneBaseState(EnemyDrone enemyDrone, Animator animator)
    {
        _enemyDrone = enemyDrone;
        _animator = animator;
    }
    
    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Exit()
    {
        base.Exit();
    }
    
  
}
