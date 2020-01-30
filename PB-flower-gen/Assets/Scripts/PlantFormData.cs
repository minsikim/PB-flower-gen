using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SplineMesh;

[CreateAssetMenu (fileName = "Plant_NewPlantName", menuName = "Project B/Plant")]
public class PlantFormData : ScriptableObject
{
    [Header("Basic Information")]
    public string flowerName;
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
    [DrawIf("plantFormType", PlantFormType.C, ComparisonType.Equals)]
    public int FlowerToPlantCount = 0;

    public Vector2Int PetalCountRange;
    public Vector2Int PetalLayerCountRange;

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
    public Color TreeColor = Color.green;
    [DrawIf("plantFormType", PlantFormType.E, ComparisonType.Equals)]
    public Color BranchColor = Color.green;
    public Color StemColor = Color.green;
    [DrawIf("plantFormType", PlantFormType.C, ComparisonType.GreaterOrEqual)]
    public Color StemBranchColor = Color.green;
    public Color PistilColor = Color.yellow;
    public bool PetalColorRandom = false;
    [DrawIf("PetalColorRandom", true, ComparisonType.NotEqual)]
    public Color PetalColor = Color.red;
    [DrawIf("PetalColorRandom", true, ComparisonType.Equals)]
    public Color PetalColorRange1;
    [DrawIf("PetalColorRandom", true, ComparisonType.Equals)]
    public Color PetalColorRange2;


    [Header("Animation Duration")]
    [Space(20)]
    [LabelOverride("Sprouting")]
    public float SproutAnimationDuration;
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

    public GameObject LeafPrefab;

    public Vector2Int LeafCountRange;

    [DrawIf("plantFormType", PlantFormType.A, ComparisonType.NotEqual)]
    [MinMax(0, 1, ShowEditRange = true)]
    public Vector2 LeafPositionRange = new Vector2(0.2f, 0.9f);

    [DrawIf("plantFormType", PlantFormType.A, ComparisonType.Equals)]
    public float LeafFixedPosition = 0.1f;

    [DrawIf("plantFormType", PlantFormType.A, ComparisonType.NotEqual)]
    [MinMax(-1, 1, ShowEditRange = true)]
    public float LeafPositionRandomPercentage = 0.25f;

    [Header("Sprout Leaf Position Data")]
    public LeafGrowRelation leafGrowRelation;
    [DrawIf("leafGrowRelation", LeafGrowRelation.Same, ComparisonType.NotEqual)]
    public Vector2Int SproutLeafCountRange;
    [DrawIf("leafGrowRelation", LeafGrowRelation.Same, ComparisonType.NotEqual)]
    [MinMax(0, 1, ShowEditRange = true)]
    public Vector2 SproutLeafPositionRange = new Vector2(0.2f, 0.9f);
    [DrawIf("leafGrowRelation", LeafGrowRelation.Same, ComparisonType.NotEqual)]
    [MinMax(-1, 1, ShowEditRange = true)]
    public float SproutLeafPositionRandomPercentage = 0.25f;
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
}
public enum VectorAxis
{
    x,
    y,
    z
}
public enum CapType
{
    round,
    sharp,
    flat
}