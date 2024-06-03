using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class MapManager : MonoSingleton<MapManager>
{
    public int mapCount;
    public List<GameObject> mapTypes;
    public GameObject portal;
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
        
        GenerateMap();
        GeneratePortal();
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

    private void GenerateMap()
    {
        //startRoom
        GameObject startRoom = Instantiate(mapTypes[Random.Range(0, mapTypes.Count)], _lastRoomTrm.position, Quaternion.identity);
        _lastRoomTrm = startRoom.transform;
        _maps.Add(startRoom.transform);
        
        //randomRoom
        for (int i = 0; i < mapCount;)
        {
            Vector3 position = _lastRoomTrm.position + _mapDir[Random.Range(0, _mapDir.Length)] * 60;

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
                GameObject newMapObj = Instantiate(mapTypes[Random.Range(0, mapTypes.Count)], position, Quaternion.identity);
                _lastRoomTrm = newMapObj.transform;
                _maps.Add(newMapObj.transform);

                i++;
            }
        }
    }
    
    private void GeneratePortal()
    {
        for (int i = 0; i < _maps.Count - 1; i++)
        {
            Vector3 startPos = _maps[i].position;
            Vector3 endPos = _maps[i + 1].position;
            Vector3 midPoint = (startPos + endPos) / 2;
            
            Vector3 direction = (endPos - startPos).normalized;
            Vector3 adjustedMidPoint = midPoint - (direction * 30);
                     
            GameObject newPortal = Instantiate(portal, adjustedMidPoint, Quaternion.identity);
            
            newPortal.transform.forward = -direction;
            newPortal.transform.parent = _maps[i];
            
            newPortal.GetComponent<Portal>().SetPos(startPos , endPos);
            newPortal.GetComponent<Portal>().SetRoom(_maps[i + 1].GetComponent<Room>());
            
            GameObject newCorridor = Instantiate(corridor , midPoint , quaternion.identity);
            newCorridor.transform.right = direction;
            
        }
    }
        
}
