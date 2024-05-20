using UnityEngine;
public class PlayerCamera : MonoBehaviour
{
    public Transform cameraPos;
    public Transform cameraRot;
    public void SetCameraRotation(float x, float y, float z = 0)
    {
        cameraRot.rotation = Quaternion.Euler(x, y, z);
    }
    public void SetCameraPosition(float x, float y, float z)
    {
        cameraPos.localPosition = new Vector3(x, y, z);
    }
    public Transform GetCameraRotTransform()
    {
        return cameraRot;
    }
    public Transform GetCameraPosTransform()
    {
        return cameraPos;
    }
}
