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
    
    public int increaseAmount;
    public int drinkAmount;
    
    public float decreaseTime;
    public int decreaseAmount;
}
