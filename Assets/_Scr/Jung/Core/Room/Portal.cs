using System.Collections;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public MeshRenderer[] meshRenderers;
    public float dissolveTime;
    public Collider _Collider;
    private readonly int _dissolveHash = Shader.PropertyToID("_DissolveHeight");
    
    private void OnEnable()
    {
        _Collider.enabled = false;
        StartCoroutine(StartDissolve(_dissolveHash));
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
            float currentDissolve = Mathf.Lerp(-5f, 5f, currentTime/dissolveTime);

            foreach (var item in mats)
            {
                item.SetFloat(_dissolveHash, currentDissolve);
            }
            
            yield return null;
        }

        _Collider.enabled = true;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterController characterController))
        {
            characterController.enabled = false;
            characterController.gameObject.transform.localPosition = new Vector3(0,1,-2);
            characterController.enabled = true;
            
            MapManager.Instance.ClearMap();
            MapManager.Instance.ChapterGeneration();
            
        }
    }
}
