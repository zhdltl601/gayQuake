using UnityEngine;

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
    
    [Header("Ammo Values")]
    public int maxAmmoInMagazine;
    public int ammoInMagazine;
    public int totalAmmo;
    
    [Header("Shot values")]
    public float damage;
    public float shotRate;
    public float bulletSpeed;
    public GameObject bullet;
    public int maxShotCount;
    public int minShotCount;
    
    [Header("Bound")]
    [Range(0,200)]public float xBound;
    [Range(0,200)]public float yBound;
    
    
}
