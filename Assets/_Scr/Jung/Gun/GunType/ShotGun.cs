

using UnityEngine;

public class ShotGun : Gun
{
    public override void Shoot()
    {
        int shotCount = Random.Range(gunData.minShotCount,gunData.maxShotCount);

      
        
        for (int i = 0; i < shotCount; i++)
        {
            gunData.ammoInMagazine--;
            Vector3 randPos = new Vector3(Random.Range(-1f,1f) , 0 , 0);
            GameObject bullet = Instantiate(gunData.bullet,_firePos.position,Quaternion.identity);
            
            bullet.GetComponent<Rigidbody>().AddForce((_firePos.right + randPos).normalized * gunData.bulletSpeed);
        }
        
        //StartCoroutine(ReboundCoroutine());
    }

    public override void ReLoad()
    {
        base.ReLoad();
    }

    public override void Aim()
    {
        base.Aim();
    }
}
