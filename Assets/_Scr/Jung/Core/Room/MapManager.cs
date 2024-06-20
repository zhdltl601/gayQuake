using System.Collections.Generic;
using System.Resources;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class MapManager : MonoSingleton<MapManager>
{
    public int chapterCount;
    public int mapCount;
    
    public List<GameObject> normalMapTypes;
    public List<GameObject> specialMapTypes;
    
    [Space]
    
    public GameObject startRoom;
    public GameObject endRoom;
    
    public GameObject corridor;

    public GameObject potal;
    
    private Transform _lastRoomTrm;
    private Vector3[] _mapDir;
    private List<Transform> _maps;

    private bool specialRoomSpawn;
    
    protected override void Awake()
    {
        base.Awake();
        
        ChapterGeneration();
    }
    
    public void ChapterGeneration()
    {
        DefaultSetting();

        GenerationStartRoom();
        GenerateMap();
        GenerationEndRoom();

        GenerateCorridor();
        
        specialRoomSpawn = false;
        _maps[0].GetComponent<Room>().EnterRoom();
    }
    public void ClearMap()
    {
        for (int i = 0; i < _maps.Count; i++)
        {
            Destroy(_maps[i].gameObject);
        }
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
        
        
        GameObject newPortal = Instantiate(potal);
        newPortal.transform.parent = room.transform;
        newPortal.transform.localPosition = Vector3.zero;
        
        room.name = "EndRoom";
    }
  
  
    private void GenerateMap()
    {
        int generatedMaps = 0;
        while (generatedMaps < mapCount)
        {
            int mapType = Random.Range(0, 2);
            bool mapGenerated = false;
            
            if (mapType == 0 && specialMapTypes.Count > 0 && generatedMaps >= mapCount / 3 && specialRoomSpawn == false)
            {
                mapGenerated = GenerationSpecial();
                specialRoomSpawn = true;
            }
            else 
            {
                mapGenerated = GenerationNormal();
            }
            
            if (mapGenerated)
            {
                generatedMaps++;
            }
        }
    }
    
    private bool Generation(Vector3 position, List<GameObject> mapTypes)
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
            

            return true;
        }
        return false; 
    }

    private bool GenerationSpecial()
    {
        Vector3 position = _lastRoomTrm.position + _mapDir[Random.Range(0, _mapDir.Length)] * 60;
        return Generation(position, specialMapTypes);
    }

    private bool GenerationNormal()
    {
        Vector3 position = _lastRoomTrm.position + _mapDir[Random.Range(0, _mapDir.Length)] * 60;
        return Generation(position, normalMapTypes);
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

            Room cRoom = currentRoom.GetComponent<Room>();
            Room nRoom = nextRoom.GetComponent<Room>();
            
            if (Mathf.Approximately(direction.x, 1))
            {
                cRoom.EastWall.SetActive(false);
                nRoom.WestWall.SetActive(false);
                
                cRoom.EastWall_Door.SetActive(true);
                nRoom.WestWall_Door.SetActive(true);
            }
            else if (Mathf.Approximately(direction.x, -1))
            {
                cRoom.WestWall.SetActive(false);
                nRoom.EastWall.SetActive(false);
                
                cRoom.WestWall_Door.SetActive(true);
                nRoom.EastWall_Door.SetActive(true);
            }
            else if (Mathf.Approximately(direction.z, 1))
            {
                cRoom.NorthWall.SetActive(false);
                nRoom.SouthWall.SetActive(false);

                cRoom.NorthWall_Door.SetActive(true);
                nRoom.SouthWall_Door.SetActive(true);
            }
            else if (Mathf.Approximately(direction.z, -1))
            {
                cRoom.SouthWall.SetActive(false);
                nRoom.NorthWall.SetActive(false);
                
                cRoom.SouthWall_Door.SetActive(true);
                nRoom.NorthWall_Door.SetActive(true);
            }
        }
    }
    #endregion
}
