using UnityEngine;

public class EnemyDamageCaster : MonoBehaviour
{
    [SerializeField] private Transform attackTrm;
    [SerializeField] private float attackRadius;
    
    public Enemy _enemy;

    private Collider[] collider;
    private void Start()
    {
        collider = new Collider[1];
    }

    public void DamageCaster()
    {
       
        int player = Physics.OverlapSphereNonAlloc(attackTrm.position ,attackRadius ,collider, _enemy._whatIsPlayer);
        
        if (player > 0)
        {
            PlayerStatController.Instance.PlayerStatSo._statDic[StatType.Health].RemoveValue(_enemy.attackDamage);    
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(attackTrm.position , attackRadius);
    }
}
