using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerStatController : MonoSingleton<PlayerStatController>
{
    
    public PlayerStatSO PlayerStatSo;
    [Space]
    public BottleDataSO healthBottleSo;

    private float _timer;

    private Stat health;
    private void Start()
    {
        health = PlayerStatSo._statDic[StatType.Health];
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        
        if (_timer > healthBottleSo.decreaseTime)
        {
            _timer = 0;
            if (health.GetValue() >= 0)
            {
                health.RemoveValue(healthBottleSo.decreaseAmount);
            }
            else
            {
                print("체력이 0 입니다.");
            }
        }
    }
}
