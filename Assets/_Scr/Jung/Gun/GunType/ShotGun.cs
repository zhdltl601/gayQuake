

using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShotGun : Gun
{
    public override GameObject[] Shoot()
    {
        int shotCount = Random.Range(gunData.minShotCount,gunData.maxShotCount);
        GameObject[] bullets = new GameObject[shotCount];
        
        for (int i = 0; i < shotCount; i++)
        {
            gunData.ammoInMagazine--;
            Vector3 randPos = new Vector3(Random.Range(-0.5f,0.5f) , 0 , 0);
            
            bullets[i] = ObjectPooling.Instance.GetObject(gunData.bullet);
            bullets[i].transform.position = _firePos.position;
            
            bullets[i].GetComponent<Rigidbody>().AddForce((_firePos.right + randPos).normalized * gunData.bulletSpeed);
        }
        //StartCoroutine(ReboundCoroutine());

        return bullets;
    }

    
}
