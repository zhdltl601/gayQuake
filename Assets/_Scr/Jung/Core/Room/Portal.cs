using UnityEngine;

public class Portal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterController characterController))
        {
            characterController.enabled = false;
            characterController.gameObject.transform.localPosition = Vector3.zero;
            characterController.enabled = true;
            
            MapManager.Instance.ClearMap();
            MapManager.Instance.ChapterGeneration();
            
            
        }
    }
}
