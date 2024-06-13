using UnityEngine;
using UnityEngine.Serialization;

public class EnemyDamageCaster : MonoBehaviour
{
    [SerializeField] private Transform attackTrm;
    [SerializeField] private float attackRadius;
    
    public Enemy enemy;

    private new Collider[] _collider;
    private void Start()
    {
        _collider = new Collider[1];
    }

    public void DamageCaster()
    {
        int player = Physics.OverlapSphereNonAlloc(attackTrm.position ,attackRadius ,_collider, enemy._whatIsPlayer);
        
        if (player > 0)
        {
            PlayerStatController.Instance.PlayerStatSo._statDic[StatType.Health].RemoveValue(enemy.attackDamage);    
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(attackTrm.position , attackRadius);
    }
}
