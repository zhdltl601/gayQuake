using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public PlayerAnimator PlayerAnimator;

    [Header("Gun Settings")]
    public Gun currentGun;
    private float _lastShootTime;
    private bool reloading;
    
    [Header("Bottle Settings")] 
    public Bottle currentBottle;
    public List<Bottle> bottleList;
    
    [Header("Pivots")]
    public Transform gunTrm;
    public Transform bottleTrm;
    
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
            Equip();
            
            Shot();
            Reload();
            ThrowGun();
        }
        else
        {
            equip = false;
        }

        if (currentBottle != null && currentBottle)
        {
            DrinkBottle();
            currentBottle.DecreaseBottle();
        }
        
        if (currentGun != null && currentBottle as AmmoBottle)
        {
            currentGun.gunMagazine.totalAmmo += (currentBottle as AmmoBottle).GetAmmo();
        }
        
        ChangeBottle();
       
    }
    private void Equip()
    {
        if (equip == false)
        {
            equip = true;
            PlayerAnimator.leftArmAnimator.runtimeAnimatorController = currentGun.runtimeAnimatorController;
            PlayerAnimator.leftArmAnimator.enabled = true;
            PlayerAnimator.leftArmAnimator.Rebind();
            PlayerAnimator.leftArmAnimator.Play("Equip", -1, 0);
            
            UIManager.Instance.SetCrosshair(currentGun.gunData.crossHair);
            SoundManager.Instance.PlayPlayerSOund("Equip");
        }
    }

    private void Reload()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentGun.ReLoad();
            StartCoroutine(ReloadCoroutine(currentGun.gunData.reloadTime));
            
            UIManager.Instance.Reload(currentGun.gunData.reloadTime);
        }
    }
    private void ThrowGun()
    {
        if (Input.GetKeyDown(KeyCode.G) && reloading == false)
        {
            PlayerAnimator.leftArmAnimator.enabled = false;
            currentGun.ThrowGun();
            currentGun = null;
            
        }
    }
    private void Shot()
    {
        bool shootAble = currentGun.gunMagazine.ammoInMagazine > 0 &&
                         _lastShootTime + currentGun.gunData.shotRate < Time.time && !reloading;
        
        if (Input.GetKey(KeyCode.Mouse0) && shootAble)
        {
            GameObject[] bullets = currentGun.Shoot();
                        
            _player.playerCamera.CameraShakePos(0.11f);
            
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
        
        PlayerAnimator.rightArmAnimator.runtimeAnimatorController = currentBottle.AnimatorController;
        PlayerAnimator.rightArmAnimator.Rebind();
        
        PlayerAnimator.rightArmAnimator.Play("Equip" , -1 , 0f);
        
        UIManager.Instance.PopupText($"{currentBottle._bottleDataSo.bottleName}");
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
            if (currentBottle._bottleDataSo.bottleType == BottleType.Normal)
            {
                Destroy(currentBottle.gameObject);
                bottleList.Remove(currentBottle);
            }
            
            currentBottle.DrinkBottle(_player.playerAnimator.rightArmAnimator);
            
            Invoke(nameof(SetBottleDefault), 1f);
        }
    }
    private void SetBottleDefault()
    {
        currentBottle = bottleList[0];
        currentBottle.gameObject.SetActive(true);
    }

    IEnumerator ReloadCoroutine(float duration)
    {
        reloading = true;
        yield return new WaitForSeconds(duration);
        reloading = false;


        UIManager.Instance.SetAmmoText();
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
