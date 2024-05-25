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
