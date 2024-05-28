using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public float health;
    
    public UnityEvent onHitEvent;
    public UnityEvent onDieEvent;

    public void ApplyDamage(float damage)
    {
        health -= damage;
        if(health <= 0)
            onDieEvent?.Invoke();
    }
    
    
    
}
