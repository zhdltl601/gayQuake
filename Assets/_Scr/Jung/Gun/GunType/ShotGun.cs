

using UnityEngine;

public class ShotGun : Gun
{
    public override GameObject[] Shoot()
    {
        int shotCount = Random.Range(gunData.minShotCount,gunData.maxShotCount);
        GameObject[] bullet = new GameObject[shotCount];
        
        for (int i = 0; i < shotCount; i++)
        {
            gunData.ammoInMagazine--;
            Vector3 randPos = new Vector3(Random.Range(-0.5f,0.5f) , 0 , 0);
            
            bullet[i] = ObjectPooling.Instance.GetObject(gunData.bullet);
            bullet[i].transform.position = _firePos.position;
            
            bullet[i].GetComponent<Rigidbody>().AddForce((_firePos.right + randPos).normalized * gunData.bulletSpeed);
        }
        //StartCoroutine(ReboundCoroutine());

        return bullet;
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
