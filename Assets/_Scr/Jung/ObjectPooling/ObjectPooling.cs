using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoSingleton<ObjectPooling>
{
   public static ObjectPooling instanc;

   private Dictionary<GameObject, Queue<GameObject>> _dictionary = new Dictionary<GameObject, Queue<GameObject>>();
   
   [SerializeField] private int poolSize;
   
   [Header("Initialize")] 
   [SerializeField] private GameObject bullet;
   
   

   private void Start()
   {
      InitializeNewPool(bullet);
   }

   public GameObject GetObject(GameObject prefab)
   {
      if (_dictionary.ContainsKey(prefab) == false)
      {
         InitializeNewPool(prefab);
      }

      if (_dictionary[prefab].Count == 0)
      {
         CreateNewObject(prefab);
      }

      GameObject objectToGet = _dictionary[prefab].Dequeue();
      objectToGet.SetActive(true);
      transform.parent = null;
      return objectToGet;
   }

   public void ReTurnObject(GameObject gameObject,float delay = 0.01f) => StartCoroutine(DelayReturn(delay,gameObject));
   
   private IEnumerator DelayReturn(float delay , GameObject objectToReturn)
   {
      yield return new WaitForSeconds(delay);
      ReturnToPool(objectToReturn);
   }
   
   private void ReturnToPool(GameObject prefab)
   {
      GameObject newPrefab = prefab.GetComponent<PooledObject>().originalPrefab;
      
      prefab.SetActive(false);
      prefab.transform.parent = transform;
      
      _dictionary[newPrefab].Enqueue(prefab);
   }
   
   private void InitializeNewPool(GameObject prefab)
   {
      _dictionary[prefab] = new Queue<GameObject>();  
      
      for (int i = 0; i < poolSize; i++)
      {
         CreateNewObject(prefab);
      }
   }

   private void CreateNewObject(GameObject prefab)
   {
      GameObject newObject = Instantiate(prefab, transform);
      
      newObject.AddComponent<PooledObject>().originalPrefab = prefab;
      newObject.SetActive(false);
      
      _dictionary[prefab].Enqueue(newObject);
   }
}
