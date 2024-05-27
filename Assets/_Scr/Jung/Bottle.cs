using System;
using UnityEngine;

public abstract class Bottle : MonoBehaviour
{
    public BottleDataSO _bottleDataSo;

    private float _timer;
    
    private void Update()
    {
        _timer += Time.deltaTime;
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            DrinkBottle();
        }
        
    }

    protected abstract void DrinkBottle();

    public virtual void DecreaseBottle()
    {
        if (_timer > _bottleDataSo.decreaseTime)
        {
            _timer = 0;
            
            PlayerStatController.Instance.PlayerStatSo._statDic[_bottleDataSo.statType].Remove();
        }
    }

}
