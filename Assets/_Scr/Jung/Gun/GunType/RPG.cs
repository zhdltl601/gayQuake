using System.Collections.Generic;
using UnityEngine;

public class RPG : Gun
{
    public float maxDistance;
    public float sphereRadius;
    public GameObject boomEffect;

     public override GameObject[] Shoot()
    {
        //총알
        gunData.ammoInMagazine--;
        GameObject[] bullet = new GameObject[1];
        
        bullet[0] = ObjectPooling.Instance.GetObject(gunData.bullet);
        ObjectPooling.Instance.ReTurnObject(bullet[0] , 2);
    
        bool isHit = Physics.SphereCast(playerCam.position, sphereRadius, playerCam.forward, out RaycastHit hit, maxDistance, whatIsEnemy);
        Vector3 direction = playerCam.forward;
        
        if (isHit)
        {
            direction = hit.point - _firePos.position;
            Health enemyHealth = hit.transform.GetComponent<Health>();

            if (_weaponController.GetCurrenBottle() is AttackBottle)
            {
                enemyHealth.ApplyDamage(PlayerStatController.Instance.PlayerStatSo._statDic[StatType.Attack].GetValue() + gunData.damage,hit.normal , hit.point);
            }
            else
            {
                enemyHealth.ApplyDamage(gunData.damage , hit.normal , hit.point);
            }

             //boomParticle
            GameObject newBoomEffect =  ObjectPooling.Instance.GetObject(boomEffect);
            newBoomEffect.transform.position = hit.point;
            newBoomEffect.transform.rotation = Quaternion.LookRotation(Vector3.up);
            ObjectPooling.Instance.ReTurnObject(newBoomEffect,2f);
        }

        bullet[0].transform.position = _firePos.position;
        bullet[0].GetComponent<Rigidbody>().AddForce(direction.normalized * gunData.bulletSpeed);
                        
        //탄피
        // GameObject newCaseShell = ObjectPooling.Instance.GetObject(caseShell);
        
        // newCaseShell.transform.position = _caseShellPos.position;

        // newCaseShell.GetComponent<Rigidbody>().AddForce((Vector3.right + Vector3.up) * Random.Range(200, 220));
        // newCaseShell.GetComponent<Rigidbody>().angularVelocity = new 
        //         Vector3(Random.Range(-100 ,100) , Random.Range(-100 , 100) , Random.Range(-100 , 100));
        
        
        // ObjectPooling.Instance.ReTurnObject(newCaseShell , 3);
        
       

        //muzzle Effect
        foreach (var item in muzzles)
        {
            item.Simulate(0);
            item.Play();
        }

        return bullet;
    }


}
