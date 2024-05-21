using UnityEngine;

public abstract class Bottle : MonoBehaviour
{
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            DrinkBottle();
        }
        ApplyBuff();
    }

    protected abstract void ApplyBuff();
    protected abstract void DrinkBottle();

    
}
