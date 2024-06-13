using UnityEngine;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class ShotGun : Gun
{
    public float shotgunSize;
    private Collider[] _enemys;
    
    private Vector3[] bulletDir = new Vector3[4];

    protected override void Start()
    {
        base.Start();

        bulletDir = new Vector3[] {Vector3.up , Vector3.down , Vector3.right , Vector3.left};
    }

    public override GameObject[] Shoot()
    {
        gunData.ammoInMagazine--;

        int _shotCount = Random.Range(gunData.minShotCount,gunData.maxShotCount);
        GameObject[] bullets = new GameObject[_shotCount];
        _enemys = new Collider[_shotCount];
        Vector3 bulletDirection = Vector3.zero;
        
         int hitCount = Physics.OverlapSphereNonAlloc(
            _firePos.position,
            shotgunSize,
            _enemys,
            whatIsEnemy
        );
         //맞았을때
        for (int i = 0; i < hitCount; i++)
        {   
            bullets[i] = ObjectPooling.Instance.GetObject(gunData.bullet);
            ObjectPooling.Instance.ReTurnObject(bullets[i] , 2);

            bullets[i].transform.position = _firePos.position;
            
            bulletDirection = _enemys[i].transform.position - _firePos.position;
            
            Health enemyHealth = _enemys[i].transform.GetComponent<Health>();
            enemyHealth.ApplyDamage(PlayerStatController.Instance.PlayerStatSo._statDic[StatType.Attack].GetValue() + gunData.damage);
            enemyHealth.onHitEvent?.Invoke();
            
            bullets[i].GetComponent<Rigidbody>().AddForce((_firePos.up + bulletDirection).normalized * gunData.bulletSpeed);
        }
        //안맞았을때
        for (int i = 0; i < _shotCount - hitCount; i++)
        {
            int value = hitCount + i;
            bullets[value] = ObjectPooling.Instance.GetObject(gunData.bullet);
            ObjectPooling.Instance.ReTurnObject(bullets[value] , 2);

            bullets[value].transform.position = _firePos.position;
            
            bullets[value].GetComponent<Rigidbody>().AddForce((_firePos.up + bulletDirection).normalized * gunData.bulletSpeed);
        }
        
        foreach (var item in muzzles)
        {
            item.Simulate(0);
            item.Play();
        }
        
        return bullets;
    }

    /*private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_firePos.position , shotgunSize);
    }*/
}
