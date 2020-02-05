using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetalLocalData : MonoBehaviour
{
    #region Properties

    public int PetalLayer;
    public int PetalIndex;

    public float Rotation;

    public Color PetalColor;

    public float StartTime;
    public float EndTime;

    //delete after LocalData refactoring
    public float FallTime;

    public bool isOnFlower = true;
    public bool waitForDisable = false;

    #endregion

    //DELETE
    void OnBecameInvisible()
    {
        waitForDisable = true;
        Debug.Log(name + " is disabled." + isActiveAndEnabled);
    }
    void OnBecameVisible()
    {
        
    }
}
