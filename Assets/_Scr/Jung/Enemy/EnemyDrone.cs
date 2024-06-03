using System;
using UnityEngine;

public class EnemyDrone : MonoBehaviour
{
    public StateMachine<EnemyStateEnum> StateMachine { get; private set; }

    private void Start()
    {
        StateMachine = new StateMachine<EnemyStateEnum>();
        
        StateMachine.AddState(EnemyStateEnum.Idle , new EnemyDroneIdleState());
        StateMachine.AddState(EnemyStateEnum.Move , new EnemyDroneMoveState());
        StateMachine.AddState(EnemyStateEnum.Attack , new EnemyDroneAttackState());
        StateMachine.AddState(EnemyStateEnum.Dead , new EnemyDroneDeadState());
        
        StateMachine.Initialize(EnemyStateEnum.Idle);
    }
}