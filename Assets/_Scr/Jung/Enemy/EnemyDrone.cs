using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyDrone : MonoBehaviour,EnemyMapSetting
{
    private readonly int _dissolveHash = Shader.PropertyToID("_DissolveHeight");
    public StateMachine<EnemyStateEnum> StateMachine { get; private set; }
    public Collider Collider { get; private set; }
    public Animator Animator  { get; private set; }
    public Rigidbody Rigidbody  { get; private set; }
    public Transform target;

    private Room _room;
    
    [HideInInspector] public Collider[] enemyCheckCollider;
    [Header("DefaultSettings")]
    public LayerMask whatIsPlayer;
    public MeshRenderer meshRenderer;
    public float moveSpeed;
    public float runAwayDistance;
    public float attackDistance;
    public int increaseAmount;
    
    [Header("ZigZag~")]
    public float zigzagTime;
    [HideInInspector] public float zigzagDuration;
    
    [Header("Attack Settings")]
    public Transform[] firePos;
    public GameObject bullet;
    public float attackTime;
    public int damage;
    
    [Space]
    public List<ParticleSystem> explosionParticleList;
    public float dissolveDuration;
    
    private void Awake()
    {
        Animator = GetComponentInChildren<Animator>();
        Rigidbody = GetComponent<Rigidbody>();
        Collider = GetComponent<Collider>();
    }
    
    private void Start()
    {
        enemyCheckCollider = new Collider[1];
        StateMachine = new StateMachine<EnemyStateEnum>();
        
        StateMachine.AddState(EnemyStateEnum.Idle , new EnemyDroneIdleState(this ,Animator));
        StateMachine.AddState(EnemyStateEnum.Move , new EnemyDroneMoveState(this ,Animator));
        StateMachine.AddState(EnemyStateEnum.Dead , new EnemyDroneDeadState(this ,Animator));
        
        StateMachine.Initialize(EnemyStateEnum.Idle);

        transform.position = new Vector3(transform.position.x , 4, transform.position.z);
    }

    private void Update()
    {
        StateMachine.CurrentState.Update();
    }

    public void HitEvent()
    {
        Animator.SetTrigger("Hit");
    }
    
    public void DeadEvent()
    {
        
        if (target != null)
        {
            Bottle _playerBottle = target.GetComponent<WeaponController>().currentBottle;
            PlayerStatController.Instance.PlayerStatSo._statDic[_playerBottle._bottleDataSo.statType].AddValue(increaseAmount);
        }
        
        foreach (var item in explosionParticleList )
        {
            item.Simulate(0);
            item.Play();
        }
        StateMachine.ChangeState(EnemyStateEnum.Dead);
        
        StartDissolve();
        RemoveEnemy();
    }
    
    public virtual Collider IsPlayerDetected()
    {
        int cnt = Physics.OverlapSphereNonAlloc(transform.position,runAwayDistance, enemyCheckCollider,whatIsPlayer);
        return cnt == 1 ? enemyCheckCollider[0] : null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position , runAwayDistance);
        Gizmos.color = Color.white;
    }
        
    public void ZigZag()
    {
        StartCoroutine(ZigzagCoroutine());
    }

    private IEnumerator ZigzagCoroutine()
    {
        zigzagDuration = Random.Range(-5,5);
        Rigidbody.AddForce(transform.right * zigzagDuration * 25, ForceMode.Force);
        yield return new WaitForSeconds(zigzagTime);
    }

    public void StartDissolve()
    {
        StartCoroutine(StartDissolveCoroutine(_dissolveHash));
    }
    public IEnumerator StartDissolveCoroutine(int _dissolveHash)
    {
        Material mat = meshRenderer.material;

        float currentTime = 0;
        while(currentTime <= dissolveDuration)
        {
            currentTime += Time.deltaTime;
            float currentDissolve = Mathf.Lerp(2f, -2f, currentTime/dissolveDuration);
            
            mat.SetFloat(_dissolveHash, currentDissolve);
            yield return null;
        }
        
        yield return new WaitForSeconds(0.1f);
        
        Destroy(gameObject);
    }

    public void SetRoom(Room room)
    {
        _room = room;
    }

    public void RemoveEnemy()
    {
        _room.aliveEnemyNames.Remove(gameObject);
    }
}