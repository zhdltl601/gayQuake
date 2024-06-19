using System.Numerics;
using UnityEngine;
using UnityEngine.Events;
using Vector3 = UnityEngine.Vector3;

public class Health : MonoBehaviour
{
    public float health;
    
    public UnityEvent onHitEvent;
    public UnityEvent onDieEvent;

    [HideInInspector] public ActionData _actionData;
    
    public void ApplyDamage(float damage , Vector3 normal , Vector3 pos)
    {
        health -= damage;
        _actionData.hitNormal = normal;
        _actionData.hitPoint = pos;
        
        onHitEvent?.Invoke();

        if (health <= 0)
        {
            onDieEvent?.Invoke();
        }
        
     

    }
    
}
