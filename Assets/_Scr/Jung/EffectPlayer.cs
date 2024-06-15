using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class EffectPlayer : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> _particleSystem;

    private Health _enemy;

    private void Start() {
        _enemy = GetComponent<Health>();
    }


    public void PlayParticle()
    {
        ActionData _actionData = _enemy._actionData;
        
        foreach (var item in _particleSystem)
        {
            item.gameObject.transform.position = _actionData.hitPoint;
            item.transform.rotation = Quaternion.LookRotation(_actionData.hitNormal);
            
            item.Simulate(0);
            item.Play();
        }
    }

}