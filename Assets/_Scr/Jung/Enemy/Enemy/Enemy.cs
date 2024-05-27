using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public EnemyStateMachine StateMachine { get; private set; }

    #region states

    public EnemyIdleState IdleState { get; private set; }
    public EnemyMoveState MoveState { get; private set; }
    public EnemyAttackState AttackState { get; private set; }
    public EnemyDeadState DeadState { get; private set; }
    
    #endregion

    #region component
    public Animator Animator { get; private set; }
    public NavMeshAgent NavMeshAgent { get; private set; }
    public Rigidbody Rigidbody { get; private set; }
    public SkinnedMeshRenderer _MeshRenderer;
    #endregion
    
    
    private Collider[] _enemyCheckCollider;

    public Transform target;
    public LayerMask _whatIsPlayer;
    
    [Header("Movement Values")]
    public float runAwayDistance;
    public float attackDistance;
    private void Awake()
    {
        Animator = GetComponentInChildren<Animator>();
        NavMeshAgent = GetComponent<NavMeshAgent>();
        Rigidbody = GetComponent<Rigidbody>();
        
        
        StateMachine = new EnemyStateMachine();

        #region stats
        IdleState = new EnemyIdleState(this , Animator , "Idle");
        MoveState = new EnemyMoveState(this , Animator , "Move");
        AttackState = new EnemyAttackState(this , Animator , "Attack");
        DeadState = new EnemyDeadState(this , Animator , "Dead");
        #endregion
        
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

    public void AnimationEnd()
    {
        StateMachine.currentState.AnimationFinish();;
    }
    
    public IEnumerator StartDissolve(int _dissolveHash)
    {
        //여기서 셰이더 처리 할거고

        Material[] mat = _MeshRenderer.materials;

        float currentTime = 0;
        while(currentTime <= 1f)
        {
            currentTime += Time.deltaTime;
            float currentDissovle = Mathf.Lerp(2f, -2f, currentTime);
            
            mat[0].SetFloat(_dissolveHash, currentDissovle);
            mat[1].SetFloat(_dissolveHash, currentDissovle);
            yield return null;
        }
        
        yield return new WaitForSeconds(0.1f);
               
    }
    
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position , runAwayDistance);
    }
    
    
}