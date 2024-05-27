using UnityEngine;

public class GunModel : MonoBehaviour
{
    [SerializeField] private Transform firePos;

    public Transform GetFirePos()
    {
        return firePos;
    }
    
}
