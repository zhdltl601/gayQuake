using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stat
{
    [SerializeField] private int _baseValue;
    [SerializeField] private int _maxValue;
    [SerializeField] private bool useMaxValue;
    public int GetValue()
    {
        return _baseValue;
    }
    
    public int GetMaxValue()
    {
        return _maxValue;
    }
    
    public void AddValue(int value)
    {
        if(useMaxValue && _baseValue >= _maxValue)
            return;

        if (value != 0)
        {
            _baseValue += value;
            SoundManager.Instance.PlayEnemyrSound("BottleIncrease");
        }
    }

    public void RemoveValue(int value)
    {
        if (_baseValue > 0)
            _baseValue -= value;
    }
    
    public void SetDefaultValue(int value)
    {
        _baseValue = value;
    }
}
