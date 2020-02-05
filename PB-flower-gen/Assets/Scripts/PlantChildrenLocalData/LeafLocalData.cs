using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafLocalData : MonoBehaviour
{

    #region Private Variables

    public int StemIndex = 0;
    public int LeafIndex;
    public int TotalLeafCount;

    public bool isSprout;

    public float SproutPosition;
    public float FinalPosition;
    public float Rotation;

    public float SproutScale;
    public float FinalScale;

    public Color leafColor;

    #endregion
    public LeafLocalData() { }
    public LeafLocalData(LeafLocalData data)
    {
        StemIndex       = data.StemIndex;
        LeafIndex       = data.LeafIndex;
        TotalLeafCount  = data.TotalLeafCount;
        isSprout        = data.isSprout;
        SproutPosition  = data.SproutPosition;
        FinalPosition   = data.FinalPosition;
        Rotation        = data.Rotation;
        SproutScale     = data.SproutScale;
        FinalScale      = data.FinalScale;
        leafColor       = data.leafColor;
    }
    public LeafLocalData AssignValues(LeafLocalData data)
    {
        StemIndex = data.StemIndex;
        LeafIndex = data.LeafIndex;
        TotalLeafCount = data.TotalLeafCount;
        isSprout = data.isSprout;
        SproutPosition = data.SproutPosition;
        FinalPosition = data.FinalPosition;
        Rotation = data.Rotation;
        SproutScale = data.SproutScale;
        FinalScale = data.FinalScale;
        leafColor = data.leafColor;

        return this;
    }
    public void AssignMaterialColor(Color color)
    {
        transform.GetChild(0).GetComponent<MeshRenderer>().material.color = color;
    }
}
