using UnityEngine;

public class GunController : MonoBehaviour
{
    public Gun currentGun;
    
    private float _lastShootTime;
    
    private void Update()
    {
        bool shootAble = currentGun.gunData.ammoInMagazine > 0 && _lastShootTime + currentGun.gunData.shotRate < Time.time;
        
        if (Input.GetKeyDown(KeyCode.Space) && shootAble)
        {
            currentGun.Shoot();
            
            _lastShootTime = Time.time;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            currentGun.ReLoad();
        }
    }
}
