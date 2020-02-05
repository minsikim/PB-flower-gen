using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerLocalData : MonoBehaviour
{
    #region Properties

    public int FlowerIndex = 0;
    public int TotalPetalCount;
    public int PetalLayerCount;
    public int[] PetalCounts
    {
        get
        {
            return PetalCounts;
        }
        set
        {
            TotalPetalCount = 0;
            foreach(int c in value)
            {
                TotalPetalCount += c;
            }
            PetalCounts = value;
        }
    }

    public GameObject parent; // Parent Must be Stem.

    #endregion


}
