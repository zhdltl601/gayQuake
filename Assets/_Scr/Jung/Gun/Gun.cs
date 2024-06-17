using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Rendering;
using VHierarchy.Libs;
using Vector3 = UnityEngine.Vector3;

public abstract class Gun : MonoBehaviour
{
    public GunDataSO gunData;

    public RuntimeAnimatorController runtimeAnimatorController;
    
    [SerializeField] protected LayerMask whatIsEnemy;
    [SerializeField] protected LayerMask whatIsGround;
    
    [SerializeField] protected GameObject caseShell;
    [SerializeField] protected List<ParticleSystem> muzzles;
    
    protected Transform _caseShellPos;
    protected Rigidbody _rigidbody;
    protected BoxCollider _collider;
    
    [HideInInspector] public Transform _firePos;
    protected Transform playerCam;
    public bool throwing;

    protected WeaponController _weaponController;
    
    
    protected virtual void Start()
    {
        playerCam = Camera.main.transform;

        _weaponController = transform.root.GetComponent<WeaponController>();
        
        _firePos = GetComponentInChildren<GunModel>().GetFirePos();
        _caseShellPos = GetComponentInChildren<GunModel>().GetCaseShell();
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<BoxCollider>();
        
        
    }
    
    public virtual GameObject[] Shoot()
    {
        //총알
        gunData.ammoInMagazine--;
        GameObject[] bullet = new GameObject[1];
        
        bullet[0] = ObjectPooling.Instance.GetObject(gunData.bullet);
        ObjectPooling.Instance.ReTurnObject(bullet[0] , 2);
        
        bool isHit = Physics.Raycast(playerCam.position , playerCam.forward,out RaycastHit hit ,100,whatIsEnemy);
        Vector3 direction = playerCam.forward;
        
        if (isHit)
        {
            direction = hit.point - _firePos.position;
            Health enemyHealth = hit.transform.GetComponent<Health>();
            ApplyDamage(enemyHealth, hit.normal , hit.point);  
            
        }
        
        bullet[0].transform.position = _firePos.position;
        bullet[0].GetComponent<Rigidbody>().AddForce(direction.normalized * gunData.bulletSpeed);
                        
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

    protected void ApplyDamage(Health enemyHealth , Vector3 normal , Vector3 point)
    {
        if (_weaponController.GetCurrenBottle() is AttackBottle)
        {
            enemyHealth.ApplyDamage(
                PlayerStatController.Instance.PlayerStatSo._statDic[StatType.Attack].GetValue() + gunData.damage,
                normal, point);
        }
        else
        {
            enemyHealth.ApplyDamage(gunData.damage, normal, point);
        }
    }

    public virtual void ReLoad()
    {
        int needAmmo = gunData.maxAmmoInMagazine - gunData.ammoInMagazine;

        if (gunData.totalAmmo - needAmmo < 0)
        {
            gunData.ammoInMagazine += gunData.totalAmmo;
            gunData.totalAmmo = 0;
        }
        else
        {
            gunData.totalAmmo -= needAmmo;
            gunData.ammoInMagazine = gunData.maxAmmoInMagazine;
        }
    }
    public virtual void ThrowGun()
    {
        SetOnGround();
        
        Vector3 random = (gameObject.transform.forward);
        _rigidbody.angularVelocity = new Vector3(Random.Range(1, 2.3f), Random.Range(1, 2.3f), Random.Range(1, 2.3f));
        _rigidbody.AddForce(random * 300);
        
    }
    private void SetOnGround()
    {
        throwing = true;

        transform.parent = null;
        _rigidbody.isKinematic = false;
        
        _rigidbody.useGravity = true;
        _collider.enabled = true;
        _collider.isTrigger = true;

        _rigidbody.constraints = RigidbodyConstraints.None;
    }
    private void OnTriggerEnter(Collider other)
    {
        if ((whatIsGround & (1 << other.gameObject.gameObject.layer)) != 0)//layerMask가 2진수로 저장되기 때문에 이러한 연산이 필요함..
        {
            throwing = false;
        }
        
        if (other.gameObject.TryGetComponent(out WeaponController weaponController) && throwing == false)
        {
            if (weaponController.GetCurrentGun() != null) return;
            
            weaponController.currentGun = this;
            transform.parent = weaponController.gunTrm;
            
            _rigidbody.isKinematic = true;
            _rigidbody.useGravity = false;
                
            _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            
        }
    }
}
