using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public abstract class Gun : MonoBehaviour
{
    public GunDataSO gunData;
    [SerializeField] private LayerMask whatIsEnemy;
    [SerializeField] protected float maxRebound;
    [SerializeField] protected float minRebound;
    
    [SerializeField] private GameObject caseShell;

    [SerializeField] private List<ParticleSystem> muzzles;
    
    [HideInInspector] public Transform _firePos;
    
    protected Transform _caseShellPos;
    protected Rigidbody _rigidbody;
    protected BoxCollider _collider;
    
    private Transform playerCam;
    private bool throwing;
    
    protected virtual void Start()
    {
        playerCam = Camera.main.transform;
        
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
            enemyHealth.ApplyDamage(PlayerStatController.Instance.PlayerStatSo._statDic[StatType.Attack].GetValue());
            enemyHealth.onHitEvent?.Invoke();
        }
        
        bullet[0].transform.position = _firePos.position;
        bullet[0].GetComponent<Rigidbody>().AddForce(direction.normalized * gunData.bulletSpeed);
        
                
        //탄피
        GameObject newCaseShell = ObjectPooling.Instance.GetObject(caseShell);
        
        newCaseShell.transform.position = _caseShellPos.position;
        newCaseShell.GetComponent<Rigidbody>().AddForce((Vector3.right + Vector3.up) * Random.Range(100, 140));
        
        ObjectPooling.Instance.ReTurnObject(newCaseShell , 2);
        
        
        //muzzle Effect
        foreach (var item in muzzles)
        {
            item.Simulate(0);
            item.Play();
        }
        
        return bullet;
    }

    public virtual void ReLoad()
    {
        int tempAmmo = gunData.maxAmmoInMagazine - gunData.ammoInMagazine;
        gunData.totalAmmo -= tempAmmo;
        gunData.ammoInMagazine = gunData.maxAmmoInMagazine;
    }

    public virtual void ThrowGun()
    {
        transform.parent = null;
        _rigidbody.isKinematic = false;
        _rigidbody.useGravity = true;
        
        _rigidbody.constraints = RigidbodyConstraints.None;
        
        _rigidbody.AddForce(gameObject.transform.forward * 300);

        throwing = true;
        
        _collider.enabled = true;
        _collider.isTrigger = true;
    }
    
    protected IEnumerator ReboundCoroutine()
    {
        float halfShotRate = gunData.shotRate / 2f;
        float elapsedTime = 0f;
        Quaternion targetRotation = Quaternion.Euler(
            Random.Range(minRebound, maxRebound),
            transform.rotation.eulerAngles.y,
            transform.rotation.eulerAngles.z
        );
        
        while (elapsedTime <halfShotRate)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / gunData.shotRate;
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, EaseInSine(t));
            yield return null;
        }

        elapsedTime = 0f;
        while (elapsedTime < halfShotRate)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / gunData.shotRate;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0,0,0) , EaseInSine(t));
            yield return null;
        }
        
        //transform.rotation = Quaternion.Euler(0,0,0);
    }
    
    private float EaseInSine(float x) {
        return 1 - Mathf.Cos((x * Mathf.PI) / 2);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Ground")
        {
            throwing = false;
        }
        
        if (other.gameObject.TryGetComponent(out WeaponController weaponController) && throwing == false)
        {
            weaponController.currentGun = this;
            transform.parent = weaponController.gunTrm;
            
            _rigidbody.isKinematic = true;
            _rigidbody.useGravity = false;
        
            _rigidbody.constraints = RigidbodyConstraints.FreezeAll;

            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.Euler(0,0,0);
        }
    }
}
