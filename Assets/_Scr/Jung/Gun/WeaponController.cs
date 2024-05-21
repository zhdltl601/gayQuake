using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Gun Settings")]
    public Gun currentGun;
    private float _lastShootTime;

    [Header("Bottle Settings")] 
    public Bottle currentBottle;
    public List<Bottle> bottleList;
    
    private void Update()
    {
        bool shootAble = currentGun.gunData.ammoInMagazine > 0 &&
                         _lastShootTime + currentGun.gunData.shotRate < Time.time;
        
        if (Input.GetKey(KeyCode.Space) && shootAble)
        {
            currentGun.Shoot();
            
            _lastShootTime = Time.time;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            currentGun.ReLoad();
        }

        ChangeBottle();
    }

    private void ChangeBottle()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchBottle(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchBottle(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchBottle(2);
        }
    }
    private void SwitchBottle(int index)
    {
        
        
        
    }
}
