using System;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private Portal portal;

  
    private void Start()
    {
        portal = GetComponentInChildren<Portal>();
        
        if(portal != null)
             portal.gameObject.SetActive(false);
    }

    public void EnterRoom()
    {
        
    }
    
    [ContextMenu("Test")]
    public void FinishRoom()
    {
        portal.gameObject.SetActive(true);
        portal.Dissolve();
    }
    
    public void ExitRoom()
    {
        Destroy(gameObject);
    }

    public void GenerationEnemy()
    {
        
    }
    
}
