using System.Collections;
using UnityEngine;
public class PlayerCamera : MonoBehaviour
{
    public Transform cameraRot;
    public Transform cameraPos;
    public void SetCameraRotation(float x, float y, float z = 0)
    {
        cameraRot.localRotation = Quaternion.Euler(x, y, z);
    }
    public void SetCameraPosition(float x, float y, float z)
    {
        cameraPos.localPosition = new Vector3(x, y, z);
    }
    public Transform GetCameraRotTransform()
    {
        //프로퍼티로 바꾸기 
        return cameraRot;
    }
    public Transform GetCameraPosTransform()
    {
        return cameraPos;
    }
    public void CameraShakeRot(Vector3 rot)
    {

    }
    public void CameraShakePos(float duration = 0, float power= 0.01f)
    {
        StartCoroutine(CameraRecoilPos(duration, power));
    }
    private IEnumerator CameraRecoilPos(float duration, float power)
    {
        float timer = 0;
        Vector3 originalPos = cameraPos.localPosition;
        while (duration >= timer)
        {
            timer += Time.deltaTime;
            Vector3 randomVector = Random.onUnitSphere * power;
            cameraPos.localPosition = randomVector;
            yield return null;
        }
        cameraPos.localPosition = originalPos;
    }
}
