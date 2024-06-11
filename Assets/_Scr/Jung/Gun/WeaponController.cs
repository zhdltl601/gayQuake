using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Gun Settings")]
    public Gun currentGun;
    private float _lastShootTime;
    private float shotTime;
    
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

        if (currentBottle != null && currentBottle._bottleDataSo.bottleType != BottleType.Special)
        {
            DrinkBottle();
            currentBottle.DecreaseBottle();
        }
        
        ChangeBottle();
    }

    private void Reload()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentGun.ReLoad();
            
            UIManager.Instance.SetAmmoText();
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
        shotTime += Time.deltaTime;
        
        bool shootAble = currentGun.gunData.ammoInMagazine > 0 &&
                         _lastShootTime + currentGun.gunData.shotRate < Time.time;
        
        if (Input.GetKey(KeyCode.Mouse0) && shootAble)
        {
            GameObject[] bullets = currentGun.Shoot();

            for (int i = 0; i < bullets.Length; i++)
            {
                Bullet newBullet = bullets[i].GetComponent<Bullet>();
                newBullet.SetBullet(playerCam.transform.forward);
                   
            }
            _lastShootTime = Time.time;

            float randomXRecoil = Random.Range(-currentGun.gunData.xBound,currentGun.gunData.xBound);
            Recoil(randomXRecoil * Time.deltaTime, currentGun.gunData.yBound * Time.deltaTime);
            _player.playerAnimator.leftArmAnimator.Play("AutoShoot", -1, 0f);

            UIManager.Instance.SetAmmoText();
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0) || shootAble == false)
        {
            
            Recoil(0,0);
        }
    }

    private void Recoil(float xAmount , float yAmount)
    {
        _player.AddRecoil(xAmount,yAmount);
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

    private void DrinkBottle()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            bottleList.Remove(currentBottle);
            currentBottle.DrinkBottle(_player.playerAnimator.rightArmAnimator);
            
            Invoke("SetBottleDefault" , 1f);
           
        }
    }


    private void SetBottleDefault()
    {
        currentBottle = bottleList[0];
        currentBottle.gameObject.SetActive(true);
    }
    
    public Gun GetCurrentGun()
    {
        return currentGun == null ? null : currentGun;
    }
}
