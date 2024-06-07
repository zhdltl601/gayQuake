using UnityEngine;

public class EnemyDroneDeadState : EnemyDroneBaseState
{
    
    public EnemyDroneDeadState(EnemyDrone enemyDrone, Animator animator) : base(enemyDrone, animator)
    {
    }
    
    public override void Enter()
    {
        base.Enter();
        _enemyDrone.Rigidbody.constraints = RigidbodyConstraints.None;
        _enemyDrone.Rigidbody.isKinematic = false;
        _enemyDrone.Rigidbody.useGravity = true;
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