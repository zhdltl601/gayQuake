using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour,EnemyMapSetting
{
    public string dieSound;
    public EnemyStateMachine StateMachine { get; private set; }
    
    private readonly int _blinkValue = Shader.PropertyToID("_BlinkValue");
    private readonly int _dissolveHash = Shader.PropertyToID("_DissolveHeight");
    private bool _IsHit = false; 
    
    #region states

    public EnemyIdleState IdleState { get; private set; }
    public EnemyMoveState MoveState { get; private set; }
    public EnemyAttackState AttackState { get; private set; }
    public EnemyDeadState DeadState { get; private set; }

    public EnemyRunAwayState RunAwayState { get; private set; }

    #endregion

    #region component
    public Animator Animator { get; private set; }
    public NavMeshAgent NavMeshAgent { get; private set; }
    public Renderer[] _MeshRenderer;
    public Collider Collider { get; private set; }

    #endregion
    
    private Collider[] _enemyCheckCollider;
    
    public Transform target;
    public LayerMask _whatIsPlayer;

    private Room currentRoom;
    
    [Header("Default Values")]
    public float moveSpeed;
    public float checkPlayerDistance;
    public float dissolveDuration;
    public LayerMask whatIsObstacle;
    public bool isDead = false;

    [Header("AttackValue")]
    public float attackDistance;
    public float minDistance;
    public int attackDamage;
    public float attackTime;
    public Vector2 attackSpeed;
    //min =x max = y;    
    
    [Header("RumAway")] 
    public int runAwayCount;
    public float runAwayDistance;
    public Transform runAwayTrm;
    public bool runAwayAble;
    public bool runningAway;
    
    [HideInInspector] public float lastAttackTime;
    [HideInInspector] public bool isAttackMove;
    
    private void Awake()
    {
        Animator = GetComponentInChildren<Animator>();
        NavMeshAgent = GetComponent<NavMeshAgent>();
        Collider = GetComponent<Collider>();
        StateMachine = new EnemyStateMachine();

        #region states
        IdleState = new EnemyIdleState(this , Animator , "Idle");
        MoveState = new EnemyMoveState(this , Animator , "Move");
        RunAwayState = new EnemyRunAwayState(this , Animator , "Move");
        AttackState = new EnemyAttackState(this , Animator , "Attack");
        DeadState = new EnemyDeadState(this , Animator , "Dead");
        #endregion
        
        runAwayTrm.position -= (transform.forward * runAwayDistance);
    }
    
    private void OnEnable() 
    {
        Dissolve(-2, 5 , false);    
    }

    private void Start()
    {
        _enemyCheckCollider = new Collider[1];
        StateMachine.Init(IdleState);
        
        NavMeshAgent.speed = Random.Range(moveSpeed - 2f,moveSpeed);
        Animator.speed =  Random.Range(attackSpeed.x , attackSpeed.y);
    }

    private void Update()
    {
        StateMachine.currentState.Update();
        
        LookPlayer();
        if(target != null)
        {
            Vector3 pos = transform.position; pos.y = 0;
            Vector3 tar = target.position; tar.y = 0;
            Vector3 dr = pos - tar;
            dr.y = 0;
            dr.Normalize();
            runAwayTrm.position = transform.position +  dr * runAwayDistance;
        }
    }
    
    #region DieLogic

    public void DieEvent()
    {
        isDead = true;
        StateMachine.ChangeState(DeadState);
        if (target != null)
        {
            Bottle _playerBottle = target.GetComponent<WeaponController>().currentBottle;
            
            PlayerStatController.Instance.PlayerStatSo._statDic[_playerBottle._bottleDataSo.statType].
                AddValue(_playerBottle._bottleDataSo.increaseAmount);
            
            UIManager.Instance.CoinText();
        }
        
        RemoveEnemy();
        
        Animator.SetLayerWeight(1 , 0);
        NavMeshAgent.isStopped = true;
       
    }
    public void Dissolve(float startValue , float endValue , bool isEead)
    {
        StartCoroutine(StartDissolve(startValue , endValue , isDead));
    }
    private IEnumerator StartDissolve(float startValue , float endValue , bool isDead)
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
            float currentDissolve = Mathf.Lerp(startValue, endValue, currentTime/dissolveDuration);

            foreach (var item in mat)
            {
                item.SetFloat(_dissolveHash, currentDissolve);
            }   
            
            yield return null;
        }

        if(isDead){
            yield return new WaitForSeconds(0.1f);
            Destroy(gameObject);
        }        
       
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

    #region Detecteced
    public virtual Collider IsPlayerDetected()
    {
        int cnt = Physics.OverlapSphereNonAlloc(transform.position, checkPlayerDistance, _enemyCheckCollider, _whatIsPlayer);
        return cnt == 1 ? _enemyCheckCollider[0] : null;
    }
    
    public virtual bool IsObstacleInLine(float distance, Vector3 direction)
    {
        return Physics.Raycast(transform.position, direction, distance , whatIsObstacle);
    }
    

    #endregion

    #region RoomSettings
    public void SetRoom(Room room)
    {
        currentRoom = room;
    }
    public void RemoveEnemy()
    {
        currentRoom.aliveEnemys.Remove(gameObject);
    }
    #endregion
    
    
     public void AnimationEnd()
    {
        StateMachine.currentState.AnimationFinish();;
    }
     public bool CanAction()
    {
        return Time.time >= lastAttackTime + attackTime;
    }
     private void LookPlayer()
    {
        if(target == null || isDead || runningAway)return;
        
        Vector3 lookTargetPos = target.position - transform.position;
        lookTargetPos.y = 0;
        lookTargetPos.Normalize();
        
        Quaternion targetRot = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookTargetPos), 10 * Time.deltaTime);
        transform.rotation = targetRot;
        //_enemy.transform.rotation = Quaternion.LookRotation(target);
    }
     
     private void OnDrawGizmos()
     {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(runAwayTrm.position , 1);

        //Gizmos.color = Color.yellow;
        //Gizmos.DrawWireSphere(transform.position, runAwayDistance);

        //Gizmos.color = Color.blue;
        //Gizmos.DrawWireSphere(transform.position, minDistance);

        //Gizmos.color = Color.white;
        //Gizmos.DrawWireSphere(transform.position, attackDistance);

    }
}