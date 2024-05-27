using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Stat")]
public class PlayerStatSO : ScriptableObject
{
    public Stat health;
    public Stat money;
    public Stat attack;
    public Stat speed;
    
    public Dictionary<StatType, Stat> _statDic; 
    
    private void OnEnable()
    {
        _statDic = new Dictionary<StatType, Stat>
        {
            { StatType.Health, health },
            { StatType.Money, money },
            { StatType.Attack, attack },
            { StatType.Speed, speed }
        };
    }
}
