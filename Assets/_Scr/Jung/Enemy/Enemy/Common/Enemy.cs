using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class Enemy : MonoBehaviour,EnemyMapSetting
{
    public EnemyStateMachine StateMachine { get; private set; }
    
    private readonly int _blinkValue = Shader.PropertyToID("_BlinkValue");
    private readonly int _dissolveHash = Shader.PropertyToID("_DissolveHeight");
    private bool _IsHit = false; 
    
    #region states

    public EnemyIdleState IdleState { get; private set; }
    public EnemyMoveState MoveState { get; private set; }
    public EnemyAttackState AttackState { get; private set; }
    public EnemyDeadState DeadState { get; private set; }
    
    #endregion

    #region component
    public Animator Animator { get; private set; }
    public NavMeshAgent NavMeshAgent { get; private set; }
    public Renderer[] _MeshRenderer;
    public Collider Collider { get; private set; }

    #endregion
    
    private Collider[] _enemyCheckCollider;
    private StatType dieStat;
    
    public Transform target;
    public LayerMask _whatIsPlayer;

    [HideInInspector] public Room currentRoom;
    
    [Header("Movement Values")]
    public float moveSpeed;
    public float runAwayDistance;
    public float attackDistance;
    [Space]
    public int attackDamage;
    public float attackTime;
    public int increaseAmount;
    public float dissolveDuration;
    
    [HideInInspector] public float lastAttackTime; 
    
    private void Awake()
    {
        Animator = GetComponentInChildren<Animator>();
        NavMeshAgent = GetComponent<NavMeshAgent>();
        Collider = GetComponent<Collider>();
        StateMachine = new EnemyStateMachine();

        #region states
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

        NavMeshAgent.speed = Random.Range(moveSpeed - 2f,moveSpeed);
        Animator.speed =  Random.Range(0.8f,1.3f);;
    }

    private void Update()
    {
        StateMachine.currentState.Update();
    }
    
    #region DieLogic

    public void DieEvent()
    {
        StateMachine.ChangeState(DeadState);
        
        if (target != null)
        {
            Bottle _playerBottle = target.GetComponent<WeaponController>().currentBottle;
            PlayerStatController.Instance.PlayerStatSo._statDic[_playerBottle._bottleDataSo.statType].AddValue(increaseAmount);
        }
        
        RemoveEnemy();
        
        Animator.SetLayerWeight(1 , 0);
        NavMeshAgent.isStopped = true;
       
    }
    public void Dissolve()
    {
        StartCoroutine(StartDissolve());
    }
    private IEnumerator StartDissolve()
    {
        Material[] mat;
        if (_MeshRenderer.Length >= 2)
        {
            mat = new Material[_MeshRenderer.Length];
            for (int i = 0; i < _MeshRenderer.Length; i++)
            {
                mat[i] = _MeshRenderer[i].material;
            }
        }
        else
        {
            mat = _MeshRenderer[0].materials;
           
        } 
        
        float currentTime = 0;
        while(currentTime <= dissolveDuration)
        {
            currentTime += Time.deltaTime;
            float currentDissolve = Mathf.Lerp(5f, -5f, currentTime/dissolveDuration);

            foreach (var item in mat)
            {
                item.SetFloat(_dissolveHash, currentDissolve);
            }   
            
            yield return null;
        }
        
        yield return new WaitForSeconds(0.1f);
        
        Destroy(gameObject);
    }

    #endregion

    #region HitLogit

    public void HitEvent()
    {
        if (_IsHit) return;
        
        
        Animator.SetTrigger("Hit");
        StartCoroutine(HitCoroutine());
    }
    IEnumerator HitCoroutine()
    {
        _IsHit = true;

        Material[] mat;
        if (_MeshRenderer.Length >= 2)
        {
            mat = new Material[_MeshRenderer.Length];
            for (int i = 0; i < _MeshRenderer.Length; i++)
            {
                mat[i] = _MeshRenderer[i].material;
            }
        }
        else
        {
            mat = _MeshRenderer[0].materials;
           
        } 
                
        for (int i = 0; i < _MeshRenderer.Length; i++)
        {
            mat[i] = _MeshRenderer[i].material;
        }
                    
        foreach (var item in mat)
        {
            item.SetFloat(_blinkValue,1);
        }
            
        yield return new WaitForSeconds(0.4f);
        _IsHit = false;
            
        NavMeshAgent.speed = moveSpeed / 5;
     
        foreach (var item in mat)
        {
            item.SetFloat(_blinkValue,0);
        }
                
        NavMeshAgent.speed = moveSpeed;
    }

    #endregion
   
    public virtual Collider IsPlayerDetected()
    {
        int cnt = Physics.OverlapSphereNonAlloc(transform.position, runAwayDistance, _enemyCheckCollider, _whatIsPlayer);
        return cnt == 1 ? _enemyCheckCollider[0] : null;
    }

    public void AnimationEnd()
    {
        StateMachine.currentState.AnimationFinish();;
    }
        
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position , runAwayDistance);
    }

    public void SetRoom(Room room)
    {
        currentRoom = room;
    }
    
    public void RemoveEnemy()
    {
        currentRoom.aliveEnemyNames.Remove(gameObject);
    }

    public bool CanAttack()
    {
        return Time.time >= lastAttackTime + attackTime;
    }
    
}