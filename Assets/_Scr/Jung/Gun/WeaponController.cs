using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public PlayerAnimator PlayerAnimator;
    
    [Header("Gun Settings")]
    public Gun currentGun;
    private float _lastShootTime;
    
    [Header("Bottle Settings")] 
    public Bottle currentBottle;
    public List<Bottle> bottleList;
    public Transform gunTrm;
    private int currentBottleIndex;
    
    private Player _player;
    private Transform playerCam;

    private bool equip = false;
    
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
            if (equip == false)
            {
                equip = true;
                //currentGun.transform.localPosition = Vector3.zero;
                //currentGun.transform.localRotation = Quaternion.Euler(0,0,0);
                
                PlayerAnimator.leftArmAnimator.enabled = true;
                PlayerAnimator.leftArmAnimator.runtimeAnimatorController = currentGun.runtimeAnimatorController;
                //PlayerAnimator.leftArmAnimator.Rebind();
                PlayerAnimator.leftArmAnimator.Play("Equip", -1, 0);
            }
            
            Shot();
            Reload();
            ThrowGun();
        }
        else
        {
            equip = false;
        }

        if (currentBottle != null && currentBottle._bottleDataSo.bottleType != BottleType.Special)
        {
            DrinkBottle();
            currentBottle.DecreaseBottle();
        }

        if (currentBottle as AmmoBottle)
        {
            currentGun.gunData.totalAmmo += (currentBottle as AmmoBottle).GetAmmo();
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
            PlayerAnimator.leftArmAnimator.enabled = false;
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
                if(bullets[i] == null){
                    continue;
                }
                
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
        float scroll = -Input.GetAxisRaw("Mouse ScrollWheel") * 10;
        
        if(scroll == 0)return;
        currentBottleIndex += (int)scroll;
        
        currentBottleIndex = Mathf.Clamp(currentBottleIndex,0 ,bottleList.Count - 1);
        SwitchBottle(currentBottleIndex);
    }
    private void SwitchBottle(int index)
    {
        if(currentGun == null)return;
        
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
            
            Invoke(nameof(SetBottleDefault), 1f);
           
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

    public Bottle GetCurrenBottle()
    {
        return currentBottle;
    }
}
