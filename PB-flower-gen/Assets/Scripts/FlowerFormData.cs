using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SplineMesh;

[CreateAssetMenu (fileName = "Flower_NewFlowerName", menuName = "Project B/Flower")]
public class FlowerFormData : ScriptableObject
{
    [Header("Basic Information")]
    public string flowerName;
    public string description;

    [Header("Form Type")]
    [Space(20)]
    public PlantFormType plantFormType;
    public FlowerFormType flowerFormType;

    [Header("Flower Data")]
    public GameObject PetalPrefab;
    public GameObject PistilPrefab;
    [DrawIf("flowerFormType", FlowerFormType.BudsArePetals, ComparisonType.NotEqual)]
    public GameObject BudPrefab;
    [DrawIf("plantFormType", PlantFormType.C, ComparisonType.Equals)]
    public int FlowerToPlantCount = 3;
    public Vector2Int PetalCountRange;
    public Vector2Int PetalLayerCount;

    [Header("Path Data")]
    [Space(20)]
    public RandomPath SproutPathData;
    public RandomPath StemPathData;
    [DrawIf("plantFormType", PlantFormType.C, ComparisonType.Equals)]
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
    [DrawIf("plantFormType", PlantFormType.A, ComparisonType.NotEqual, order = 0)]
    [Tooltip("Leaf Position Random Value: H", order = 1)]
    [MinMax(-1, 1, ShowEditRange = true, order = 2)]
    public Vector2 LeafPositionRandomValue = new Vector2(-.25f, 0.25f);

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