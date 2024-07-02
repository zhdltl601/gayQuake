using System.Numerics;
using UnityEngine;
using UnityEngine.Events;
using Vector3 = UnityEngine.Vector3;

public class Health : MonoBehaviour
{
    public float health;
    
    public UnityEvent onHitEvent;
    public UnityEvent onDieEvent;

    [HideInInspector] public ActionData ActionData;
    
    public void ApplyDamage(float damage , Vector3 normal , Vector3 pos)
    {
        health -= damage;
        ActionData.hitNormal = normal;
        ActionData.hitPoint = pos;
        
        onHitEvent?.Invoke();

        if (health <= 0)
        {
            UIManager.Instance.CoinText();
            onDieEvent?.Invoke();
        }
    }
}
