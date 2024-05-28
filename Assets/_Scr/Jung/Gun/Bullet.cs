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
        
        PlayerStatController.Instance.PlayerStatSo._statDic[statType].AddModifier(increaseAmount);
    }
}



