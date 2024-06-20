using System;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public enum GunType
{
    Pistol,
    ShotGun,
    Rifle,
    Revolver,
    Rpg,
}

[CreateAssetMenu(menuName = "SO/gunData")]
public class GunDataSO : ScriptableObject
{
    [Header("Default Values")]
    public GunType gunType;
    public string gunName;
    public float reloadTime;
    
    [Header("Ammo Values")]
    [SerializeField] private int maxAmmoInMagazine;
    [SerializeField] private int totalAmmo;
    
    [Header("Shot values")]
    public float damage;
    public float shotRate;
    public float bulletSpeed;
    public GameObject bullet;
    public int maxShotCount;
    public int minShotCount;
    
    [Header("Bound")]
    [Range(0,200)]public float xBound;
    [Range(0,400)]public float yBound;

    public Sprite crossHair;

    public int GetMaxAmmoInMagazine()
    {
        return maxAmmoInMagazine;
    }

    public int GetTotalAmmo()
    {
        return totalAmmo;
    }
    
}
