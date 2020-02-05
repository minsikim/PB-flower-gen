using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerLocalData : MonoBehaviour
{
    #region Properties

    public int FlowerIndex = 0;
    public int TotalPetalCount;
    public int PetalLayerCount;
    public float PetalFallPercentage = 0.2f;
    public Color PetalColor = Color.yellow;
    public int[] PetalCounts;

    #endregion
    public FlowerLocalData() { }
    public FlowerLocalData(FlowerLocalData data)
    {
        FlowerIndex         = data.FlowerIndex;
        TotalPetalCount     = data.TotalPetalCount;
        PetalLayerCount     = data.PetalLayerCount;
        PetalFallPercentage = data.PetalFallPercentage;
        PetalColor          = data.PetalColor;
        PetalCounts         = data.PetalCounts;
    }
    public FlowerLocalData AssignValues(FlowerLocalData data)
    {
        FlowerIndex = data.FlowerIndex;
        TotalPetalCount = data.TotalPetalCount;
        PetalLayerCount = data.PetalLayerCount;
        PetalFallPercentage = data.PetalFallPercentage;
        PetalColor = data.PetalColor;
        PetalCounts = data.PetalCounts;

        return this;
    }
}
