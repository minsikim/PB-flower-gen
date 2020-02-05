using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetalLocalData : MonoBehaviour
{
    #region Properties

    public int PetalLayer;
    public int PetalIndex;

    public float Rotation;

    public Color Color;

    public float StartTime;
    public float EndTime;
    //Delete after Local Data Refactoring
    public float FallTime = 5f;

    public bool isOnFlower = true;
    public bool waitForDisable = false;

    #endregion
    public PetalLocalData() { }
    public PetalLocalData(PetalLocalData data)
    {
        PetalLayer      = data.PetalLayer;
        PetalIndex      = data.PetalIndex;
        Rotation        = data.Rotation;
        Color           = data.Color;
        StartTime       = data.StartTime;
        EndTime         = data.EndTime;
        FallTime        = data.FallTime;
        isOnFlower      = data.isOnFlower;
        waitForDisable  = data.waitForDisable;
    }
    public PetalLocalData AssignValues(PetalLocalData data)
    {
        PetalLayer = data.PetalLayer;
        PetalIndex = data.PetalIndex;
        Rotation = data.Rotation;
        Color = data.Color;
        StartTime = data.StartTime;
        EndTime = data.EndTime;
        FallTime = data.FallTime;
        isOnFlower = data.isOnFlower;
        waitForDisable = data.waitForDisable;

        return this;
    }
}
