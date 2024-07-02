using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WeaponController : MonoBehaviour
{
    public PlayerAnimator playerAnimator;

    [Header("Gun Settings")]
    public Gun currentGun;
    private float _lastShootTime;
    private bool _reloading;
    
    [Header("Bottle Settings")] 
    public Bottle currentBottle;
    public List<Bottle> bottleList;
    
    [Header("Pivots")]
    public Transform gunTrm;
    public Transform bottleTrm;
    
    private int _currentBottleIndex;
    
    private Player _player;
    private Transform _playerCam;

    private bool _equip = false;

    private void Awake()
    {
        Time.timeScale = 1;
    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        _playerCam = Camera.main.transform;
        _player = GetComponent<Player>();
        
        UIManager.Instance.CoinText();
        UIManager.Instance.SetBottleUI($"<color=#40739e>{currentBottle._bottleDataSo.bottleName}</color>: {currentBottle._bottleDataSo.bottleExplain}");
        
       playerAnimator.rightArmAnimator.runtimeAnimatorController = currentBottle.AnimatorController;
       playerAnimator.rightArmAnimator.Play("Equip" , -1 , 0f);
        
        //Equip();
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
            _equip = false;
        }

        if (currentBottle != null && currentBottle)
        {
            DrinkBottle();
            currentBottle.DecreaseBottle();
        }
        
        ApplyAmmoBottle();
        ChangeBottle();
    }

    private void ApplyAmmoBottle()
    {
        if (currentGun != null && currentBottle is AmmoBottle)
        {
            currentGun.gunMagazine.totalAmmo += ((AmmoBottle)currentBottle).GetAmmo();
        }
    }

    private void Equip()
    {
        if (_equip == false)
        {
            _equip = true;
            playerAnimator.leftArmAnimator.runtimeAnimatorController = currentGun.runtimeAnimatorController;
            playerAnimator.leftArmAnimator.enabled = true;
            playerAnimator.leftArmAnimator.Rebind();
            playerAnimator.leftArmAnimator.Play("Equip", -1, 0);
            
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
        if (Input.GetKeyDown(KeyCode.G) && _reloading == false)
        {
            playerAnimator.leftArmAnimator.enabled = false;
            currentGun.ThrowGun();
            currentGun = null;
            
        }
    }
    private void Shot()
    {
        bool shootAble = currentGun.gunMagazine.ammoInMagazine > 0 &&
                         _lastShootTime + currentGun.gunData.shotRate < Time.time && !_reloading;
        
        if (Input.GetKey(KeyCode.Mouse0) && shootAble)
        {
            GameObject[] bullets = currentGun.Shoot();
                        
            _player.PlayerCamera.CameraShakePos(0.11f);
            
            for (int i = 0; i < bullets.Length; i++)
            {
                if(bullets[i] == null){
                    continue;
                }
                
                Bullet newBullet = bullets[i].GetComponent<Bullet>();
                newBullet.SetBullet(_playerCam.transform.forward);
            }
            
            _lastShootTime = Time.time;

            float randomXRecoil = Random.Range(-currentGun.gunData.xBound,currentGun.gunData.xBound);
            Recoil(randomXRecoil * Time.deltaTime, currentGun.gunData.yBound * Time.deltaTime);
            
            _player.PlayerAnimator.leftArmAnimator.Play("AutoShoot", -1, 0f);
            
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
        _currentBottleIndex += (int)scroll;
        _currentBottleIndex = Mathf.Clamp(_currentBottleIndex,0 ,bottleList.Count - 1);
        SwitchBottle(_currentBottleIndex);
        
        playerAnimator.rightArmAnimator.runtimeAnimatorController = currentBottle.AnimatorController;
        playerAnimator.rightArmAnimator.Play("Equip" , -1 , 0f);
    }
    private void SwitchBottle(int index)
    {
        if(currentGun == null)return;
        
        currentBottle.gameObject.SetActive(false);
        currentBottle = bottleList[index];
        currentBottle.gameObject.SetActive(true);

        UIManager.Instance.SetBottleUI($"<color=#40739e>{currentBottle._bottleDataSo.bottleName}</color>: {currentBottle._bottleDataSo.bottleExplain}");

    }
    private void DrinkBottle()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentBottle._bottleDataSo.bottleType == BottleType.Normal)
            {
                Destroy(currentBottle.gameObject);
                currentBottle.DrinkBottle(_player.PlayerAnimator.rightArmAnimator);
            
                Invoke(nameof(SetBottleDefault), 1f);
                bottleList.Remove(currentBottle);
            }
        }
    }
    private void SetBottleDefault()
    {
        currentBottle = bottleList[0];
        currentBottle.gameObject.SetActive(true);
    }

    IEnumerator ReloadCoroutine(float duration)
    {
        _reloading = true;
        yield return new WaitForSeconds(duration);
        _reloading = false;


        UIManager.Instance.SetAmmoText();
    }
    
    public Gun GetCurrentGun()
    {
        return currentGun == null ? null : currentGun;
    }

    public Bottle GetCurrenBottle()
    {
        return  currentBottle == null ? null : currentBottle;
    }
}
