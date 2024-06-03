using System;
using System.Collections;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private MeshRenderer[] meshRenderers;
    [SerializeField] private GameObject surface;
    private readonly int dissolveValue = Shader.PropertyToID("_Value");
    
    private Vector3 _startPos;
    private Vector3 _nextPos;
    private bool isFinishing = false;
    
    
    [SerializeField] private CharacterController player;
    

    public void Dissolve()
    {
        StartCoroutine(StartDissolve(dissolveValue));
    }   
    private IEnumerator StartDissolve(int _dissolveHash)
    {
        Material[] mats = new Material[meshRenderers.Length];

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            mats[i] = meshRenderers[i].material;
        }

        float currentTime = 0;
        while (currentTime <= 1f)
        {
            currentTime += Time.deltaTime;
            float currentDissolve = Mathf.Lerp(1, 0f, currentTime);

            for (int i = 0; i < mats.Length; i++)
            {
                mats[i].SetFloat(_dissolveHash, currentDissolve);
            }

            yield return null;
        }
        
        surface.SetActive(true);
        isFinishing = true;
    }
    public void SetPos(Vector3 startPos , Vector3 nextPos)
    {
        _startPos = startPos;
        _nextPos = nextPos;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(isFinishing == false)return;
        
        if (other.GetComponent<Player>())
        {
            player = other.GetComponent<CharacterController>();
            
            player.enabled = false;
            other.transform.localPosition = _nextPos;
            
            Invoke("PlayerMoveOn" , 0.1f);
        }
    }
    private void PlayerMoveOn()
    {
        if (player == null) return;

        player.enabled = true;
    }

}