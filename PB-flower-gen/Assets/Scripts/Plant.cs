using SplineMesh;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Plant : MonoBehaviour
{

    #region Basic Serializable Info

    [SerializeField] PlantFormData data;
    public RuntimeAnimatorController NormalizedTimeAnimationController;

    #endregion

    #region Private Fields
    //Initial Datas
    private DateTime initialTime;

    //Flower Data
    //TODO plantFormData를 다 distribute 한다음에 plantLocalData에 다 모아서 저장하자
    private PlantLocalData _d;


    private GameObject PetalPrefab;
    private GameObject PistilPrefab;
    private GameObject LeafPrefab;

    private int FlowerToPlantCount;

    private int[] PetalCounts;
    private int[] PetalLayerCounts;

    private float PetalMinClosedTime;
    private float PetalMaxClosedTime;
    private float PetalMinOpenTime;
    private float PetalMaxOpenTime;
    private float PetalRandomTimeValue;

    //Constant Mesh Generation Options
    private const float MAX_VERTEXT_DISTANCE = 1f;

    //List of Children by Type
    private List<GameObject> LeavesList;
    private List<GameObject> BranchesList;
    private List<GameObject> FlowersList;
    private List<GameObject> PetalsList;

    //Children GameObjects
    private GameObject Tree;
    private GameObject Branch;
    private GameObject Stem;
    private GameObject StemBranches;
    private GameObject Branches;
    private GameObject Leaves;
    private GameObject Flower;
    private GameObject Flowers;


    #endregion

    #region Animation Delegates

    public delegate void AnimationMethod(float progress);

    List<AnimationMethod> SproutMethods = new List<AnimationMethod>();
    List<AnimationMethod> GrowMethods = new List<AnimationMethod>();
    List<AnimationMethod> BloomMethods = new List<AnimationMethod>();
    List<AnimationMethod> FallMethods = new List<AnimationMethod>();
    List<AnimationMethod> RebloomMethods = new List<AnimationMethod>();

    public AnimationMethod Sprout;
    public AnimationMethod Grow;
    public AnimationMethod Bloom;
    public AnimationMethod Fall;
    public AnimationMethod Rebloom;

    #endregion

    #region Life Cycles
    private void Awake()
    {
        animator = GetComponent<Animator>();

        //희성: 로딩이랑 처음 만드는 거랑 구분
        if (true)
        {
            InitializeFlowerData();
        }
        else
        {
            loadFlowerData();
        }

    }

    void Start()
    {
        AddAnimationMethodsToList();
        DistributeAnimationMethodsByType();
    }

    private void Update()
    {
        
    }
    #endregion

    #region Data Managing Functions

    //InitializeFlowerData 랑 DistributeDataByType 을 통합하고 작은 단위로 쪼개야할듯
    void InitializeFlowerData()
    {
        // FlowerFormData의 DATA를 모두 적절한 방식으로 배당

        

        initialTime = DateTime.Now;

        plantName  = data.plantName;
        description = data.description;

        plantFormType   = data.plantFormType;
        flowerFormType  = data.flowerFormType;

        PetalPrefab     = data.PetalPrefab;
        PistilPrefab    = data.PistilPrefab;
        LeafPrefab      = data.LeafPrefab;

        PetalMinClosedTime = data.PetalMinClosedTime;
        PetalMaxClosedTime = data.PetalMaxClosedTime;
        PetalMinOpenTime = data.PetalMinOpenTime;
        PetalMaxOpenTime = data.PetalMaxOpenTime;
        PetalRandomTimeValue = data.PetalRandomTimeValue;

        rotation = UnityEngine.Random.Range(-180f, 0);

        SproutPathData      = data.SproutPathData;
        StemPathData        = data.StemPathData;
        StemBranchPathData  = data.StemBranchPathData;
        BranchPathData      = data.BranchPathData;
        TreePathData        = data.TreePathData;

        SproutPathNodes        = DistributePath(SproutPathData);
        StemPathNodes          = DistributePath(StemPathData);
        StemBranchPathNodes    = DistributePath(StemBranchPathData);
        BranchPathNodes        = DistributePath(BranchPathData);
        TreePathNodes          = DistributePath(TreePathData);

        LeafCountRange                  = data.LeafCountRange;
        LeafPositionRange               = data.LeafPositionRange;
        LeafPositionRandomPercentage    = data.LeafPositionRandomPercentage;
        LeafFixedPosition               = data.LeafFixedPosition;
        LeafScaleRandomValue            = data.LeafScaleRandomValue;

        LeafCount = UnityEngine.Random.Range(LeafCountRange.x, LeafCountRange.y + 1);

        leafGrowRelation                    = data.leafGrowRelation;
        SproutLeafCountRange                = data.SproutLeafCountRange;
        SproutLeafPositionRange             = data.SproutLeafPositionRange;
        SproutLeafPositionRandomPercentage  = data.SproutLeafPositionRandomPercentage;
        SproutLeafScale                     = data.SproutLeafScale;

        SproutParticles = data.SproutParticles;

        SproutAnimationDuration     = data.SproutAnimationDuration;
        GrowAniamtionDuration       = data.GrowAniamtionDuration;
        BloomAnimationDuration      = data.BloomAnimationDuration;
        FallAnimationDuration       = data.FallAnimationDuration;
        RebloomAnimationDuration    = data.RebloomAnimationDuration;

        LeavesList = new List<GameObject>();
        BranchesList = new List<GameObject>();
        FlowersList = new List<GameObject>();
        PetalsList = new List<GameObject>();

        durationList = new List<float>()
        {
            SproutAnimationDuration,
            GrowAniamtionDuration,
            BloomAnimationDuration,
            FallAnimationDuration,
            RebloomAnimationDuration
        };

        durationCycle = BloomAnimationDuration + FallAnimationDuration + RebloomAnimationDuration;

        TreeColor = data.TreeColor;
        BranchColor = data.BranchColor;
        StemColor = data.StemColor;
        StemBranchColor = data.StemBranchColor;
        PistilColor = data.PistilColor;
        PetalColorRandom = data.PetalColorRandom;
        LeafColorRandom = data.LeafColorRandom;
        if (!PetalColorRandom) PetalColor = data.PetalColor;
        else PetalColor = GetColorFromRange(data.PetalColorRange1, data.PetalColorRange2);
        if (!LeafColorRandom) LeafColor = data.LeafColor;
        else LeafColors = DistributeColorsFromRange(LeafCount, data.LeafColorRange1, data.LeafColorRange2);

        DistributeGameObjectsByType(plantFormType);

        transform.RotateAround(transform.position, Vector3.up, rotation);

        SetAnimationState(FlowerAnimationState.Sprout);
    }

    void DistributeGameObjectsByType(PlantFormType type)
    {
        switch (type)
        {
            case PlantFormType.A:
                Stem    = InitWithParent("Stem", transform);
                Leaves  = InitWithParent("Leaves", Stem.transform);
                Flower  = InitWithParent("Flower", Stem.transform);

                //Initialize Stem + Mesh
                Spline tempSpline = CreateSplinefromNodes(SproutPathNodes);
                SproutPathNodes = MatchNodeCount(SproutPathNodes, StemPathNodes, tempSpline);
                CurrentMainSpline = InitSpline(Stem, SproutPathNodes);
                InitMesh(Stem);
                ApplyColorToMesh(Stem, StemColor);

                //Distribute and Initialize Leaves GameObjects
                Debug.Log(LeafFixedPosition);
                LeafPositionList = DistributeLeafPositions(LeafCount, LeafFixedPosition);
                LeafRotationList = DistributeLeafRotations(LeafPositionList.Count);

                SproutLeafCount = UnityEngine.Random.Range(SproutLeafCountRange.x, SproutLeafCountRange.y + 1);
                SproutPositionList = LeafPositionList;

                for (int i = 0; i < LeafCount; i++)
                {
                    GameObject leaf = Instantiate(LeafPrefab, Leaves.transform);
                    LeafLocalData leafData = leaf.GetComponent<LeafLocalData>();
                    Transform leafTransform = leaf.transform;
                    leafData.parent = Leaves;
                    leafData.LeafIndex = i;
                    leafData.TotalLeafCount = LeafCount;
                    if (leafGrowRelation == LeafGrowRelation.Same) leafData.SproutPosition = LeafPositionList[i];
                    else leafData.SproutPosition = SproutPositionList[i];
                    leafData.FinalPosition = LeafPositionList[i];
                    leafData.Rotation = LeafRotationList[i];
                    leafData.SproutScale = SproutLeafScale + SproutLeafScale * UnityEngine.Random.Range(- LeafScaleRandomValue, LeafScaleRandomValue);
                    if (LeafColorRandom)
                    {
                        leafData.leafColor = LeafColors[i];
                        leafData.AssignMaterialColor(LeafColors[i]);
                    }
                    else
                    {
                        leafData.leafColor = LeafColor;
                        leafData.AssignMaterialColor(LeafColor);
                    }

                    UpdateTransformOnSpline(leaf, CurrentMainSpline, leafData.SproutPosition, leafData.Rotation, 0f);

                    LeavesList.Add(leaf);
                }

                //Distribute and Initialize Flower GameObjects 
                PetalLayerCounts = new int[1];
                PetalLayerCounts[0] = UnityEngine.Random.Range(data.PetalLayerCountRange.x, data.PetalLayerCountRange.y);
                PetalCounts = DistributeRandomIntArray(PetalLayerCounts[0], data.PetalCountRange);

                InitFlower(Flower, PetalLayerCounts[0], PetalCounts, Stem);

                break;
            case PlantFormType.B:
                Stem = InitWithParent("Stem", transform);
                Leaves = InitWithParent("Leaves", Stem.transform);
                Flower = InitWithParent("Flower", Stem.transform);

                //Initialize Stem + Mesh
                Spline tempSpline2 = CreateSplinefromNodes(SproutPathNodes);
                SproutPathNodes = MatchNodeCount(SproutPathNodes, StemPathNodes, tempSpline2);
                CurrentMainSpline = InitSpline(Stem, SproutPathNodes);
                InitMesh(Stem);
                ApplyColorToMesh(Stem, StemColor);

                //Distribute and Initialize Leaves GameObjects
                Debug.Log(LeafFixedPosition);
                LeafPositionList = DistributeLeafPositions(LeafCount, LeafPositionRange, LeafPositionRandomPercentage);
                LeafRotationList = DistributeLeafRotations(LeafPositionList.Count);

                SproutLeafCount = UnityEngine.Random.Range(SproutLeafCountRange.x, SproutLeafCountRange.y + 1);
                SproutPositionList = LeafPositionList;

                for (int i = 0; i < LeafCount; i++)
                {
                    GameObject leaf = Instantiate(LeafPrefab, Leaves.transform);
                    LeafLocalData leafData = leaf.GetComponent<LeafLocalData>();
                    Transform leafTransform = leaf.transform;
                    leafData.parent = Leaves;
                    leafData.LeafIndex = i;
                    leafData.TotalLeafCount = LeafCount;
                    if (leafGrowRelation == LeafGrowRelation.Same) leafData.SproutPosition = LeafPositionList[i];
                    else leafData.SproutPosition = SproutPositionList[i];
                    leafData.FinalPosition = LeafPositionList[i];
                    leafData.Rotation = LeafRotationList[i];
                    leafData.SproutScale = SproutLeafScale + SproutLeafScale * UnityEngine.Random.Range(-LeafScaleRandomValue, LeafScaleRandomValue);
                    if (LeafColorRandom)
                    {
                        leafData.leafColor = LeafColors[i];
                        leafData.AssignMaterialColor(LeafColors[i]);
                    }
                    else
                    {
                        leafData.leafColor = LeafColor;
                        leafData.AssignMaterialColor(LeafColor);
                    }

                    UpdateTransformOnSpline(leaf, CurrentMainSpline, leafData.SproutPosition, leafData.Rotation, 0f);

                    LeavesList.Add(leaf);
                }

                //Distribute and Initialize Flower GameObjects 
                PetalLayerCounts = new int[1];
                PetalLayerCounts[0] = UnityEngine.Random.Range(data.PetalLayerCountRange.x, data.PetalLayerCountRange.y);
                PetalCounts = DistributeRandomIntArray(PetalLayerCounts[0], data.PetalCountRange);

                InitFlower(Flower, PetalLayerCounts[0], PetalCounts, Stem);

                break;
            case PlantFormType.C:
                Stem = InitWithParent("Stem", transform);
                StemBranches = InitWithParent("StemBranches", Stem.transform);
                Flower = InitWithParent("Flower", Stem.transform);

                CurrentMainSpline = InitSpline(Stem, SproutPathNodes);
                InitMesh(Stem);
                ApplyColorToMesh(Stem, StemColor);

                break;
            case PlantFormType.D:
                Stem = InitWithParent("Stem", transform);
                Branches = InitWithParent("Branches", Stem.transform);
                Leaves = InitWithParent("Leaves", Stem.transform);

                CurrentMainSpline = InitSpline(Stem, SproutPathNodes);
                InitMesh(Stem);
                ApplyColorToMesh(Stem, StemColor);

                break;
            case PlantFormType.E:
                Tree = InitWithParent("Tree", transform);
                Branches = InitWithParent("Branches", Tree.transform);

                TreeSpline = InitSpline(Tree, SproutPathNodes);
                InitMesh(Tree);
                ApplyColorToMesh(Tree, TreeColor);

                break;
            default:
                Console.WriteLine("PlantFormType is Broken");
                break;
        }
    }

    /// <summary>
    /// Save Flower Data to Save Data
    /// </summary>
    void SaveFlowerData()
    {

    }
    /// <summary>
    /// Load Saved Data
    /// </summary>
    void loadFlowerData()
    {
        //현재 시각과 대비해서 현재 스테잇을 정하는 것이 가장 중요
    }
    private void AddAnimationMethodsToList()
    {
        SproutMethods.Add(SproutA);
        SproutMethods.Add(SproutB);
        SproutMethods.Add(SproutC);
        SproutMethods.Add(SproutD);
        SproutMethods.Add(SproutE);

        GrowMethods.Add(GrowA);
        GrowMethods.Add(GrowB);
        GrowMethods.Add(GrowC);
        GrowMethods.Add(GrowD);
        GrowMethods.Add(GrowE);

        BloomMethods.Add(BloomA);
        BloomMethods.Add(BloomB);
        BloomMethods.Add(BloomC);
        BloomMethods.Add(BloomD);
        BloomMethods.Add(BloomE);

        FallMethods.Add(FallA);
        FallMethods.Add(FallB);
        FallMethods.Add(FallC);
        FallMethods.Add(FallD);
        FallMethods.Add(FallE);

        RebloomMethods.Add(RebloomA);
        RebloomMethods.Add(RebloomB);
        RebloomMethods.Add(RebloomC);
        RebloomMethods.Add(RebloomD);
        RebloomMethods.Add(RebloomE);
    }

    private void DistributeAnimationMethodsByType()
    {
        Sprout  = SproutMethods [(int)plantFormType];
        Grow    = GrowMethods   [(int)plantFormType];
        Bloom   = BloomMethods  [(int)plantFormType];
        Fall    = FallMethods   [(int)plantFormType];
        Rebloom = RebloomMethods[(int)plantFormType];
    }

    #endregion

    #region Random Data Distribution

    private int[] DistributeRandomIntArray(int howMany , Vector2Int range)
    {
        int[] counts = new int[2];

        for(int i = 0; i < howMany; i++)
        {
            counts[i] = UnityEngine.Random.Range(range.x, range.y);
        }
        return counts;
    }

    /// <summary>
    /// Used for Fixing Random Values when a FlowerPrefab is Instantiated
    /// </summary>
    /// <param name="RandomizablePathData"></param>
    /// <returns></returns>
    private Node[] DistributePath(RandomPath RandomizablePathData)
    {
        Node[] tempNodeList = (Node[])RandomizablePathData.nodes.Clone();

        foreach (RandomNode rn in RandomizablePathData.randomNode)
        {
            float random = UnityEngine.Random.Range(rn.randomRange.min, rn.randomRange.max);
            tempNodeList[rn.randomNodeIndex].position[(int)rn.randomAxis] += random;
            tempNodeList[rn.randomNodeIndex].handleOut[(int)rn.randomAxis] += random;
        }

        return tempNodeList;
    }

    /// <summary>
    /// Distributes Leaf Positions
    /// </summary>
    /// <param name="countRange"></param>
    /// <param name="positionRange"></param>
    /// <param name="positionRandomValue"></param>
    /// <returns>
    /// Returns ListArray of 0 < float < 1 values that represents relative position on stem or branch
    /// </returns>
    private List<float> DistributeLeafPositions(int leafCount, Vector2 positionRange, float positionRandomValue)
    {
        List<float> positions = new List<float>();

        float from = positionRange.x;
        float to = positionRange.y;
        float range = to - from;
        float interval = range / leafCount;

        for (int i = 0; i < leafCount; i++)
        {
            float random = UnityEngine.Random.Range(-positionRandomValue, positionRandomValue);
            positions.Add(from + (interval * i) + (interval * random) );
        }

        return positions;
    }
    private List<float> DistributeLeafPositions(int leafCount, float fixedPosition)
    {
        List<float> positions = new List<float>();

        for (int i = 0; i < leafCount; i++)
        {
            positions.Add(fixedPosition);
        }

        return positions;
    }
    private List<float> DistributeLeafPositions(int leafCount, float fixedPosition, int positionRandomValue)
    {
        List<float> positions = new List<float>();

        for (int i = 0; i < leafCount; i++)
        {
            float random = UnityEngine.Random.Range(-positionRandomValue, positionRandomValue);
            positions.Add(fixedPosition + random);
        }

        return positions;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="leafCount"></param>
    /// <returns>Float Value represents Euler angles 0 to 360</returns>
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

    private Color[] DistributeColorsFromRange(int LeafCount, Color LeafColorRange1, Color LeafColorRange2)
    {
        Color[] colorArray = new Color[LeafCount];
        for(int i = 0; i < LeafCount; i++)
        {
            colorArray[i] = GetColorFromRange(LeafColorRange1, LeafColorRange2);
        }
        return colorArray;
    }

    private void InitFlower(GameObject flowerObject, int PetalLayerCount, int[] PetalCounts, GameObject parent)
    {
        GameObject Pistil = InitWithParent(PistilPrefab, flowerObject.transform);
        GameObject Petals = InitWithParent("Petals", flowerObject.transform);

        FlowerLocalData flowerData  = flowerObject.AddComponent<FlowerLocalData>();
        flowerObject.AddComponent<Animator>().runtimeAnimatorController = NormalizedTimeAnimationController;

        flowerData.PetalLayerCount  = PetalLayerCount;
        flowerData.PetalCounts      = PetalCounts;
        flowerData.parent           = parent;

        for (int i = 0; i < PetalLayerCount; ++i)
        {
            GameObject petalLayer = InitWithParent("PetalLayer" + i, flowerObject.transform);
            petalLayer.transform.SetParent(Petals.transform);
            petalLayer.transform.localPosition = new Vector3(0, 0, 0);

            float petalLayerClosedTime = (PetalMaxClosedTime - PetalMinClosedTime) / PetalLayerCount * i + PetalMinClosedTime;
            float petalLayerOpenTime = (PetalMaxOpenTime - PetalMinOpenTime) / PetalLayerCount * i + PetalMinOpenTime;

            for (int j = 0; j < PetalCounts[i]; ++j)
            {
                float RandomValue = UnityEngine.Random.Range(-PetalRandomTimeValue, PetalRandomTimeValue);

                GameObject petal = Instantiate(PetalPrefab, petalLayer.transform);
                //TODO - SetLocalData
                PetalLocalData petalLocalData = petal.GetComponent<PetalLocalData>();
                petalLocalData.Rotation = (360 / PetalCounts[i] * j) +i * 30;
                petalLocalData.PetalLayer = i;
                petalLocalData.PetalIndex = j;
                petalLocalData.StartTime = petalLayerClosedTime + RandomValue;
                petalLocalData.EndTime = petalLayerOpenTime + RandomValue;
                petalLocalData.parent = petalLayer;
                petalLocalData.FallTime = UnityEngine.Random.Range(0f, 5f);

                //SetTransform, Animation NormalizedTime
                petal.transform.Rotate(new Vector3(0, petalLocalData.Rotation, 0));
                petal.GetComponent<Animator>().SetFloat("Time", petalLocalData.StartTime);
                //TODO 머테리얼 입힐 조금 더 똑똑한 방법을 생각해야할듯
                petal.transform.Find("Plane").GetComponent<SkinnedMeshRenderer>().material.color = PetalColor;

                flowerData.TotalPetalCount++;
                PetalsList.Add(petal);
}
        }

        UpdateTransformOnSpline(flowerObject, CurrentMainSpline, 1f, 0f, 0f);
        flowerObject.SetActive(false);
    }

    #endregion

    #region Public Animation Functions

    #region Animation Progress and Animation State

    public FlowerAnimationState GetCurrentState()
    {
        return currentAnimationState;
    }
    public void SetAnimationState(FlowerAnimationState state)
    {
        currentAnimationState = state;
        LastStateChangedTime = DateTime.Now;
    }
    public float GetProgression()
    {
        float currentStateProgressionSeconds = (float)(DateTime.Now - LastStateChangedTime).TotalSeconds;
        float currentStateAnimationDuration = durationList[(int)currentAnimationState];
        float progression = currentStateProgressionSeconds / currentStateAnimationDuration;

        return progression;
    }
    public void SwitchToNextState()
    {
        if (currentAnimationState != FlowerAnimationState.Rebloom)
        {
            currentAnimationState = currentAnimationState.Next();
        }
        else
        {
            currentAnimationState = FlowerAnimationState.Bloom;
        }
        LastStateChangedTime = DateTime.Now;
    }

    #endregion

    #region Animation Sub Methods

    public void InitParticles()
    {
        Instantiate(SproutParticles, transform);
    }

    #endregion

    #region Sprout Animations by Type

    /// <summary> 처음 심었을때 새싹까지 Animation Controller의 OnStateUpdate에서 실행됨(약5초) </summary>
    public void SproutA(float progress)
    {
        //TODO Progress Scriptable Object 에서 설정하게끔 바꿔라
        float[] stemProgress = new float[2] { 0f , .8f };
        float[] leafProgress = new float[2] { .5f , 1f };

        float currentStemProgress = (progress - stemProgress[0]) / (stemProgress[1] - stemProgress[0]);
        float currentLeafProgress = (progress - leafProgress[0]) / (leafProgress[1] - leafProgress[0]);

        if( 0f <= currentStemProgress && currentStemProgress <= 1f)
        {
            float currentThicknessMax = SproutPathData.pathMeshProperties.Thickness.max * currentStemProgress;
            float currentThicknessMin = SproutPathData.pathMeshProperties.Thickness.min * currentStemProgress;
            DrawStem(CurrentMainSpline, currentThicknessMax, currentThicknessMin, 0f, currentStemProgress);
        }

        if (0f <= currentLeafProgress && currentLeafProgress <= 1f)
        {
            foreach(GameObject leaf in LeavesList)
            {
                LeafLocalData leafData = leaf.GetComponent<LeafLocalData>();
                float currentScale = SproutLeafScale * currentLeafProgress;
                UpdateTransformOnSpline(leaf, CurrentMainSpline, leafData.SproutPosition * progress, leafData.Rotation, currentScale);
            }
        }
    }
    public void SproutB(float progress)
    {
        SproutA(progress);
    }
    public void SproutC(float progress)
    {

    }
    public void SproutD(float progress)
    {

    }
    public void SproutE(float progress)
    {

    }

    #endregion

    #region Grow Animations by Type

    /// <summary> 새싹에서 봉우리까지의 성장 (약1시간) </summary>
    public void GrowA(float progress)
    {
        //TODO Progress Scriptable Object 에서 설정하게끔 바꿔라
        //Stem Grow Progress , Stem Mesh Progress, Bug Growth Progress, Leaf Growth Progress 
        float[] stemProgress = new float[2] { 0f, 1f };
        //float[] leafProgress = new float[2] { .5f, 1f };

        float currentStemProgress = (progress - stemProgress[0]) / (stemProgress[1] - stemProgress[0]);
        //float currentLeafProgress = (progress - leafProgress[0]) / (leafProgress[1] - leafProgress[0]);

        if (0f <= currentStemProgress && currentStemProgress <= 1f)
        {
            Node[] lerpedNodeList = LerpNodeList(SproutPathNodes, StemPathNodes, CurrentMainSpline, progress);
            CurrentMainSpline = UpdateNodeToSpline(CurrentMainSpline, lerpedNodeList);
            float currentThicknessMax = (StemPathData.pathMeshProperties.Thickness.max - SproutPathData.pathMeshProperties.Thickness.max) * currentStemProgress + SproutPathData.pathMeshProperties.Thickness.max;
            float currentThicknessMin = (StemPathData.pathMeshProperties.Thickness.min = SproutPathData.pathMeshProperties.Thickness.min) * currentStemProgress + SproutPathData.pathMeshProperties.Thickness.min;
            DrawStem(CurrentMainSpline, currentThicknessMax, currentThicknessMin, 0f, 1f);
            UpdateTransformOnSpline(Flower, CurrentMainSpline, 1f, 0f, progress);
        }

        foreach (GameObject leaf in LeavesList)
        {
            LeafLocalData leafData = leaf.GetComponent<LeafLocalData>();
            float positionTime = 0f;
            if (leafGrowRelation == LeafGrowRelation.Same)
            {
                positionTime = leafData.FinalPosition;
            }
            else
            {
                positionTime = progress * (leafData.FinalPosition - leafData.SproutPosition) + leafData.SproutPosition;
            }
            UpdateTransformOnSpline(
                leaf, CurrentMainSpline,
                positionTime,
                leafData.Rotation, 
                progress * (1f - leafData.SproutScale) + leafData.SproutScale);
        }
    }
    public void GrowB(float progress)
    {
        GrowA(progress);
    }
    public void GrowC(float progress) { }
    public void GrowD(float progress) { }           
    public void GrowE(float progress) { }
    public void FastGrow()
    {
        //TODO 로얄젤리 썼을때 빨리 자라는거 구현
    }
    /// <summary> Grow 단계 이전에 처리할 것들 </summary>
    public void OnGrowStart()
    {

        MatchNodeCount(SproutPathNodes, StemPathNodes, CurrentMainSpline);
        Flower.SetActive(true);
    }

    #endregion

    #region Bloom Animations by Type
    /// <summary> 성장이 끝나면 시작 봉우리에서 피는 것 까지 에니메이션, 끝나면 활성화 (Grow를 Rj내고 끝내면 약5초 / else 약15초?) </summary>
    public void BloomA(float progress)
    {
        foreach(GameObject p in PetalsList)
        {
            PetalLocalData petalData = p.GetComponent<PetalLocalData>();
            float localProgress = CalculateTimeByMinToMax(petalData.StartTime, petalData.EndTime, progress);
            SetAnimationNormalizedTime(p, "Time", localProgress);
        }
    }
    public void BloomB(float progress)
    {
        BloomA(progress);
    }
    public void BloomC(float progress) { }
    public void BloomD(float progress) { }
    public void BloomE(float progress) { }

    #endregion

    #region Fall Animations by Type
    /// <summary> 활성화 상태에서 비활성화 상태 사이에 천천히 꽃이 지는 에니메이션 </summary>
    public void FallA(float progress)
    {
        List<int> tempDestroyIndex = new List<int>();
        // Flower Petal, Pistils Normalize Value에 따라 닫히거나 쪼그라듬
        for (int i = 0; i < PetalsList.Count; i++)
        {
            GameObject p = PetalsList[i];
            PetalLocalData pData = p.GetComponent<PetalLocalData>();
            float tempFallTime = pData.FallTime;
            bool isOnFlower = pData.isOnFlower;
            //if (pData.waitForDisable)
            //{
            //    tempDestroyIndex.Add(i);
            //}
            //else
            //{
                if (tempFallTime > 0 && tempFallTime < progress && isOnFlower)
                {
                    p.GetComponent<Rigidbody>().isKinematic = false;
                    p.GetComponent<Rigidbody>().useGravity = true;
                    p.GetComponent<Rigidbody>().AddForce(Vector3.up * 4);
                }
                else if (tempFallTime < 0f)
                {
                    p.GetComponent<PetalLocalData>().isOnFlower = false;
                    
                }
            }
        //}
        //for (var j = 0; j < tempDestroyIndex.Count; j++)
        //{
        //    PetalsList.RemoveAt(tempDestroyIndex[tempDestroyIndex.Count - 1 - j]);
        //    Destroy(PetalsList[tempDestroyIndex[tempDestroyIndex.Count - 1 - j]]);
        //}
    }
    public void FallB(float progress)
    {
        FallA(progress);
    }
    public void FallC(float progress) { }
    public void FallD(float progress) { }
    public void FallE(float progress) { }
    public void OnFallExit()
    {
        foreach (GameObject p in PetalsList)
        {
            p.GetComponent<PetalLocalData>().FallTime--;
            p.GetComponent<Rigidbody>().AddForce((Vector3.right + Vector3.forward) * 20);
        }
    }
    #endregion

    #region ReBloom Animations by Type
    /// <summary> 꽃이 진 상태에서 봉우리까지의 상태로 에니메이션 (꽃이 고유하게 가진 Fall Time) </summary>
    public void RebloomA(float progress)
    {
        foreach (GameObject p in PetalsList)
        {
            PetalLocalData petalData = p.GetComponent<PetalLocalData>();
            float localProgress = CalculateTimeByMaxToMin(petalData.EndTime, petalData.StartTime, progress);
            SetAnimationNormalizedTime(p, "Time", localProgress);
        }
    }
    public void RebloomB(float progress)
    {
        RebloomA(progress);
    }
    public void RebloomC(float progress) { }
    public void RebloomD(float progress) { }
    public void RebloomE(float progress) { }

    #endregion

    #endregion

    #region Private Utility Functions
    private GameObject InitWithParent(GameObject prefab, Transform parent)
    {
        GameObject instantiatedObject = Instantiate(prefab, parent);
        instantiatedObject.transform.localPosition = Vector3.zero;
        return instantiatedObject;
    }
    private GameObject InitWithParent(string name, Transform parent)
    {
        GameObject newObject = new GameObject(name);
        newObject.transform.SetParent(parent);
        newObject.transform.localPosition = Vector3.zero;
        return newObject;
    }
    private void InitMesh(GameObject obj)
    {
        obj.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
        meshFilter.mesh = new Mesh();
    }
    private Color GetColorFromRange(Color c1, Color c2)
    {
        Color color = new Color(0, 0, 0, 1);

        color.r = UnityEngine.Random.Range(c1.r, c2.r);
        color.g = UnityEngine.Random.Range(c1.g, c2.g);
        color.b = UnityEngine.Random.Range(c1.b, c2.b);

        return color;
    }
    private void ApplyColorToMesh(GameObject o, Color c)
    {
        MeshRenderer renderer;
        try
        {
            renderer = o.GetComponent<MeshRenderer>();
        }
        catch (MissingComponentException e)
        {
            throw new MissingComponentException("MeshRenderer Component is Missing", e);
        }
        renderer.material.color = c;
    }
    public void InvokeAnimation(string methodName, float seconds)
    {
        try
        {
            Invoke(methodName, seconds);
        }
        catch(MissingMethodException e)
        {
            throw new MissingMethodException("Wrong Method name to Invoke", e);
        }
    }

    #endregion

    #region Private Animation Functions
    private void UpdateTransformOnSpline(GameObject objectToUpdate, Spline spline, float positionTime, float yEulerAngle, float localScale)
    {
        CurveSample p = GetSampleAt(spline, positionTime);
        UpdatePositionOnSpline(objectToUpdate, p.location);
        UpdateRotationOnSpline(objectToUpdate, yEulerAngle, p.tangent);
        UpdateLocalScale(objectToUpdate, localScale);
        objectToUpdate.transform.RotateAround(transform.position, Vector3.up, rotation);
    }
    private void UpdateTransformOnSpline(GameObject objectToUpdate, Spline spline, float positionTime)
    {
        CurveSample p = GetSampleAt(spline, positionTime);
        UpdatePositionOnSpline(objectToUpdate, p.location);
        UpdateRotationOnSpline(objectToUpdate, 0f, p.tangent);
    }
    private void UpdatePositionOnSpline(GameObject objectToUpdate, Vector3 p)
    {
        objectToUpdate.transform.position = p + transform.position;
    }
    private void UpdatePositionOnSpline(GameObject objectToUpdate, Spline spline, float positionTime)
    {
        CurveSample p = GetSampleAt(spline, positionTime);
        objectToUpdate.transform.position = p.location + transform.position;
    }
    private void UpdateRotationOnSpline(GameObject objectToUpdate, float yEulerAngle, Vector3 tangent)
    {
        objectToUpdate.transform.rotation = Quaternion.Euler(0, yEulerAngle, Vector3.Angle(Vector3.up, tangent));
    }
    private void UpdateLocalScale(GameObject objectToUpdate, float localScale)
    {
        objectToUpdate.transform.localScale = Vector3.one * localScale;
    }

    private void UpdateLeafTransform(GameObject leaf, Spline spline, float position, float yRotation, float localScale)
    {
        CurveSample p = GetSampleAt(spline, position);
        leaf.transform.position = p.location + transform.position;
        leaf.transform.rotation = Quaternion.Euler(0, yRotation, Vector3.Angle(Vector3.up, p.tangent));
        leaf.transform.localScale = Vector3.one * localScale;
    }
    //TODO set this to be calculate each petal's own start~end Time and then sign it.
    public void SetAnimationNormalizedTime(GameObject obj, string floatName, float progress)
    {

        obj.GetComponent<Animator>().SetFloat(floatName, progress);
    }
    private float CalculateTimeByMinToMax(float min, float max, float progress)
    {
        float calculatedTime = ( ( max - min ) * progress ) + min;
        return calculatedTime;
    }
    private float CalculateTimeByMaxToMin(float max, float min, float progress)
    {
        float calculatedTime = max - (max - min) * progress;
        return calculatedTime;
    }

    void SetAnimationNormalizedTime(GameObject obj, string floatName, float progress, int retryNumber = 0)
    {
        const int maxRetryNumber = 5;
        try
        {
            obj.GetComponent<Animator>().SetFloat(floatName, progress);
        }
        catch (MissingComponentException e)
        {
            if (retryNumber < maxRetryNumber)
            {
                obj.AddComponent<Animator>().runtimeAnimatorController = NormalizedTimeAnimationController;
                SetAnimationNormalizedTime(obj, floatName, progress, retryNumber + 1);
            }
            else
            {
                throw e;
            }
                
        }
    }

    #endregion

    #region Generate Mesh Functions

    private void DrawStem(Spline spline, float maxThickness, float minThickness)
    {
        DrawStem(spline, maxThickness, minThickness, 0f, 1f);
    }

    private void DrawStem(Spline spline, float maxThickness, float minThickness, float startTime = 0f, float endTime = 1f, CapType capType = CapType.round)
    {
        const int vertexCount = 8;

        List<Vector3> meshVertices = new List<Vector3>();
        List<int> meshTriangles = new List<int>();

        //Init mesh to ReDraw
        Mesh mesh;
        try
        {
            mesh = spline.gameObject.GetComponent<MeshFilter>().mesh;
            mesh.Clear();
        }
        catch (MissingComponentException e)
        {
            throw new MissingComponentException("Mesh Related Component is Missing", e);
        }

        int vertexRingCount = (int)Math.Round(spline.Length * (endTime - startTime) / MAX_VERTEXT_DISTANCE);
        if (vertexRingCount < 2) vertexRingCount = 2;
        float vertexInterval = (endTime - startTime) / vertexRingCount;
        float thicknessInterval = (maxThickness - minThickness) / vertexRingCount;

        //for each Vertex Path Generate 8 vertices for a circle
        for (int i = 0; i <= vertexRingCount; i++)
        {
            CurveSample centerSamplePoint = GetSampleAt(spline, startTime + (vertexInterval * i));

            float currentThickness = maxThickness - thicknessInterval * i;
            Vector3[] tempList = GenerateCircleVertices(centerSamplePoint, vertexCount, currentThickness);
            meshVertices.AddRange(tempList);
        }

        //Generate triangles for Quads
        for (int i = 0; i < vertexRingCount; i++)
        {
            int initialPoint = i * vertexCount;
            int[] tempList = GenerateCircleTriangles(initialPoint, vertexCount);
            meshTriangles.AddRange(tempList);
        }

        //GenerateCap
        switch (capType)
        {
            case CapType.flat:

                break;

            case CapType.sharp:

                break;

            case CapType.round:
                
                //Add Cap Vertices
                CurveSample endPoint = GetSampleAt(spline, endTime);
                Vector3 radiusDistanceFromEndPoint = endPoint.tangent * minThickness / 2;
                Vector3 centerPoint = endPoint.location + radiusDistanceFromEndPoint / 2;

                float currentThickness = minThickness * (float)Math.Sqrt(3) / 2;
                Vector3[] tempPointList = GenerateCircleVertices(centerPoint, endPoint.tangent, vertexCount, currentThickness);
                meshVertices.AddRange(tempPointList);

                Vector3 lastPoint = endPoint.location + radiusDistanceFromEndPoint;
                meshVertices.Add(lastPoint);

                //Add Triangles
                int initialPoint = (vertexRingCount) * vertexCount;
                int[] tempTriangleList = GenerateCircleTriangles(initialPoint, vertexCount);
                meshTriangles.AddRange(tempTriangleList);

                int[] capTrinangleList = GenerateCapTriangles(meshVertices.Count - 3 - vertexCount, meshVertices.Count - 2, meshVertices.Count - 1);

                meshTriangles.AddRange(capTrinangleList);
                break;

            default:

                break;
        }

        mesh.SetVertices(meshVertices);
        mesh.SetTriangles(meshTriangles, 0);

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    private int[] GenerateCapTriangles(int pIndex1, int pIndex2, int lastPIndex)
    {
        int count = Math.Abs(pIndex2 - pIndex1);
        int[] capTriangles = new int[count * 3];
        int firstPoint;
        if (pIndex1 > pIndex2) firstPoint = pIndex2;
        else firstPoint = pIndex1;

        for (int i = 0; i < count; i++)
        {
            int lap = 0;
            if (i == count - 1) lap = -count;
            capTriangles[i * 3] = lap + firstPoint + i;
            capTriangles[i * 3 + 1] = lastPIndex;
            capTriangles[i * 3 + 2] = lap + firstPoint + i + 1;
        }

        return capTriangles;
    }
    private Vector3[] GenerateCircleVertices(CurveSample samplePoint, int vertexCount, float width)
    {
        return GenerateCircleVertices(samplePoint.location, samplePoint.tangent, vertexCount, width);
    }
    private Vector3[] GenerateCircleVertices(Vector3 localCenterPoint, Vector3 tangent, int vertexCount, float width)
    {
        Vector3[] circleVertexList = new Vector3[vertexCount];
        float radius = width;
        Vector3 firstPoint = new Vector3(radius / 2, 0, 0);
        for (int i = 0; i < vertexCount; i++)
        {
            Vector3 rotationAxis;
            if (i == 0) rotationAxis = Vector3.up;
            else rotationAxis = tangent;

            Vector3 p = Quaternion.AngleAxis(360 / vertexCount * -i, rotationAxis) * firstPoint + localCenterPoint;
            circleVertexList[i] = p;
        }

        return circleVertexList;
    }
    private int[] GenerateCircleTriangles(int initPoint, int vertexCount)
    {

        //Generate Cylinder Triangles according to 
        int[] circleTriangleList = new int[vertexCount * 6];

        for (int i = 0; i < vertexCount; i++)
        {
            int[] tempQuad;
            if (i == vertexCount - 1)
            {
                tempQuad = GenerateQuad(
                    initPoint + i,
                    initPoint + i - vertexCount + 1,
                    initPoint + i + vertexCount,
                    initPoint + i + 1
                );
            }
            else
            {
                tempQuad = GenerateQuad(
                    initPoint + i,
                    initPoint + i + 1,
                    initPoint + i + vertexCount,
                    initPoint + i + vertexCount + 1
                );
            }

            for (int j = 0; j < tempQuad.Length; j++)
            {
                circleTriangleList[i * 6 + j] = tempQuad[j];
            }
        }

        return circleTriangleList;
    }
    private int[] GenerateQuad(int a, int b, int c, int d)
    {
        //Generate Quad Array consisting of 6 integers
        //Clockwise = a > c > d > b
        int[] quadRoutine = new int[6];

        quadRoutine[0] = a;
        quadRoutine[1] = c;
        quadRoutine[2] = d;
        quadRoutine[3] = a;
        quadRoutine[4] = d;
        quadRoutine[5] = b;

        return quadRoutine;
    }

    #endregion

    #region Spline Helper Methods
    //Lerp Splines for Grow();
    private Spline InitSpline(GameObject o, Node[] nodes)
    {
        Spline spline = o.AddComponent<Spline>();
        spline = NodeToSpline(spline, nodes);
        return spline;
    }
    private Spline CreateSplinefromNodes(Node[] nodes)
    {
        Spline spline = new Spline();
        NodeToSpline(spline, nodes);
        return spline;
    }
    private Spline NodeToSpline(Spline spline, Node[] nodes)
    {
        foreach (Node n in nodes)
        {
            spline.AddNode(new SplineNode(n.position, n.handleOut));
        }
        return spline;
    }
    private Spline UpdateNodeToSpline(Spline spline, Node[] nodes)
    {
        for (int i = 0; i < spline.nodes.Count; i++)
        {
            spline.nodes[i].Position = nodes[i].position;
            spline.nodes[i].Direction = nodes[i].handleOut;
        }
        return spline;
    }
    /// <summary>
    /// This is a Emergency Match Node Count, Only use this when beform Lerping 2 Splines that can possibly have different Length of Node Arrays
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    private Spline MatchNodeCount(Spline from, Node[] to)
    {
        int fromCount = from.nodes.Count;
        int toCount = to.Length;
        int difference = toCount - fromCount;
        if (fromCount != toCount)
        {
           for(int i = 0; i < difference; i++)
            {
                //Adds Node between last and before last spline
                
                SplineNode lastNode = from.nodes[fromCount - 1];
                SplineNode formerLastNode = from.nodes[fromCount - 2];
                SplineNode firstNode = from.nodes[0];

                float fullDist = Vector3.Distance(lastNode.Position, firstNode.Position);
                float formerDist = Vector3.Distance(formerLastNode.Position, firstNode.Position);
                float lastFormerDist = Vector3.Distance(formerLastNode.Position, lastNode.Position);

                float insertTime = ((lastFormerDist / 2) + formerDist) / fullDist;

                CurveSample insertNode = GetSampleAt(from, insertTime);
                from.InsertNode(fromCount - 1, new SplineNode(insertNode.location,  insertNode.tangent * 0.1f + insertNode.location));
            }
        }
        return from;
    }
    /// <summary>
    /// Matches 2 Node Array Length with a temporary spline, fromSpline has to be a temporary spline, not a spline in Game Scene
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="fromSpline"></param>
    /// <returns></returns>
    private Node[] MatchNodeCount(Node[] from, Node[] to, Spline fromSpline)
    {
        int fromCount = from.Length;
        int toCount = to.Length;
        int difference = toCount - fromCount;

        if(fromCount != fromSpline.nodes.Count)
        {
            Debug.LogError("Node list and Spline Node List does not match");
        }

        List<Node> tempNodeList = new List<Node>(from);

        if (fromCount != toCount)
        {
            for (int i = 0; i < difference; i++)
            {
                //Adds Node between last and before last spline

                SplineNode lastNode = fromSpline.nodes[fromCount - 1];
                SplineNode formerLastNode = fromSpline.nodes[fromCount - 2];
                SplineNode firstNode = fromSpline.nodes[0];

                float fullDist = Vector3.Distance(lastNode.Position, firstNode.Position);
                float formerDist = Vector3.Distance(formerLastNode.Position, firstNode.Position);
                float lastFormerDist = Vector3.Distance(formerLastNode.Position, lastNode.Position);

                float insertTime = ((lastFormerDist / 2) + formerDist) / fullDist;

                CurveSample insertNodeSample = GetSampleAt(fromSpline, insertTime);
                Node insertNode = new Node(insertNodeSample.location, insertNodeSample.tangent * 0.1f + insertNodeSample.location);
                tempNodeList.Insert(tempNodeList.Count-1, insertNode);

                fromSpline.InsertNode(fromSpline.nodes.Count - 1, new SplineNode(insertNode.position, insertNode.handleOut));
            }
        }
        Node[] insertedNodeArray = tempNodeList.ToArray();

        return insertedNodeArray;
    }
    private Node[] LerpNodeList(Node[] from, Node[] to, Spline spline, float progress)
    {
        //Check Node Count to Lerp
        if (from.Length != to.Length)
        {
            spline = MatchNodeCount(spline, to);
            from = MatchNodeCount(from, to, spline);
        }

        Node[] lerpedNodeList = new Node[to.Length];

        for (int i = 0; i < lerpedNodeList.Length; i++)
        {
            lerpedNodeList[i] = LerpNode(from[i], to[i], progress);
        }

        return lerpedNodeList;
    }
    private Node LerpNode(Node from, Node to, float progress)
    {
        Vector3 lerpedPosition = (to.position - from.position) * progress + from.position;
        Vector3 lerpedhandleOut = (to.handleOut - from.handleOut) * progress + from.handleOut;
        Node changedNode = new Node(lerpedPosition, lerpedhandleOut);

        return changedNode;
    }

    private CurveSample GetSampleAt(Spline spline, float t)
    {
        return spline.GetSample(t * (spline.nodes.Count - 1));
    }
    private Vector3 GetPointAt(Spline spline, float t)
    {
        return spline.GetSample(t * (spline.nodes.Count - 1)).location;
    }
    private Vector3 GetDirectionAt(Spline spline, float t)
    {
        return spline.GetSample(t * (spline.nodes.Count - 1)).tangent;
    }

    #endregion
}
