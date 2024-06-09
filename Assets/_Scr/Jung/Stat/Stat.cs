using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stat
{
    [SerializeField] private int _baseValue;
        
    public int GetValue()
    {
        return _baseValue;
    }

    public void AddValue(int value)
    {
        if (value != 0)
            _baseValue += value;
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
