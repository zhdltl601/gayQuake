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
    public Transform gunTrm;

    private Player _player;
    private Transform playerCam;
    
    
    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        playerCam = Camera.main.transform;

        _player = GetComponent<Player>();
    }

    private void Update()
    {
        if (currentGun != null)
        {
            Shot();
            Reload();
            ThrowGun();
        }   
       
        
        ChangeBottle();
        currentBottle.DecreaseBottle();
    }

    private void Reload()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentGun.ReLoad();
        }
    }
    
    private void ThrowGun()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            currentGun.ThrowGun();
            currentGun = null;
        }
    }
    
    private void Shot()
    {
        bool shootAble = currentGun.gunData.ammoInMagazine > 0 &&
                         _lastShootTime + currentGun.gunData.shotRate < Time.time;
        
        if (Input.GetKey(KeyCode.Mouse0) && shootAble)
        {
            GameObject[] bullets = currentGun.Shoot();

            for (int i = 0; i < bullets.Length; i++)
            {
                Bullet newBullet = bullets[i].GetComponent<Bullet>();
                newBullet.SetBullet( 
                    currentBottle._bottleDataSo.increaseAmount,
                    playerCam.transform.forward);
            }
            _lastShootTime = Time.time;
            
            Recoil();
        }
    }


    private void Recoil()
    {
        float xBound = Random.Range(currentGun.gunData.xBound.x, currentGun.gunData.xBound.y);
        float yBound = Random.Range(currentGun.gunData.yBound.x, currentGun.gunData.yBound.x);
        
        _player.xRotation += xBound;
        _player.yRotation += yBound;
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
        currentBottle.gameObject.SetActive(false);
        currentBottle = bottleList[index];
        currentBottle.gameObject.SetActive(true);
    }
}
