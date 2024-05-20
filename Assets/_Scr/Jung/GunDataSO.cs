using UnityEngine;

public enum GunType
{
    Pistol,
    ShotGun,
    Rifle,
    
}

[CreateAssetMenu(menuName = "SO/gunData")]
public class GunDataSO : ScriptableObject
{
    public GunType gunType;
    
    public string gunName;
    public int maxAmmoInMagazine;
    public int ammoInMagazine;
    public int totalAmmo;

    public float damage;
    public float shotRate;
    
    public float bulletSpeed;
    public GameObject bullet;

    public int maxShotCount;
    public int minShotCount;
}
