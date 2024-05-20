using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    public GunDataSO gunData;
    
    public abstract void Shoot();
    public abstract void ReLoad();
    public abstract void Aim();
}
