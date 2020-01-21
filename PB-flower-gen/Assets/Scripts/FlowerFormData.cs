using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("Mandatory Prefabs")]
    [Space(20)]
    public GameObject PetalPrefab;
    public GameObject PistilPrefab;
    public GameObject LeafPrefab;

    [Header("Path Data")]
    [Space(20)]
    public RandomPath SproutPath;
    public RandomPath StemPath;
    [Tooltip("Mandatory for PlantTypes that have Branches")]
    public RandomPath BranchPath;

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
    public Vector2Int LeafCountRange;
    [MinMax(0, 1, ShowEditRange = true)]
    public Vector2 LeafPositionRange = new Vector2(0.2f, 0.9f);
    [Tooltip("Leaf Position Random Value: H")]
    [MinMax(-1, 1, ShowEditRange = true)]
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
    public Vector3[] pathPoints;
    public RandomPoint[] randomPoints;
}
[System.Serializable]
public struct RandomPoint
{
    public int randomPoint;
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