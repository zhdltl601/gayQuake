using UnityEngine;
using UnityEngine.Serialization;

public class EnemyDamageCaster : MonoBehaviour
{
    [SerializeField] private Transform attackTrm;
    [SerializeField] private float attackRadius;
    
    public Enemy enemy;
    
    private Collider[] _playerCount;
    private void Start()
    {
        _playerCount = new Collider[1];
    }

    public void DamageCaster()
    {
        int player = Physics.OverlapSphereNonAlloc(attackTrm.position ,attackRadius ,_playerCount, enemy._whatIsPlayer);
        
        if (player > 0)
        {
            PlayerStatController.Instance.PlayerStatSo._statDic[StatType.Health].RemoveValue(enemy.attackDamage);    
            UIManager.Instance.BloodScreen(Color.red);
        }
        
        SoundManager.Instance.PlayEnemyrSound(enemy.dieSound);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(attackTrm.position , attackRadius);
    }
}
