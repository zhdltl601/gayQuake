using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private TrailRenderer _trailRenderer;

    private StatType statType;
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

    public void SetBullet(StatType _stat , int _increaseAmount,Vector3 lookAt)
    {
        statType = _stat;
        increaseAmount = _increaseAmount;
        transform.forward = lookAt;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        //if(other.GetComponent<Enemy>() == false)return;
        if (other.gameObject.layer != LayerMask.NameToLayer("Enemy"))
            return;

        other.GetComponent<Health>().ApplyDamage(PlayerStatController.Instance.PlayerStatSo._statDic[StatType.Attack].GetValue());
        other.GetComponent<Health>().onHitEvent?.Invoke();
        
        //나중에 적을 죽일때만 활성해 해줘야 합니다.PlayerStatController.Instance.PlayerStatSo._statDic[statType].AddModifier(increaseAmount);
        
    }
}



