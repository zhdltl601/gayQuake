using UnityEngine;

public class RPG : Gun
{
    public float rpgDistance;
    public float sphereRadius;

    public GameObject boomEffect;
    private Vector3 point;
     public override GameObject[] Shoot()
    {
          //총알
        gunMagazine.ammoInMagazine--;
        int shotCount = Random.Range(gunData.minShotCount , gunData.maxShotCount);
        
        GameObject[] bullet = new GameObject[shotCount];
        Vector3 direction = playerCam.forward;

        bool isHit = Physics.Raycast(playerCam.position , playerCam.forward,out RaycastHit hit ,rpgDistance,whatIsEnemy | whatIsGround);
        if (isHit)
        {
            Collider[] cols = Physics.OverlapSphere(hit.point , sphereRadius , whatIsEnemy);
            GameObject newbullet = ObjectPooling.Instance.GetObject(gunData.bullet);
            newbullet.transform.position = _firePos.position;
            newbullet.transform.forward = direction;
            
            ObjectPooling.Instance.ReTurnObject(newbullet , 2);
            
            direction = hit.transform.position - _firePos.position;
            newbullet.GetComponent<Rigidbody>().AddForce(direction.normalized * gunData.bulletSpeed);

            point = hit.point;
            Invoke("BoomEffect",0.4f);
            
            foreach (var item in cols)
            {
                if (item != null)
                {
                    Health health = item.transform.GetComponent<Health>();
                    if(health == null)continue;
                    
                    Vector3 closestPoint = item.ClosestPoint(hit.point);
                    Vector3 normal = (hit.point - closestPoint).normalized;
                    ApplyDamage(health , normal , closestPoint);
                }
            }
        }
        
        //muzzle Effect
        foreach (var item in muzzles)
        {
            item.Simulate(0);
            item.Play();
        }
        
        return bullet;
    }

     private void BoomEffect()
     {
         GameObject newBoomEffect = ObjectPooling.Instance.GetObject(boomEffect);
         newBoomEffect.transform.position = point;
         newBoomEffect.GetComponent<EffectPlayer>().OnlyParticlePlay();
         ObjectPooling.Instance.ReTurnObject(newBoomEffect, 2);
     }
}
