using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public enum BottleType
{
    Special,
    Normal
}

[CreateAssetMenu(menuName = "SO/bottleData")]
public class BottleDataSO : ScriptableObject
{
    public string bottleName;
    [TextArea] public string bottleExplain;
    public BottleType bottleType;
    public StatType statType;
    
    public int increaseAmount;
    public int drinkAmount;
    
    public float decreaseTime;
    public int decreaseAmount;

}
