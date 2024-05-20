using System.Collections;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    public GunDataSO gunData;
    [SerializeField] protected float maxRebound;
    [SerializeField] protected float minRebound;
    
    protected Transform _firePos;
    
    protected virtual void Start()
    {
        _firePos = GetComponentInChildren<GunModel>().GetFirePos();
    }
    
    public virtual void Shoot()
    {
        gunData.ammoInMagazine--;
        GameObject bullet = Instantiate(gunData.bullet,_firePos.position,Quaternion.identity);
        bullet.GetComponent<Rigidbody>().AddForce(_firePos.right * gunData.bulletSpeed);
        
        StartCoroutine(ReboundCoroutine());
    }

    public virtual void ReLoad()
    {
        int tempAmmo = gunData.maxAmmoInMagazine - gunData.ammoInMagazine;
        gunData.totalAmmo -= tempAmmo;
        gunData.ammoInMagazine = gunData.maxAmmoInMagazine;
    }

    public virtual void Aim()
    {
        
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
}
