using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SplineMesh;

[CreateAssetMenu (fileName = "Plant_NewPlantName", menuName = "Project B/Plant")]
public class PlantFormData : ScriptableObject
{
    [Header("Basic Information")]
    public string plantName;
    public string description;

    [Header("Plant Type")]
    [Space(20)]
    public PlantFormType plantFormType;

    [Header("Flower Type")]
    public FlowerFormType flowerFormType;


    [Header("Flower Data")]
    public GameObject PetalPrefab;
    public GameObject PistilPrefab;
    [DrawIf("flowerFormType", FlowerFormType.BudsArePetals, ComparisonType.NotEqual)]
    public GameObject BudPrefab;
    public GameObject LeafPrefab;
    [DrawIf("plantFormType", PlantFormType.C, ComparisonType.Equals)]
    public int FlowerToPlantCount = 0;

    public PetalData PetalData;

    [Header("Path Data")]
    [Space(20)]
    public RandomPath SproutPathData;
    public RandomPath StemPathData;
    [DrawIf("plantFormType", PlantFormType.C, ComparisonType.GreaterOrEqual)]
    public RandomPath StemBranchPathData;
    [DrawIf("plantFormType", PlantFormType.E, ComparisonType.Equals)]
    public RandomPath BranchPathData;
    [DrawIf("plantFormType", PlantFormType.E, ComparisonType.Equals)]
    public RandomPath TreePathData;

    [Header("Materials")]
    [Space(20)]
    [DrawIf("plantFormType", PlantFormType.E, ComparisonType.Equals)]
    public ColorData TreeColorData;
    [DrawIf("plantFormType", PlantFormType.E, ComparisonType.Equals)]
    public ColorData BranchColorData;
    public ColorData StemColorData;
    [DrawIf("plantFormType", PlantFormType.C, ComparisonType.GreaterOrEqual)]
    public ColorData StemBranchColorData;
    public ColorData PistilColorData;
    public ColorData PetalColorData;
    public ColorData LeafColorData;


    [Header("Animation Duration")]
    [Space(20)]
    [LabelOverride("Sprouting")]
    public float SproutAnimationDuration;
    public AnimationCurve SproutAnimationCurve;
    [LabelOverride("Growing")]
    public float GrowAniamtionDuration;
    [LabelOverride("Blooming")]
    public float BloomAnimationDuration;
    [LabelOverride("Falling")]
    public float FallAnimationDuration;
    [LabelOverride("Reblooming")]
    public float RebloomAnimationDuration;


    [Header("Leaf Position Data")]
    [Space(20)]

    public LeafGrowRelation leafGrowRelation;
    public GameObject SproutParticles;

    public LeafData SproutLeafData;
    public LeafData GrownLeafData;

}
[System.Serializable]
public struct ColorData
{
    public bool isRandom;
    public Color Color;
    public Color ColorRange1;
    public Color ColorRange2;
}

[System.Serializable]
public struct LeafData
{
    public Vector2Int CountRange;
    public bool isFixed;

    public float FixedPosition;

    public Vector2 PositionRange;
    public float PositionRandomPercentage;

    public float BaseScale;
    public float ScaleRandomValue;
}
[System.Serializable]
public struct MinMaxRange
{
    public float min;
    public float max;
}
[System.Serializable]
public struct RandomPath
{
    public PathMeshProperties pathMeshProperties;
    public Node[] nodes;
    public RandomNode[] randomNode;
}
[System.Serializable]
public struct RandomNode
{
    public int randomNodeIndex;
    public VectorAxis randomAxis;
    public MinMaxRange randomRange;
}
[System.Serializable]
public struct PathMeshProperties
{
    public MinMaxRange Thickness;
    public bool ThickerAtStart;
    [Space]
    public CapType capType;
}
[System.Serializable]
public struct Node
{
    public Vector3 position;
    public Vector3 handleOut;
    public Node(Vector3 position, Vector3 handleOut)
    {
        this.position = position;
        this.handleOut = handleOut;
    }
}
[System.Serializable]
public struct PetalData
{
    public Vector2Int PetalCountRange;
    public Vector2Int PetalLayerCountRange;
    public float MinClosedTime;
    public float MaxClosedTime;
    public float MinOpenTime;
    public float MaxOpenTime;
    public float RandomTimeValue;
    public float FallPercentage;
}