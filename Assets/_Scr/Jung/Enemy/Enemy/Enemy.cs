using System;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public EnemyStateMachine StateMachine { get; private set; }

    public EnemyIdleState IdleState { get; private set; }
    public EnemyMoveState MoveState { get; private set; }
    public Animator Animator { get; private set; }
    public NavMeshAgent NavMeshAgent { get; private set; }
    public Rigidbody Rigidbody { get; private set; }

    private  Collider[] _enemyCheckCollider;

    public Transform target;
    public LayerMask _whatIsPlayer;
    
    [Header("Movement Values")]
    public float runAwayDistance;
    
    private void Awake()
    {
        Animator = GetComponentInChildren<Animator>();
        NavMeshAgent = GetComponent<NavMeshAgent>();
        Rigidbody = GetComponent<Rigidbody>();
        
        
        StateMachine = new EnemyStateMachine();
        IdleState = new EnemyIdleState(this , Animator , "Idle");
        MoveState = new EnemyMoveState(this , Animator , "Move");
    }

    private void Start()
    {
        _enemyCheckCollider = new Collider[1];
        StateMachine.Init(IdleState);
    }

    private void Update()
    {
        StateMachine.currentState.Update();
    }
    
    public virtual Collider IsPlayerDetected()
    {
        int cnt = Physics.OverlapSphereNonAlloc(transform.position, runAwayDistance, _enemyCheckCollider, _whatIsPlayer);
        return cnt == 1 ? _enemyCheckCollider[0] : null;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position , runAwayDistance);
    }
}