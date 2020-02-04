using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetalLocalData : MonoBehaviour
{
    #region Public Variables

    public float Rotation;
    public int PetalLayer;
    public int PetalIndex;
    public float StartTime;
    public float EndTime;
    public float FallTime;
    public bool isOnFlower = true;
    public bool waitForDisable = false;
    public GameObject parent;

    #endregion


    void OnBecameInvisible()
    {
        waitForDisable = true;
        Debug.Log(name + " is disabled." + isActiveAndEnabled);
    }
    void OnBecameVisible()
    {
        
    }
}
