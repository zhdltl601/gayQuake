using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private TrailRenderer _trailRenderer;

    private GameObject originalPrefab;
    private void Start()
    {
        originalPrefab = GetComponent<PooledObject>().originalPrefab;
    }

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

    public void SetBullet(Vector3 lookAt)
    {
        transform.forward = lookAt;
    }

    private void OnTriggerEnter(Collider other)
    {
        ObjectPooling.Instance.ReTurnObject(gameObject);
    }
}



