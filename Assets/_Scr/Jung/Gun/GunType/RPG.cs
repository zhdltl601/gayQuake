using System;
using System.Numerics;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class RPG : Gun
{
    public float sphereRadius;

    public GameObject boomEffect;
    private Vector3 point;
    private Vector3 normal;
    public override GameObject[] Shoot()
    {
        //총알
        gunMagazine.ammoInMagazine--;
        int shotCount = Random.Range(gunData.minShotCount, gunData.maxShotCount);

        GameObject[] bullet = new GameObject[shotCount];
        Vector3 playerCamFoward = playerCam.forward;

        bool isHit = Physics.Raycast(playerCam.position, playerCam.forward, out RaycastHit hit, Mathf.Infinity, whatIsEnemy);
        if (isHit)
        {
            point = hit.point;
            normal = hit.normal;

            Collider[] cols = Physics.OverlapSphere(hit.point, sphereRadius, whatIsEnemy);

            //bullet
            GameObject newBullet = ObjectPooling.Instance.GetObject(gunData.bullet);
            newBullet.transform.position = _firePos.position;
            newBullet.transform.forward = playerCamFoward;
            ObjectPooling.Instance.ReTurnObject(newBullet, 2);

            newBullet.GetComponent<Rigidbody>().AddForce(playerCamFoward.normalized * gunData.bulletSpeed);

            Invoke("BoomEffect", 0.4f);

            foreach (var item in cols)
            {
                if (item != null && item.TryGetComponent(out Health health))
                {
                    Vector3 closestPoint = item.ClosestPoint(point);
                    Vector3 normal = (point - closestPoint).normalized;

                    ApplyDamage(health, -normal, closestPoint);
                }
            }
        }

        //muzzle Effect
        foreach (var item in muzzles)
        {
            item.Simulate(0);
            item.Play();
        }

        SoundManager.Instance.PlayPlayerSound("Shot_Rocket");

        return bullet;
    }

    private void BoomEffect()
    {
        GameObject newBoomEffect = ObjectPooling.Instance.GetObject(boomEffect);

        newBoomEffect.transform.position = point;
        newBoomEffect.transform.forward = normal;
        newBoomEffect.GetComponent<EffectPlayer>().OnlyParticlePlay();

        ObjectPooling.Instance.ReTurnObject(newBoomEffect, 2);
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawRay();
    }
}
