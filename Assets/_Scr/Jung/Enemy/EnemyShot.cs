using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class EnemyShot : MonoBehaviour
{
    [SerializeField] private Transform firePos;
    [SerializeField] private GameObject bullet;

    public Enemy _enemy;
    [SerializeField] private float duration = 0.2f;
    [SerializeField] private float targetIntensity = 0.5f;
    [SerializeField] private float screenTime = 0;
    public void Shot()
    {
        bool isHit = Physics.Raycast(firePos.position, firePos.forward, out RaycastHit hit, _enemy.attackDistance, _enemy._whatIsPlayer | _enemy.whatIsObstacle);

        GameObject newBullet = ObjectPooling.Instance.GetObject(bullet);
        newBullet.transform.position = firePos.position;

        Vector3 dir;
        if (isHit)
        {
            PlayerStatController.Instance.PlayerStatSo._statDic[StatType.Health].RemoveValue(_enemy.attackDamage);
            UIManager.Instance.BloodScreen(Color.red, duration, targetIntensity, screenTime);

            dir = (hit.point - firePos.position);
        }
        else
        {
            dir = (firePos.forward);
        }
        dir.Normalize();

        newBullet.GetComponent<Rigidbody>().AddForce(dir * 1200);
        newBullet.GetComponent<Bullet>().SetBullet(firePos.forward);

        ObjectPooling.Instance.ReTurnObject(newBullet, 1.5f);

        SoundManager.Instance.PlayEnemyrSound("Enemy_Shot");
    }
}
