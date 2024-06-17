using UnityEngine;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class ShotGun : Gun
{
    public float shotGunDistance;
    public float shotgunRadius;

    public override GameObject[] Shoot()
    {
        //총알
        gunData.ammoInMagazine--;
        int shotCount = Random.Range(gunData.minShotCount , gunData.maxShotCount);
        
        GameObject[] bullet = new GameObject[shotCount];
        Vector3 direction = playerCam.forward;

        bool isHit = Physics.Raycast(playerCam.position , playerCam.forward,out RaycastHit hit ,shotGunDistance,whatIsEnemy);
        if (isHit)
        {
            Collider[] cols = Physics.OverlapSphere(hit.point , shotgunRadius , whatIsEnemy);
            
            foreach (var item in cols)
            {
                GameObject newbullet = ObjectPooling.Instance.GetObject(gunData.bullet);
                newbullet.transform.position = _firePos.position;
                newbullet.transform.forward = direction;
                ObjectPooling.Instance.ReTurnObject(newbullet , 2);
                
                if (item != null)
                {
                    direction = item.transform.position - _firePos.position;
                    
                    Health health = item.transform.GetComponent<Health>();
                    
                    Vector3 closestPoint = item.ClosestPoint(hit.point);
                    Vector3 normal = (hit.point - closestPoint).normalized;
                    
                    ApplyDamage(health , normal , closestPoint);
                }
                
                newbullet.GetComponent<Rigidbody>().AddForce(direction.normalized * gunData.bulletSpeed);
            }
        }
        else
        {
            for(int i = 0; i < shotGunDistance; i++)
            {
                GameObject newbullet = ObjectPooling.Instance.GetObject(gunData.bullet);
                newbullet.transform.position = _firePos.position;

                ObjectPooling.Instance.ReTurnObject(newbullet , 2);
                newbullet.GetComponent<Rigidbody>().AddForce(direction* gunData.bulletSpeed);
            }
        }

        //탄피
        GameObject newCaseShell = ObjectPooling.Instance.GetObject(caseShell);
        
        newCaseShell.transform.position = _caseShellPos.position;

        newCaseShell.GetComponent<Rigidbody>().AddForce((Vector3.right + Vector3.up) * Random.Range(200, 220));
        newCaseShell.GetComponent<Rigidbody>().angularVelocity = new 
            Vector3(Random.Range(-100 ,100) , Random.Range(-100 , 100) , Random.Range(-100 , 100));
        
        
        ObjectPooling.Instance.ReTurnObject(newCaseShell , 3);
        
        //muzzle Effect
        foreach (var item in muzzles)
        {
            item.Simulate(0);
            item.Play();
        }

        return bullet;
    }

    
}
