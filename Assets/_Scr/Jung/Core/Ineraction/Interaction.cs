using System;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    public Action onInteraction;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            onInteraction?.Invoke();
        }
    }
    
    
}
