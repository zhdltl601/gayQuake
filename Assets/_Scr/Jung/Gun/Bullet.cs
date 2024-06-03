using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private TrailRenderer _trailRenderer;

    private int increaseAmount;

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

    public void SetBullet(int _increaseAmount,Vector3 lookAt)
    {
        increaseAmount = _increaseAmount;
        transform.forward = lookAt;
    }
    
}



