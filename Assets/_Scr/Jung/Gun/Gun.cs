using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public abstract class Gun : MonoBehaviour
{
    public GunDataSO gunData;
    [FormerlySerializedAs("gunInfo")] public GunMagazine gunMagazine;
    
    
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

    public WeaponController _weaponController;


    private void Awake()
    {
        gunMagazine.maxAmmoInMagazine = gunData.GetMaxAmmoInMagazine();
        gunMagazine.ammoInMagazine = gunMagazine.maxAmmoInMagazine;
        gunMagazine.totalAmmo = gunData.GetTotalAmmo();

    }

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
        gunMagazine.ammoInMagazine--;
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
            
            UIManager.Instance.ChangeCrosshair();
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
       
        SoundManager.Instance.PlayPlayerSound("Shot");
        
        return bullet;
    }

    protected void ApplyDamage(Health enemyHealth, Vector3 normal, Vector3 point)
    {
        float damage = (_weaponController.GetCurrenBottle()._bottleDataSo.statType is StatType.Attack)
            ? PlayerStatController.Instance.PlayerStatSo._statDic[StatType.Attack].GetValue() + gunData.damage
            : gunData.damage;

        enemyHealth.ApplyDamage(damage, normal, point);
    }

    public virtual void ReLoad()
    {
        int needAmmo = gunMagazine.maxAmmoInMagazine - gunMagazine.ammoInMagazine;

        if (gunMagazine.totalAmmo - needAmmo < 0)
        {
            gunMagazine.ammoInMagazine += gunMagazine.totalAmmo;
            gunMagazine.totalAmmo = 0;
        }
        else
        {
            gunMagazine.totalAmmo -= needAmmo;
            gunMagazine.ammoInMagazine = gunMagazine.maxAmmoInMagazine;
        }
    }
    public virtual void ThrowGun()
    {
        SettingThrow();
                
        Vector3 random = (gameObject.transform.forward);
        _rigidbody.angularVelocity = new Vector3(Random.Range(1, 2.3f), Random.Range(1, 2.3f), Random.Range(1, 2.3f));
        _rigidbody.AddForce(random * 300);
        
    }
    private void SettingThrow()
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
        if ((whatIsGround & (1 << other.gameObject.layer)) != 0)//layerMask가 2진수로 저장되기 때문에 이러한 연산이 필요함..
        {
            throwing = false;
        }
        
        if (other.gameObject.TryGetComponent(out WeaponController weaponController) && throwing == false)
        {
            if (weaponController.GetCurrentGun() != null) return;

            _weaponController = weaponController;
            
            _weaponController.currentGun = this;
            transform.parent = _weaponController.gunTrm;
            
            _rigidbody.isKinematic = true;
            _rigidbody.useGravity = false;
            
            _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            
            UIManager.Instance.SetAmmoText();
        }
    }
}
