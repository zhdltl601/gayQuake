using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Room : MonoBehaviour
{
    [SerializeField] private Portal portal;
    [SerializeField] private GameObject enemy;


    private bool isInPlayer;
    public List<GameObject> enemys = new List<GameObject>();
    private void Start()
    {
        portal = GetComponentInChildren<Portal>();

        if (portal != null)
        {
            portal.gameObject.SetActive(false);
        }

    }

    public void EnterRoom()
    {
        GenerationEnemy();
        isInPlayer = true;
    }
    
    private void FinishRoom()
    {
        portal.gameObject.SetActive(true);
        isInPlayer = false;
    }
    
    private void Update()
    {
        if (enemys.Count <= 0 && isInPlayer)
        {
            FinishRoom();
        }  
        
    }
    

    private void GenerationEnemy()
    {
        for (int i = 0; i < 10; i++ )
        {
            GameObject newEnemy = Instantiate(enemy,transform.position + new Vector3(Random.Range(1,10) , 0 , Random.Range(1,10)) , Quaternion.identity);
            newEnemy.GetComponent<Enemy>().currentRoom = this;
            enemys.Add(newEnemy);
        }
    }
    
}
