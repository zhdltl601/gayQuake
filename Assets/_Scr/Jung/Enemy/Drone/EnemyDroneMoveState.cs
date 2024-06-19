using Unity.VisualScripting;
using UnityEngine;

public class EnemyDroneMoveState : EnemyDroneBaseState
{
    private Vector3 direction;
    private float attackTimer;
    private float timer;

    public EnemyDroneMoveState(EnemyDrone enemyDrone, Animator animator) : base(enemyDrone, animator)
    {
    }
    
    public override void Enter()
    {
        base.Enter();
        attackTimer = _enemyDrone.attackTime;
    }

    public override void Update()
    {
        base.Update();

        timer += Time.deltaTime;
        
        
        LookAtTarget();

        if (Vector3.Distance(_enemyDrone.transform.position, _enemyDrone.target.position) > _enemyDrone.attackDistance)
        {
            direction = _enemyDrone.target.position - _enemyDrone.transform.position;
            _enemyDrone.Rigidbody.velocity = direction.normalized * _enemyDrone.moveSpeed;//이동하고
            
            //_enemyDrone.ZigZag();
        }
        else
        {
            _enemyDrone.Rigidbody.velocity = Vector3.zero;
        }
        
        if (attackTimer <= timer)
        {
            Shot();
            timer = 0;
        }
    }
    
    

    private void LookAtTarget()
    {
        Vector3 directionToTarget = _enemyDrone.target.position - _enemyDrone.transform.position;
        directionToTarget.y = Mathf.Clamp(directionToTarget.y  , -30 , 30);
        
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        _enemyDrone.transform.rotation = 
            Quaternion.Lerp(_enemyDrone.transform.rotation, targetRotation, 10 * Time.deltaTime);
    }

    private void Shot()
    {
        
        for (int i = 0; i < _enemyDrone.firePos.Length; i++)
        {
            GameObject newBullet = ObjectPooling.Instance.GetObject(_enemyDrone.bullet);
            newBullet.transform.position = _enemyDrone.firePos[i].position;

            Vector3 direction = (_enemyDrone.target.position - _enemyDrone.firePos[i].position).normalized;
            
            if (Physics.Raycast(_enemyDrone.firePos[i].position + new Vector3(0,0.5f, 0) , direction , out RaycastHit hit , _enemyDrone.attackDistance,_enemyDrone.whatIsPlayer))
            {
                UIManager.Instance.BloodScreen(Color.red);
                PlayerStatController.Instance.PlayerStatSo._statDic[StatType.Health].SetDefaultValue(PlayerStatController.Instance.PlayerStatSo._statDic[StatType.Health].GetValue() - _enemyDrone.damage);
            }
            
            newBullet.GetComponent<Rigidbody>().AddForce(direction * 600);
            newBullet.transform.forward = direction;
        }

    }


    public override void Exit()
    {
        base.Exit();
    }

  
}