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
    public float buffPercent;//현재 버프가 얼마나 찼는지
    
}
