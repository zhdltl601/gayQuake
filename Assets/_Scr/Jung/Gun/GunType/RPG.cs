using UnityEngine;

public class RPG : Gun
{
    public virtual GameObject[] Shoot()
    {
        //총알
        gunData.ammoInMagazine--;
        GameObject[] bullet = new GameObject[1];
        
        bullet[0] = ObjectPooling.Instance.GetObject(gunData.bullet);
        ObjectPooling.Instance.ReTurnObject(bullet[0] , 2);
    
        bool isHit = Physics.Raycast(playerCam.position , playerCam.forward,out RaycastHit hit ,100,whatIsEnemy);
        Vector3 direction = playerCam.forward;
        
        if (isHit)
        {
            direction = hit.point - _firePos.position;
            Health enemyHealth = hit.transform.GetComponent<Health>();
            enemyHealth.ApplyDamage(PlayerStatController.Instance.PlayerStatSo._statDic[StatType.Attack].GetValue() + gunData.damage);
            enemyHealth.onHitEvent?.Invoke();
        }
        
        bullet[0].transform.position = _firePos.position;
        bullet[0].GetComponent<Rigidbody>().AddForce(direction.normalized * gunData.bulletSpeed);
        
        //muzzle Effect
        foreach (var item in muzzles)
        {
            item.Simulate(0);
            item.Play();
        }

        return bullet;
    }
}
