using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stat
{
    [SerializeField] private int _baseValue;
    
    
    public List<int> modifiers;
    
    
    public int GetValue()
    {
        int finalValue = _baseValue;
        foreach (int value in modifiers)
            finalValue += value;

        return finalValue;
    }

    public void AddModifier(int value)
    {
        if (value != 0)
            modifiers.Add(value);
    }

    public void RemoveModifier(int value)
    {
        if (value != 0)
            modifiers.Remove(value);
    }

    public void Remove()
    {
        if(modifiers.Count > 0)
             modifiers.RemoveAt(modifiers.Count - 1);
    }

    public void SetDefaultValue(int value)
    {
        _baseValue = value;
    }
}
