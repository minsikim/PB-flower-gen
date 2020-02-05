using System;
using System.Collections.Generic;
using UnityEngine;

public class PlantLocalData : MonoBehaviour
{
    #region Properties

    #region Basic
    public DateTime InitialTime;
    public string PlantName { get; set; }
    public string Description { get; set; }
    public float Rotation { get; set; }
    public PlantFormType PlantFormType { get; set; }
    public FlowerFormType FlowerFormType { get; set; }
    #endregion

    #region Prefabs
    public GameObject PetalPrefab;
    public GameObject PistilPrefab;
    public GameObject BudPrefab;
    public GameObject LeafPrefab;
    #endregion

    #region Animation Information
    public FlowerAnimationState CurrentAnimationState;
    public DateTime LastStateChangedTime;
    public TimeSpan AnimationCycleTime;
    public List<float> AnimationDurationList;
    #endregion

    #region Children Information
    StemLocalData MainStem;
    FlowerLocalData Flower;
    List<PetalLocalData> Petals;
    List<LeafLocalData> Leaves;
    #endregion

    #endregion

    #region Constructor

    PlantLocalData(PlantFormData data)
    {
        InitialTime = DateTime.Now;

        PlantName = data.plantName;
        Description = data.description;

        Rotation = UnityEngine.Random.Range(-150f, -30f);

        PlantFormType = data.plantFormType;
        FlowerFormType = data.flowerFormType;

        MainStem = DistributeMainStemLocalData(data.SproutPathData, data.StemPathData, data.StemColor);
        Flower = DistributeFlowerLocalData(data.PetalLayerCountRange, data.PetalCountRange);
        Petals = new List<PetalLocalData>();
        Leaves = new List<LeafLocalData>();
        for (int i = 0; i < Flower.TotalPetalCount; i++)
        {

        }
    }

    #endregion

    #region Distribution Functions

    #region Stems
    private StemLocalData DistributeMainStemLocalData(RandomPath sproutRandomPath, RandomPath stemRandomPath, Color color, bool isBranch = false)
    {
        StemLocalData data = new StemLocalData();

        data.isBranch       = isBranch;
        
        data.SproutNodes    = DistributePath(sproutRandomPath);
        data.Nodes          = DistributePath(stemRandomPath);
        data.Color          = color;

        return data;
    }

    private Node[] DistributePath(RandomPath RandomizablePathData)
    {
        Node[] distributedNodeArray = (Node[])RandomizablePathData.nodes.Clone();

        foreach (RandomNode rn in RandomizablePathData.randomNode)
        {
            float random = UnityEngine.Random.Range(rn.randomRange.min, rn.randomRange.max);
            distributedNodeArray[rn.randomNodeIndex].position[(int)rn.randomAxis] += random;
            distributedNodeArray[rn.randomNodeIndex].handleOut[(int)rn.randomAxis] += random;
        }

        return distributedNodeArray;
    }
    #endregion

    #region Flowers

    private FlowerLocalData DistributeFlowerLocalData(Vector2Int petalLayerCountRange, Vector2Int petalCountRange)
    {
        FlowerLocalData data = new FlowerLocalData();

        int tempPetalLayerCount = UnityEngine.Random.Range(petalLayerCountRange.x, petalLayerCountRange.y + 1);
        data.TotalPetalCount = tempPetalLayerCount;
        data.PetalCounts = DistributeRandomIntArray(tempPetalLayerCount, petalCountRange);

        return data;
    }

    private List<PetalLocalData> DistributePetalLocalData(FlowerLocalData flower)
    {

        return new List<PetalLocalData>();
    }

    #endregion

    #region Util

    private int[] DistributeRandomIntArray(int howMany, Vector2Int range)
    {
        int[] counts = new int[2];

        for (int i = 0; i < howMany; i++)
        {
            counts[i] = UnityEngine.Random.Range(range.x, range.y + 1);
        }
        return counts;
    }

    #endregion

    #endregion
}
