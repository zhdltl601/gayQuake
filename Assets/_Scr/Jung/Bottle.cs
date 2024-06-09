using System;
using UnityEngine;

public abstract class Bottle : MonoBehaviour
{
    public BottleDataSO _bottleDataSo;

    private Animator _animator;
    
    private float _timer;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        _timer += Time.deltaTime;
    }

    public virtual void DrinkBottle()
    {
        _animator.SetTrigger("Drink");
        print(_animator);
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

}
