using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/goodsList")]
public class GoodsList : ScriptableObject
{
    public GameObject[] lists;
    public List<GameObject> goods;

    private void OnEnable()
    {
        goods = new List<GameObject>();
        ResetList();
    }

    public void ResetList()
    {
        goods.Clear();

        foreach (var item in lists)
        {
            goods.Add(item);
        }
    }
    
}
