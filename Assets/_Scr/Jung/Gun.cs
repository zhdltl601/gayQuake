using System.Collections;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    public GunDataSO gunData;
    [SerializeField] protected float maxRebound;
    [SerializeField] protected float minRebound;
    
    protected Transform _firePos;
    protected Rigidbody _rigidbody;
    protected BoxCollider _collider;
    
    
    private bool throwing;
    
    protected virtual void Start()
    {
        _firePos = GetComponentInChildren<GunModel>().GetFirePos();
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<BoxCollider>();
    }
    
    public virtual GameObject[] Shoot()
    {
        gunData.ammoInMagazine--;

        GameObject[] bullet = new GameObject[1];
        
        bullet[0] = ObjectPooling.Instance.GetObject(gunData.bullet);
        
        bullet[0].transform.position = _firePos.position;
        bullet[0].GetComponent<Rigidbody>().AddForce(_firePos.right * gunData.bulletSpeed);
        
        ObjectPooling.Instance.ReTurnObject(bullet[0] , 2);
        
        //StartCoroutine(ReboundCoroutine());

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
        
        _rigidbody.AddForce(gameObject.transform.forward * 100);

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
