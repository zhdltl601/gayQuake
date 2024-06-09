using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.TestTools;
using Random = UnityEngine.Random;

[System.Serializable]
public struct EnemySettings
{
  public GameObject enemy;
  public int enemyCount;
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

    public List<GameObject> aliveEnemyNames = new List<GameObject>();
    private bool _isInPlayer;
    
    private void Start()
    {
        corridor = GetComponentInChildren<Corridor>();
    }
    private void Update()
    {
        if (aliveEnemyNames.Count <= 0 && _isInPlayer)
        {
            FinishRoom();
        }  
    }
    public void EnterRoom()
    {
        _isInPlayer = true;
                    
        if (type == RoomType.Special) return;
        GenerationEnemy();
    }
    private void FinishRoom()
    {
        _isInPlayer = false;
        corridor?.DoorOpen();
    }
    private void GenerationEnemy()
    {
        foreach (var item in _enemySettingsArray)
        {
            for (int i = 0; i < item.enemyCount; i++)
            {
                GameObject newEnemy = Instantiate(item.enemy,transform.position + new Vector3(Random.Range(1,10) , 0 , Random.Range(1,10)) , Quaternion.identity);
                newEnemy.GetComponent<EnemyMapSetting>().SetRoom(this);
                aliveEnemyNames.Add(newEnemy);
            }
        }
    }
}
