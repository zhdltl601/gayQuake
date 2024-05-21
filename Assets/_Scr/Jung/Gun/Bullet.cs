using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private TrailRenderer _trailRenderer;
    
    private void OnEnable()
    {
       
        _trailRenderer.emitting = true;
    }
    private void OnDisable()
    {
        _rigidbody.velocity = Vector3.zero;
        _trailRenderer.emitting = false;
        _trailRenderer.Clear();
    }
    
}
