using System;
using UnityEngine;

public class GunModel : MonoBehaviour
{
    [SerializeField] private Transform firePos;
    [SerializeField] private Transform caseShell;

    public Transform GetFirePos()
    {
        return firePos;
    }

    public Transform GetCaseShell()
    {
        return caseShell;
    }
    
}
