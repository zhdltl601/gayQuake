using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerViewmodel : MonoBehaviour
{
    public Transform viewmodelPivot;
    public Transform viewmodelPivotHint;
    public void SetViewmodelHintPosition(float x, float y, float z)
    {
        viewmodelPivotHint.localPosition = new Vector3(x, y, z);
    }
    public void SetViewmodelHintRotation(float x, float y, float z = 0)
    {
        viewmodelPivotHint.localRotation = Quaternion.Euler(x, y, z);
    }
    public void SetViewmodelPosition(float x, float y, float z)
    {
        viewmodelPivot.localPosition = new Vector3(x, y, z);
    }
    public void SetViewmodelRotation(float x, float y, float z = 0)
    {
        viewmodelPivot.localRotation = Quaternion.Euler(x, y, z);
    }
}
