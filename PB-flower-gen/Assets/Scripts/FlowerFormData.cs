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
    public PlantFormType plantFormType;
    public FlowerFormType flowerFormType;

    [Header("Mandatory Prefabs")]
    public GameObject PetalPrefab;
    public GameObject PistilPrefab;
    public GameObject LeafPrefab;

    [Header("Path Data")]
    public RandomPath SproutPath;
    public RandomPath StemPath;
    [Tooltip("Mandatory for PlantTypes that have Branches")]
    public RandomPath BranchPath;

    [Header("Animation Duration")]
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

    [Space]
    [Header("Animation Duration")]
    [MinMax(0, 1, ShowEditRange = true)]
    public Vector2 AnimationRange = new Vector2(0, 1);

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