using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class ShotGun : Gun
{
    public float shotgunSize;
    
    public override GameObject[] Shoot()
    {
        gunData.ammoInMagazine--;

        int _shotCount = Random.Range(gunData.minShotCount,gunData.maxShotCount);
        GameObject[] bullets = new GameObject[_shotCount];
        Vector3 bulletDirection = Vector3.zero;
        
        for (int i = 0; i < _shotCount; i++)
        {   
            bullets[i] = ObjectPooling.Instance.GetObject(gunData.bullet);
            
            var position = _firePos.position;
            bullets[i].transform.position = position;
            
            ObjectPooling.Instance.ReTurnObject(bullets[i] , 2);
            
            bool isHit = Physics.SphereCast
            (
                position
                , transform.lossyScale.x * shotgunSize, 
                _firePos.forward, 
                out RaycastHit hit, 
                2.5f , whatIsEnemy);
            
            if (isHit)
            {
                Health enemyHealth = hit.transform.GetComponent<Health>();

                if(_weaponController.GetCurrenBottle() is AttackBottle){
                    enemyHealth.ApplyDamage(PlayerStatController.Instance.PlayerStatSo._statDic[StatType.Attack].GetValue() + gunData.damage 
                        ,hit.normal , hit.point);
                }
                else
                {
                    enemyHealth.ApplyDamage(gunData.damage , hit.normal , hit.point);
                }
                bulletDirection = hit.transform.position - _firePos.position;
            }
            else
            {
                bulletDirection = _firePos.forward;
            }
            
            bulletDirection.Normalize();
            Vector3 randPos = new Vector3(Random.Range(-0.25f,0.25f) , Random.Range(-0.25f , 0.25f) , 0);
                
            
            bullets[i].GetComponent<Rigidbody>().AddForce((_firePos.up + bulletDirection + randPos).normalized * gunData.bulletSpeed);
        }
              
        foreach (var item in muzzles)
        {
            item.Simulate(0);
            item.Play();
        }
        
        return bullets;
    }

}
