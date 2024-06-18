using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerViewmodel : MonoBehaviour
{
    public Transform viewmodelPivot;
    public Transform viewmodelPivotHint;

    [Header("AL")]
    [SerializeField] private Transform al_wallrun;
#if false
    public void SetViewmodelHintPosition(float x, float y, float z)
    {
        viewmodelPivotHint.localPosition = new Vector3(x, y, z);
    }
    public void SetViewmodelHintRotation(float x, float y, float z = 0)
    {
        viewmodelPivotHint.localRotation = Quaternion.Euler(x, y, z);
    }
#endif
    public void SetViewmodelPosition(float x, float y, float z)
    {
        viewmodelPivot.localPosition = new Vector3(x, y, z);
    }
    public void SetViewmodelRotation(float x, float y, float z = 0)
    {
        viewmodelPivot.localRotation = Quaternion.Euler(x, y, z);
    }
    private void SetWallrunViewmodeAngle(float z)
    {
        al_wallrun.localRotation = Quaternion.Euler(0, 0, z);
    }
    public void WallRun(float z)
    {
        SetWallrunViewmodeAngle(z);
    }
}
