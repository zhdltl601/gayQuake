using System;
using UnityEngine;

public abstract class Bottle : MonoBehaviour
{
    public BottleDataSO _bottleDataSo;
    public RuntimeAnimatorController AnimatorController;
    private float _timer;
    
    private void Update()
    {
        _timer += Time.deltaTime;
    }

    public virtual void DrinkBottle(Animator _animator)
    {
        _animator.Play("Drink", -1, 0f);
        
        PlayerStatController.Instance.PlayerStatSo._statDic[_bottleDataSo.statType].AddValue(_bottleDataSo.drinkAmount);
    }

    public void AnimationEnd()
    {
        Destroy(gameObject);
    }

    public virtual void DecreaseBottle()
    {
        if (_timer > _bottleDataSo.decreaseTime)
        {
            _timer = 0;
            
            PlayerStatController.Instance.PlayerStatSo._statDic[_bottleDataSo.statType].RemoveValue(_bottleDataSo.decreaseAmount);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out WeaponController weaponController))
        {
            transform.parent = weaponController.bottleTrm;
            weaponController.bottleList.Add(this);
        }
    }
    
    public void SetBottleParent(Transform trm)
    {
        transform.parent = trm;
    }
}
