using System.Collections;
using UnityEngine;

public class Corridor : MonoBehaviour
{
    private Room _nextRoom;

    public float dissolveTime;
    private readonly int _dissolveValue = Shader.PropertyToID("_Value");
    private Collider _collider;
    public MeshRenderer[] meshRenderers;
    
    private void Start()
    {
        _collider = GetComponent<Collider>();
    }

    public void SetRoom(Room _room)
    {
        _nextRoom = _room;
    }
    public void DoorOpen()
    {
        StartCoroutine(StartDissolve(_dissolveValue));
    }
    private IEnumerator StartDissolve(int _dissolveHash)
    {
        Material[] mats = new Material[meshRenderers.Length];

        for (int i =0; i < meshRenderers.Length; i++)
        {
            mats[i] = meshRenderers[i].material;
        }

        float currentTime = 0;
        while (currentTime <= dissolveTime)
        {
            currentTime += Time.deltaTime;
            float currentDissolve = Mathf.Lerp(0, 0.7f, currentTime/dissolveTime);

            foreach (var item in mats)
            {
                item.SetFloat(_dissolveHash, currentDissolve);
            }
            
            yield return null;
        }
        
        foreach (var item in meshRenderers)
        {
            Destroy(item.gameObject);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            _nextRoom.EnterRoom();
            _collider.enabled = false;
        }
        
    }
}
