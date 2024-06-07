using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class MapManager : MonoSingleton<MapManager>
{
    public int mapCount;
    
    public List<GameObject> normalMapTypes;
    public List<GameObject> specialMapTypes;
    
    [Space]
    
    public GameObject startRoom;
    public GameObject endRoom;
    
    public GameObject corridor;
    
    private Transform _lastRoomTrm;
    private Vector3[] _mapDir;
    private List<Transform> _maps;
    
    [Space]
    [Header("LayerMask")] 
    [SerializeField] private LayerMask whatIsMap;
    
    protected override void Awake()
    {
        base.Awake();
        
        DefaultSetting();
        
        GenerationStartRoom();
        GenerateMap();
        GenerationEndRoom();
        
        GenerateCorridor();
    }

    private void Start()
    {
        _maps[0].GetComponent<Room>().EnterRoom();
    }
    
    private void DefaultSetting()
    {
        _mapDir = new Vector3[3];
        _maps = new List<Transform>();

        _mapDir[0] = transform.forward;
        _mapDir[1] = transform.right;
        _mapDir[2] = -transform.right;
        
        _lastRoomTrm = transform;
    }
    
    #region GenerationMethos
     private void GenerationStartRoom()
  {
        GameObject room = Instantiate(startRoom, _lastRoomTrm.position,
            Quaternion.identity);
        _lastRoomTrm = room.transform;
        _maps.Add(room.transform);

        room.name = "StartRoom";
  }
  
  private void GenerationEndRoom()
  {
      bool positionOccupied = false;
      Vector3 position = Vector3.zero;
      
      while (!positionOccupied)
      {
          position = _lastRoomTrm.position + _mapDir[Random.Range(0, _mapDir.Length)] * 60;
      
          foreach (Transform map in _maps)
          {
              if (map.position != position)
              {
                  positionOccupied = true;
              }
          }
      }
      
      GameObject room = Instantiate(endRoom, position, Quaternion.identity);
      _lastRoomTrm = room.transform;
      _maps.Add(room.transform);
      
      room.name = "EndRoom";
  }
  
  
    private void GenerateMap()
    {
        for (int i = 0; i < mapCount;)
        {
            int mapType = Random.Range(0,2);

            if (mapType == 0 && specialMapTypes.Count > 0 && i > mapCount/2)
            {
                GenerationSpecial();
            }
            else 
            {
                GenerationNormal();
            }
            
            i++;
        }
        
    }
    private void Generation(Vector3 position, List<GameObject> mapTypes, bool removeFromList = false)
    {
        bool positionOccupied = false;
        foreach (Transform map in _maps)
        {
            if (map.position == position)
            {
                positionOccupied = true;
                break;
            }
        }

        if (!positionOccupied)
        {
            int randomIndex = Random.Range(0, mapTypes.Count);
            GameObject newMapObj = Instantiate(mapTypes[randomIndex], position, Quaternion.identity);
            
            _lastRoomTrm = newMapObj.transform;
            _maps.Add(newMapObj.transform);
        
            if (removeFromList)
            {
                mapTypes.RemoveAt(randomIndex);
            }
        }
        
    }
    private void GenerationSpecial()
    {
        Vector3 position = _lastRoomTrm.position + _mapDir[Random.Range(0, _mapDir.Length)] * 60;
        Generation(position, specialMapTypes, true);
    }
    private void GenerationNormal()
    {
        Vector3 position = _lastRoomTrm.position + _mapDir[Random.Range(0, _mapDir.Length)] * 60;
        Generation(position, normalMapTypes);
    }
    private void GenerateCorridor()
    {
        for (int i = 0; i < _maps.Count - 1; i++)
        {
            Transform currentRoom = _maps[i];
            Transform nextRoom = _maps[i + 1];

            Vector3 startPos = currentRoom.position;
            Vector3 endPos = nextRoom.position;
            Vector3 midPoint = (startPos + endPos) / 2;
            
            Vector3 direction = (endPos - startPos).normalized;
                             
            GameObject newCorridor = Instantiate(corridor , midPoint , quaternion.identity);

            newCorridor.transform.right = direction;
            newCorridor.transform.parent = _maps[i];
            
            newCorridor.GetComponent<Corridor>().SetRoom(nextRoom.GetComponent<Room>());
            
            if (Mathf.Approximately(direction.x, 1))
            {
                // nextRoom이 currentRoom의 오른쪽에 있을 때
                currentRoom.Find("Wall/EastWall").gameObject.SetActive(false);
                nextRoom.Find("Wall/WestWall").gameObject.SetActive(false);
                
                currentRoom.Find("Wall/EastWall_Door").gameObject.SetActive(true);
                nextRoom.Find("Wall/WestWall_Door").gameObject.SetActive(true);
            }
            else if (Mathf.Approximately(direction.x, -1))
            {
                // nextRoom이 currentRoom의 왼쪽에 있을 때
                currentRoom.Find("Wall/WestWall").gameObject.SetActive(false);
                nextRoom.Find("Wall/EastWall").gameObject.SetActive(false);
                
                currentRoom.Find("Wall/WestWall_Door").gameObject.SetActive(true);
                nextRoom.Find("Wall/EastWall_Door").gameObject.SetActive(true);
            }
            else if (Mathf.Approximately(direction.z, 1))
            {
                // nextRoom이 currentRoom의 위쪽에 있을 때
                currentRoom.Find("Wall/NorthWall").gameObject.SetActive(false);
                nextRoom.Find("Wall/SouthWall").gameObject.SetActive(false);

                currentRoom.Find("Wall/NorthWall_Door").gameObject.SetActive(true);
                nextRoom.Find("Wall/SouthWall_Door").gameObject.SetActive(true);
            }
            else if (Mathf.Approximately(direction.z, -1))
            {
                // nextRoom이 currentRoom의 아래쪽에 있을 때
                currentRoom.Find("Wall/SouthWall").gameObject.SetActive(false);
                nextRoom.Find("Wall/NorthWall").gameObject.SetActive(false);
                
                currentRoom.Find("Wall/SouthWall_Door").gameObject.SetActive(true);
                nextRoom.Find("Wall/NorthWall_Door").gameObject.SetActive(true);
            }
        }
    }
    #endregion
    
        
}
