using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public struct EnemySettings
{
  public GameObject enemy;
  public int enemyCount;
  public Transform[] enemyPos;
}

public enum RoomType
{
    Normal,
    Special
}

public class Room : MonoBehaviour
{
     public RoomType type;
    [SerializeField] private Corridor corridor;
    [SerializeField] private EnemySettings[] _enemySettingsArray;
    public List<GameObject> aliveEnemys = new List<GameObject>();

    public bool customMap;
    
    private bool _isInPlayer;

    [SerializeField] private Transform generationTrm;

    private Portal portal;
    
    [Header("Wall")]
    public GameObject EastWall;
    public GameObject WestWall;
    public GameObject NorthWall;
    public GameObject SouthWall;
    
    [Header("Door")]
    public GameObject EastWall_Door;
    public GameObject WestWall_Door;
    public GameObject NorthWall_Door;
    public GameObject SouthWall_Door;
    private void Start()
    {
        if (gameObject.name == "EndRoom")
        {
            portal = GetComponentInChildren<Portal>();
            portal.gameObject.SetActive(false);
        }
        else
        {
            corridor = GetComponentInChildren<Corridor>();
        }
    }

    private void Update()
    {
        if (aliveEnemys.Count <= 0 && _isInPlayer)
        {
            FinishRoom();
        }  
    }
    public void EnterRoom()
    {
        _isInPlayer = true;
        
        if (type == RoomType.Special){ return;}
        GenerationEnemy();
    }
    private void FinishRoom()
    {
        _isInPlayer = false;

        if (portal !=null && MapManager.Instance.chapterCount >= 1)
        {
            MapManager.Instance.chapterCount--;
            portal.gameObject.SetActive(true);
        }
        
        corridor?.DoorOpen();
        
    }
    private void GenerationEnemy()
    {
        if (customMap)
        {
            foreach (var item in _enemySettingsArray)
            {
                for (int i = 0; i < item.enemyCount; i++)
                {
                    GameObject newEnemy = Instantiate(item.enemy, item.enemyPos[i].position, Quaternion.identity);
                    
                    newEnemy.GetComponent<EnemyMapSetting>().SetRoom(this);
                    aliveEnemys.Add(newEnemy);
                }
            }
        }
        else
        {
            foreach (var item in _enemySettingsArray)
            {
                for (int i = 0; i < item.enemyCount; i++)
                {
                    GameObject newEnemy = Instantiate(item.enemy,transform.position + new Vector3(Random.Range(-15,15) , 0 , Random.Range(-15,15)) , Quaternion.identity);
                    newEnemy.GetComponent<EnemyMapSetting>().SetRoom(this);
                    aliveEnemys.Add(newEnemy);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if(type == RoomType.Special)return;
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(generationTrm.position , new Vector3(30 ,3 , 30));
    }
}
