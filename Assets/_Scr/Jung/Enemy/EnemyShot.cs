using UnityEngine;

public class EnemyShot : MonoBehaviour
{
    [SerializeField] private Transform firePos;
    [SerializeField] private GameObject bullet;
    
    public Enemy _enemy;

    public void Shot()
    {
        bool isHit = Physics.Raycast(firePos.position  , firePos.forward ,out RaycastHit hit,_enemy.attackDistance ,_enemy._whatIsPlayer);

        GameObject newBullet = ObjectPooling.Instance.GetObject(bullet);
        newBullet.transform.position = firePos.position;
        
        Vector3 dir;
        if (isHit)
        {
            PlayerStatController.Instance.PlayerStatSo._statDic[StatType.Health].RemoveValue(_enemy.attackDamage);
            dir = (hit.point - firePos.position);
        }
        else
        {
            dir = (firePos.forward);
        }
        dir.Normalize();
        
        newBullet.GetComponent<Rigidbody>().AddForce(dir * 1200);
        newBullet.GetComponent<Bullet>().SetBullet(firePos.forward);
        
        ObjectPooling.Instance.ReTurnObject(newBullet , 1.5f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(firePos.position , firePos.forward * _enemy.attackDistance);
    }
}
