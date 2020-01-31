using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafLocalData : MonoBehaviour
{

    #region Private Variables

    public int leafIndex;
    public int totalLeafCount;
    public float sproutPosition;
    public float finalPosition;
    public float rotation;
    public float sproutScale;
    public Color leafColor;
    public GameObject parent;

    public void AssignMaterialColor(Color color)
    {
        transform.GetChild(0).GetComponent<MeshRenderer>().material.color = color;
    }

    #endregion
}
