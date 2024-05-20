

using UnityEngine;

public class ShotGun : Gun
{
    public override void Shoot()
    {
        int shotCount = Random.Range(gunData.minShotCount,gunData.maxShotCount);
        
        for (int i = 0; i < shotCount; i++)
        {
            gunData.ammoInMagazine--;
            GameObject bullet = Instantiate(gunData.bullet,_firePos.position,Quaternion.identity);
            bullet.GetComponent<Rigidbody>().AddForce(_firePos.right * gunData.bulletSpeed);
        }
        
        StartCoroutine(ReboundCoroutine());
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
