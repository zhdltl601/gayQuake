using UnityEngine;

public enum BottleType
{
    Special,
    Normal
}

[CreateAssetMenu(menuName = "SO/bottleData")]
public class BottleDataSO : ScriptableObject
{
    public string bottleName;
    public BottleType bottleType;
    public StatType statType;
        
    public float decreaseTime;
    public int increaseAmount;
}
