using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

[CreateAssetMenu(menuName = "SO/Stat")]
public class PlayerStatSO : ScriptableObject
{
    public Stat health;
    public Stat money;
    public Stat attack;
    public Stat speed;
    public Stat ammo;
    
    public Dictionary<StatType, Stat> _statDic; 
    
    private void OnEnable()
    {
        _statDic = new Dictionary<StatType, Stat>
        {
            { StatType.Health, health },
            { StatType.Money, money },
            { StatType.Attack, attack },
            { StatType.Speed, speed },
            { StatType.Ammo  , ammo}
        };
        
        health.SetDefaultValue(800);
        money.SetDefaultValue(200);
        attack.SetDefaultValue(0);
        speed.SetDefaultValue(0);
        ammo.SetDefaultValue(0);
    }
}
