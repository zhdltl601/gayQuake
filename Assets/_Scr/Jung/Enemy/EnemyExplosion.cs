using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class EnemyExplosion : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> explosionEffects;
    [SerializeField] private float explosionRadius;
    
    private Collider[] _collider;
    private Enemy _enemy;

    private void Start()
    {
        _enemy = GetComponentInParent<Enemy>();
        _collider = new Collider[1];
    }

    public void Explosion()
    {
        foreach (var item in explosionEffects)
        {
            item.Simulate(0);
            item.Play();
        }
                    
        int player = Physics.OverlapSphereNonAlloc(transform.position ,explosionRadius ,_collider, _enemy._whatIsPlayer);
        if (player > 0)
        {
            PlayerStatController.Instance.PlayerStatSo._statDic[StatType.Health].RemoveValue(_enemy.attackDamage);
            UIManager.Instance.BloodScreen(Color.red);
        }

        SoundManager.Instance.PlayEnemyrSound("Explosion");
        
        _enemy.DieEvent();
    }

    private void DestroyGameObject()
    {
        Destroy(transform.parent.gameObject);
    }
    
    public void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position , explosionRadius);
    }
}
