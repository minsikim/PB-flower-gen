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

    public GameObject parent;

    public void AssignMaterialColor(Color color)
    {
        transform.GetChild(0).GetComponent<MeshRenderer>().material.color = color;
    }

    #endregion
}
