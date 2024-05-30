using UnityEngine;
using UnityEngine.UI;


public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public float maxHealth;
    public float health;
    
    void Start()
    {
        maxHealth = PlayerStatController.Instance.PlayerStatSo._statDic[StatType.Health].GetValue();
        health = maxHealth;
    }
    
    void Update()
    {
        health = PlayerStatController.Instance.PlayerStatSo._statDic[StatType.Health].GetValue();
        
        healthSlider.value = health * 100 / maxHealth;
        
        
    }
    
    
    
    
}
