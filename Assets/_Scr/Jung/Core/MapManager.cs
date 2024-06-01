using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class MapManager : MonoSingleton<MapManager>
{
    public int mapCount;
    public List<GameObject> mapTypes;
    public GameObject corridor;

    private Transform _lastRoomTrm;
    private Vector3[] _mapDir;
    private List<Vector3> _maps;

    private void Start()
    {
        DefaultSetting();
        
        GenerateMap();
        GeneraCorridor();
    }

    private void DefaultSetting()
    {
        _mapDir = new Vector3[3];
        _maps = new List<Vector3>();

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
        _maps.Add(startRoom.transform.position);
        
        //randomRoom
        for (int i = 0; i < mapCount;)
        {
            Vector3 position = _lastRoomTrm.position + _mapDir[Random.Range(0, _mapDir.Length)] * 60;

            if (!_maps.Contains(position))
            {
                GameObject newMapObj = Instantiate(mapTypes[Random.Range(0, mapTypes.Count)], position, Quaternion.identity);
                _lastRoomTrm = newMapObj.transform;
                _maps.Add(newMapObj.transform.position);
                
                i++;
            }
        }
    }

    private void GeneraCorridor()
    {
        for (int i = 0; i < _maps.Count - 1; i++)
        {
            Vector3 startPos = _maps[i];
            Vector3 endPos = _maps[i + 1];
            
            Vector3 midPoint = (startPos + endPos) / 2;
            Vector3 direction = (endPos - startPos).normalized;
            
            GameObject newCorridor = Instantiate(corridor, midPoint, Quaternion.identity);
            newCorridor.transform.right = direction;
        }
    }
}