using System.Collections;
using UnityEngine;

public class Pistol : Gun
{
    private Transform _firePos;

    [SerializeField] private float maxRebound;
    [SerializeField] private float minRebound;
        
    private void Start()
    {
        _firePos = GetComponentInChildren<GunModel>().GetFirePos();
    }

    public override void Shoot()
    {
        gunData.ammoInMagazine--;
        GameObject bullet = Instantiate(gunData.bullet,_firePos.position,Quaternion.identity);
        bullet.GetComponent<Rigidbody>().AddForce(_firePos.right * gunData.bulletSpeed);
        
        //rebound
        StartCoroutine(ReboundCoroutine());


    }

    public override void ReLoad()
    {
        int tempAmmo = gunData.maxAmmoInMagazine - gunData.ammoInMagazine;
        gunData.totalAmmo -= tempAmmo;
        gunData.ammoInMagazine = gunData.maxAmmoInMagazine;
    }

    public override void Aim()
    {
        
    }

    private IEnumerator ReboundCoroutine()
    {
        float elapsedTime = 0f;
        Quaternion targetRotation = Quaternion.Euler(
            Random.Range(minRebound, maxRebound),
            transform.rotation.eulerAngles.y,
            transform.rotation.eulerAngles.z
        );

        // 첫 번째 단계: 초기 회전값에서 목표 회전값으로
        while (elapsedTime < gunData.shotRate)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / gunData.shotRate;
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, t);
            yield return null;
        }

        // 두 번째 단계: 목표 회전값에서 원래의 회전값으로
        elapsedTime = 0f; // 초기화
        while (elapsedTime < gunData.shotRate)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / gunData.shotRate;
            transform.rotation = Quaternion.Lerp(targetRotation, Quaternion.Euler(0,0,0) , t);
            yield return null;
        }

        // 마지막으로 원래의 회전값으로 설정하여 정확히 0으로 맞춥니다.
        transform.rotation = Quaternion.Euler(0,0,0);
    }

}
