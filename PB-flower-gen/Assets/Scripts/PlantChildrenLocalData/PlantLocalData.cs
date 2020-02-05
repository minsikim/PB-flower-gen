using System;
using System.Collections.Generic;
using UnityEngine;
using SplineMesh;

public class PlantLocalData
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
    public GameObject SproutParticles;
    #endregion

    #region Animation Information
    public FlowerAnimationState CurrentAnimationState;
    public DateTime LastStateChangedTime;
    public List<float> AnimationDurationList;
    public float AnimationCycleTime;
    #endregion

    #region Children Information
    public StemLocalData MainStem;
    public FlowerLocalData Flower;
    public List<PetalLocalData> Petals;
    public List<LeafLocalData> Leaves;
    #endregion

    #endregion

    #region Constructor

    public PlantLocalData() { }
    public PlantLocalData(PlantFormData data)
    {
        InitialTime     = DateTime.Now;

        PlantName       = data.plantName;
        Description     = data.description;

        Rotation        = Util.RandomRange(-150f, -30f);

        PlantFormType   = data.plantFormType;
        FlowerFormType  = data.flowerFormType;

        PetalPrefab     = data.PetalPrefab;
        PistilPrefab    = data.PistilPrefab;
        BudPrefab       = data.BudPrefab;
        LeafPrefab      = data.LeafPrefab;
        SproutParticles = data.SproutParticles;

        CurrentAnimationState   = FlowerAnimationState.Sprout;
        LastStateChangedTime    = InitialTime;
        AnimationDurationList   = new List<float>()
        {
            data.SproutAnimationDuration,
            data.GrowAniamtionDuration,
            data.BloomAnimationDuration,
            data.FallAnimationDuration,
            data.RebloomAnimationDuration
        };
        AnimationCycleTime = data.BloomAnimationDuration + data.FallAnimationDuration + data.RebloomAnimationDuration;

        MainStem    = DistributeMainStemLocalData(data.SproutPathData, data.StemPathData, data.StemColorData);
        Flower      = DistributeFlowerLocalData(data.PetalData, data.PetalColorData);
        Petals      = DistributePetalLocalData(Flower, data.PetalData);
        Leaves      = DistributeLeafLocalData(data.leafGrowRelation, data.SproutLeafData, data.GrownLeafData, data.LeafColorData);

        Debug.Log("PlantLocalData Constructed");
    }

    #endregion

    #region Distribution Functions

    #region Stems
    private StemLocalData DistributeMainStemLocalData(RandomPath sproutRandomPath, RandomPath stemRandomPath, ColorData colorData, bool isBranch = false)
    {
        StemLocalData data = new StemLocalData();

        data.isBranch        = isBranch;
        data.Nodes           = DistributePath(stemRandomPath);
        data.SproutNodes     = Util.MatchNodeCount(DistributePath(sproutRandomPath), data.Nodes);
        data.SproutThickness = new Vector2(sproutRandomPath.pathMeshProperties.Thickness.min, sproutRandomPath.pathMeshProperties.Thickness.max);
        data.StemThickness   = new Vector2(stemRandomPath.pathMeshProperties.Thickness.min, stemRandomPath.pathMeshProperties.Thickness.max);
        data.Color           = !colorData.isRandom ? colorData.Color : Util.GetColorFromRange(colorData.ColorRange1, colorData.ColorRange2);

        return data;
    }

    private Node[] DistributePath(RandomPath RandomizablePathData)
    {
        Node[] distributedNodeArray = (Node[])RandomizablePathData.nodes.Clone();

        foreach (RandomNode rn in RandomizablePathData.randomNode)
        {
            float random = Util.RandomRange(rn.randomRange.min, rn.randomRange.max);
            distributedNodeArray[rn.randomNodeIndex].position[(int)rn.randomAxis] += random;
            distributedNodeArray[rn.randomNodeIndex].handleOut[(int)rn.randomAxis] += random;
        }

        return distributedNodeArray;
    }
    #endregion

    #region Flowers

    private FlowerLocalData DistributeFlowerLocalData(PetalData petalData, ColorData colorData)
    {
        FlowerLocalData data = new FlowerLocalData();

        Vector2Int petalLayerCountRange = petalData.PetalLayerCountRange;
        Vector2Int petalCountRange = petalData.PetalCountRange;

        int tempPetalLayerCount = UnityEngine.Random.Range(petalLayerCountRange.x, petalLayerCountRange.y + 1);
        data.PetalLayerCount = tempPetalLayerCount;
        data.PetalCounts = Util.DistributeRandomIntArray(tempPetalLayerCount, petalCountRange);
        data.TotalPetalCount = Util.SumArray(data.PetalCounts);
        data.PetalFallPercentage = petalData.FallPercentage;
        data.PetalColor = !colorData.isRandom ? colorData.Color : Util.GetColorFromRange(colorData.ColorRange1, colorData.ColorRange2);
        

        return data;
    }

    private List<PetalLocalData> DistributePetalLocalData(FlowerLocalData flower, PetalData petalData)
    {
        List<PetalLocalData> dataList = new List<PetalLocalData>();

        int PetalLayerCount = flower.PetalLayerCount;
        int[] PetalCounts = flower.PetalCounts;
        float PetalMinClosedTime = petalData.MinClosedTime;
        float PetalMaxClosedTime = petalData.MaxClosedTime;
        float PetalMinOpenTime = petalData.MinOpenTime;
        float PetalMaxOpenTime = petalData.MaxOpenTime;
        float PetalRandomTimeValue = petalData.RandomTimeValue;

        for (int i = 0; i < flower.PetalLayerCount; i++)
        {
            float petalLayerClosedTime = (PetalMaxClosedTime - PetalMinClosedTime) / PetalLayerCount * i + PetalMinClosedTime;
            float petalLayerOpenTime = (PetalMaxOpenTime - PetalMinOpenTime) / PetalLayerCount * i + PetalMinOpenTime;
            for (int j = 0; j < flower.PetalCounts[i]; j++)
            {
                PetalLocalData petalLocalData = new PetalLocalData();

                float RandomValue = Util.RandomRange(PetalRandomTimeValue);

                petalLocalData.Rotation = (360 / PetalCounts[i] * j) + i * 30;
                petalLocalData.PetalLayer = i;
                petalLocalData.PetalIndex = j;
                petalLocalData.StartTime = petalLayerClosedTime + RandomValue;
                petalLocalData.EndTime = petalLayerOpenTime + RandomValue;
                petalLocalData.Color = flower.PetalColor;

                dataList.Add(petalLocalData);
            }
        }
        return dataList;
    }

    #endregion

    #region Leaves

    private List<LeafLocalData> DistributeLeafLocalData(LeafGrowRelation leafGrowRelation, LeafData SproutLeafData, LeafData GrownLeafData, ColorData colorData)
    {
        List<LeafLocalData> dataList = new List<LeafLocalData>();

        int TotalLeafCount = Util.RandomRange(GrownLeafData.CountRange);
        int TotalSproutLeafCount = Util.RandomRange(SproutLeafData.CountRange);

        for (int i = 0; i < TotalLeafCount; i++)
        {
            LeafLocalData data = new LeafLocalData();

            data.LeafIndex = i;
            data.TotalLeafCount = TotalLeafCount;

            //TODO use DistributeLeafRotations
            data.Rotation = Util.RandomRange(180f);

            data.FinalPosition = DistributeLeafPosition(TotalLeafCount, i, GrownLeafData);
            data.SproutPosition = leafGrowRelation == LeafGrowRelation.Same ? data.FinalPosition : DistributeLeafPosition(TotalLeafCount, i, SproutLeafData);

            data.FinalScale = GrownLeafData.BaseScale + Util.RandomRange(GrownLeafData.ScaleRandomValue);
            data.SproutScale = SproutLeafData.BaseScale + Util.RandomRange(SproutLeafData.ScaleRandomValue);

            data.leafColor = !colorData.isRandom ? colorData.Color : Util.GetColorFromRange(colorData.ColorRange1, colorData.ColorRange2);

            data.isSprout = i < TotalSproutLeafCount ? true : false;

            dataList.Add(data);
        }
        return dataList;
    }

    private float DistributeLeafPosition(int leafCount, int i, Vector2 positionRange, float positionRandomPercentage)
    {
        float range = positionRange.y - positionRange.x;
        float interval = range / leafCount;

        float random = Util.RandomRange(positionRandomPercentage);;

        return positionRange.x + (interval * i) + (interval * random);
    }
    private float DistributeLeafPosition(int leafCount, int i, LeafData leafData)
    {
        if (leafData.isFixed) return leafData.FixedPosition;
        else
        {
            Vector2 positionRange = leafData.PositionRange;
            float positionRandomPercentage = leafData.PositionRandomPercentage;
            float range = positionRange.y - positionRange.x;
            float interval = range / leafCount;
            float random = Util.RandomRange(positionRandomPercentage);

            return positionRange.x + (interval * i) + (interval * random);
        }

    }
    //TODO Make This to distribute 1 and put it in DistributeLeafLocalData
    private List<float> DistributeLeafRotations(int leafCount)
    {
        List<float> rotations = new List<float>();

        for (int i = 0; i < leafCount; i++)
        {
            int section = i % 3;
            rotations.Add(UnityEngine.Random.Range(section * 120, (section + 1) * 120));
        }

        return rotations;
    }

    #endregion

    #endregion
}
