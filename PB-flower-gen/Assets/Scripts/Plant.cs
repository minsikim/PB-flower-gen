using SplineMesh;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Plant : MonoBehaviour
{

    #region Basic Serializable Info
    [LabelOverride("Flower Form Data")]
    [SerializeField] PlantFormData data;
    #endregion

    #region Private Fields
    //Initial Datas
    private DateTime initialTime;

    //Flower Data
    private string plantName;
    private string description;

    private float rotation;

    private PlantFormType plantFormType;
    private FlowerFormType flowerFormType;

    private GameObject PetalPrefab;
    private GameObject PistilPrefab;
    private GameObject LeafPrefab;

    private int FlowerToPlantCount;

    private int[] PetalCounts;
    private int[] PetalLayerCounts;

    //Randomizable Path Data
    private RandomPath SproutPathData;
    private RandomPath StemPathData;
    private RandomPath StemBranchPathData;
    private RandomPath BranchPathData;
    private RandomPath TreePathData;

    //Distributed Path Data
    private Node[] SproutPathNodes;
    private Node[] StemPathNodes;
    private Node[] StemBranchPathNodes;
    private Node[] BranchPathNodes;
    private Node[] TreePathNodes;

    //Constant Mesh Generation Options
    private const float maxVertexDistance = 0.2f;

    //Leaf Position Data
    private Vector2Int LeafCountRange;
    private Vector2 LeafPositionRange;
    private float LeafPositionRandomPercentage;
    private float LeafFixedPosition;

    private int LeafCount;

    private LeafGrowRelation leafGrowRelation;
    private Vector2Int SproutLeafCountRange;
    private Vector2 SproutLeafPositionRange;
    private float SproutLeafPositionRandomPercentage;
    private float SproutLeafScale;

    private int SproutLeafCount;

    //Leaf Positions
    private List<float> LeafPositionList;
    private List<float> LeafRotationList;
    private List<float> SproutPositionList;

    //List of Children by Type
    private List<GameObject> LeavesData;
    private List<GameObject> BranchesData;
    private List<GameObject> FlowersData;

    //Children GameObjects
    private GameObject Tree;
    private GameObject Branch;
    private GameObject Stem;
    private GameObject StemBranches;
    private GameObject Branches;
    private GameObject Leaves;
    private GameObject Flower;
    private GameObject Flowers;

    private Spline TreeSpline;
    private Spline StemSpline;

    //Animation State
    private FlowerAnimationStates currentAnimationState = FlowerAnimationStates.Sprout;
    private DateTime LastStateChangedTime;

    //Animation Durations
    List<float> durationList;
    private float durantionCycle;

    private float SproutAnimationDuration;
    private float GrowAniamtionDuration;
    private float BloomAnimationDuration;
    private float FallAnimationDuration;
    private float RebloomAnimationDuration;

    //Animation Process Trackers
    private float initAnimationPosition = 0f;
    private float growAnimationPosition = 0f;
    private float bloomAnimationPosition = 0f;
    private float fallAnimationPosition = 0f;
    private float rebloomAnimationPosition = 0f;

    //Animator
    private Animator animator;

    //Colors
    private Color TreeColor;
    private Color BranchColor;
    private Color StemColor;
    private Color StemBranchColor;
    private Color PistilColor;
    private bool  PetalColorRandom;
    private Color PetalColor;
    private Color PetalColorRange1;
    private Color PetalColorRange2;

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

        //TODO 로딩이랑 처음 만드는 거랑 구분
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
    #endregion

    #region Data Managing Functions
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

        rotation = UnityEngine.Random.Range(0f, 360f);

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

        leafGrowRelation                    = data.leafGrowRelation;
        SproutLeafCountRange                = data.SproutLeafCountRange;
        SproutLeafPositionRange             = data.SproutLeafPositionRange;
        SproutLeafPositionRandomPercentage  = data.SproutLeafPositionRandomPercentage;
        SproutLeafScale                     = data.SproutLeafScale;

        SproutAnimationDuration     = data.SproutAnimationDuration;
        GrowAniamtionDuration       = data.GrowAniamtionDuration;
        BloomAnimationDuration      = data.BloomAnimationDuration;
        FallAnimationDuration       = data.FallAnimationDuration;
        RebloomAnimationDuration    = data.RebloomAnimationDuration;

        LeavesData = new List<GameObject>();
        BranchesData = new List<GameObject>();
        FlowersData = new List<GameObject>();

        durationList = new List<float>()
        {
            SproutAnimationDuration,
            GrowAniamtionDuration,
            BloomAnimationDuration,
            FallAnimationDuration,
            RebloomAnimationDuration
        };

        durantionCycle = BloomAnimationDuration + FallAnimationDuration + RebloomAnimationDuration;

        TreeColor = data.TreeColor;
        BranchColor = data.BranchColor;
        StemColor = data.StemColor;
        StemBranchColor = data.StemBranchColor;
        PistilColor = data.PistilColor;
        if (!data.PetalColorRandom) PetalColor = data.PetalColor;
        else PetalColor = GetColorFromRange(data.PetalColorRange1, data.PetalColorRange2);

        DistributeGameObjectsByType(plantFormType);

        SaveFlowerData();

        SetAnimationState(FlowerAnimationStates.Sprout);
    }

    void DistributeGameObjectsByType(PlantFormType type)
    {
        switch (type)
        {
            case PlantFormType.A:
                GameObject StemParent    = InitWithParent("StemParent",   transform);
                Stem    = InitWithParent("Stem", StemParent.transform);
                Leaves  = InitWithParent("Leaves", StemParent.transform);
                Flower  = InitWithParent("Flower", StemParent.transform);

                //Initialize Stem + Mesh
                StemSpline = MakeSpline(Stem, SproutPathNodes);
                InitMesh(Stem);
                ApplyColorToMesh(Stem, StemColor);
                //Stem.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, rotation));

                //Distribute and Initialize Leaves GameObjects
                LeafCount = UnityEngine.Random.Range(LeafCountRange.x, LeafCountRange.y + 1);
                LeafPositionList = DistributeLeafPositions(LeafCount, LeafPositionRange, LeafPositionRandomPercentage);
                LeafRotationList = DistributeLeafRotations(LeafPositionList.Count);


                //TODO 

                SproutLeafCount = UnityEngine.Random.Range(SproutLeafCountRange.x, SproutLeafCountRange.y + 1);
                if (leafGrowRelation == LeafGrowRelation.Same)
                {
                    SproutPositionList = LeafPositionList;
                }
                else if (leafGrowRelation == LeafGrowRelation.Differ)
                {
                    SproutPositionList = DistributeLeafPositions(LeafCount, SproutLeafPositionRange, SproutLeafPositionRandomPercentage);
                }


                for (int i = 0; i < LeafCount; i++)
                {
                    GameObject leaf = Instantiate(LeafPrefab, Leaves.transform);
                    LeafLocalData leafData = leaf.GetComponent<LeafLocalData>();
                    Transform leafTransform = leaf.transform;
                    leafData.parent = Leaves;
                    leafData.leafIndex = i;
                    leafData.totalLeafCount = LeafCount;
                    if (leafGrowRelation == LeafGrowRelation.Same) leafData.sproutPosition = LeafPositionList[i];
                    else leafData.sproutPosition = SproutPositionList[i];
                    leafData.finalPosition = LeafPositionList[i];
                    leafData.rotation = LeafRotationList[i];
                    leafData.sproutScale = SproutLeafScale;

                    //TODO
                    UpdateLeafTransform(leaf, StemSpline, leafData.sproutPosition, leafData.rotation, leafData.sproutScale);


                    LeavesData.Add(leaf);
                }

                //Distribute and Initialize Flower GameObjects 
                PetalLayerCounts = new int[1];
                PetalLayerCounts[0] = UnityEngine.Random.Range(data.PetalLayerCountRange.x, data.PetalLayerCountRange.y);
                PetalCounts = DistributeRandomIntArray(PetalLayerCounts[0], data.PetalCountRange);

                InitFlower(Flower, PetalLayerCounts[0], PetalCounts);

                //SaveDistributedData in Private Variables for Save Plant Form Data


                break;
            case PlantFormType.B:
                Stem = InitWithParent("Stem", transform);
                Leaves = InitWithParent("Leaves", Stem.transform);
                Flower = InitWithParent("Flower", Stem.transform);

                StemSpline = MakeSpline(Stem, SproutPathNodes);
                InitMesh(Stem);
                ApplyColorToMesh(Stem, StemColor);

                break;
            case PlantFormType.C:
                Stem = InitWithParent("Stem", transform);
                StemBranches = InitWithParent("StemBranches", Stem.transform);
                Flower = InitWithParent("Flower", Stem.transform);

                StemSpline = MakeSpline(Stem, SproutPathNodes);
                InitMesh(Stem);
                ApplyColorToMesh(Stem, StemColor);

                break;
            case PlantFormType.D:
                Stem = InitWithParent("Stem", transform);
                Branches = InitWithParent("Branches", Stem.transform);
                Leaves = InitWithParent("Leaves", Stem.transform);

                StemSpline = MakeSpline(Stem, SproutPathNodes);
                InitMesh(Stem);
                ApplyColorToMesh(Stem, StemColor);

                break;
            case PlantFormType.E:
                Tree = InitWithParent("Tree", transform);
                Branches = InitWithParent("Branches", Tree.transform);

                TreeSpline = MakeSpline(Tree, SproutPathNodes);
                InitMesh(Tree);
                ApplyColorToMesh(Tree, TreeColor);

                break;
            default:
                Console.WriteLine("PlantFormType is Broken");
                break;
        }
    }

    void UpdateFlowerData()
    {
        
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
            positions.Add(from + interval + (interval * random) );
        }

        return positions;
    }
    private List<float> DistributeLeafFixedPositions(int leafCount, Vector2 positionRange, float positionRandomValue)
    {
        List<float> positions = new List<float>();

        float from = positionRange.x;
        float to = positionRange.y;
        float range = to - from;
        float interval = range / leafCount;

        for (int i = 0; i < leafCount; i++)
        {
            float random = UnityEngine.Random.Range(-positionRandomValue, positionRandomValue);
            positions.Add(from + interval + (interval * random));
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

    private void AssignLeavesToSpline(Spline spline)
    {
        GameObject parent = spline.gameObject;

    }
    private void AssignBranchToSpline(Spline spline)
    {
        GameObject parent = spline.gameObject;

    }
    private void InitFlower(GameObject flowerObject, int PetalLayerCount, int[] PetalCounts)
    {
        const float CloseLayerDifference = 0.01f;

        GameObject Petals = InitWithParent("Petals", flowerObject.transform);
        for (int i = 0; i < PetalLayerCount; ++i)
        {
            InitWithParent("PetalLayer" + i, flowerObject.transform);
            GameObject petalLayer = new GameObject();
            petalLayer.transform.SetParent(Petals.transform);
            petalLayer.transform.localPosition = new Vector3(0, 0, 0);

            for (int j = 0; j < PetalCounts[i]; ++j)
            {
                GameObject petal = Instantiate(PetalPrefab, petalLayer.transform);
                petal.transform.Rotate(new Vector3(0, (360 / PetalCounts[i] * j) + i * 30, 0));
                petal.GetComponent<Animator>().SetFloat("Time", 0.0f + ((PetalLayerCount - i) * CloseLayerDifference));
            }
        }
    }
    #endregion

    #region Public Animation Functions

    #region Animation Progress and Animation State

    public FlowerAnimationStates GetCurrentState()
    {
        return currentAnimationState;
    }
    public void SetAnimationState(FlowerAnimationStates state)
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
        if (currentAnimationState != FlowerAnimationStates.Rebloom)
        {
            currentAnimationState = currentAnimationState.Next();
        }
        else
        {
            currentAnimationState = FlowerAnimationStates.Bloom;
        }
        LastStateChangedTime = DateTime.Now;
    }

    #endregion

    #region Animation Sub Methods

    private void UpdateLeafTransform(GameObject leaf, Spline spline, float position, float yRotation, float localScale)
    {
        leaf.transform.position = GetSampleAt(spline, position).location - transform.position;
        leaf.transform.Rotate(0, yRotation, 0);
        leaf.transform.localScale = Vector3.one * localScale;
    }

    #endregion

    #region Sprout Animations by Type

    /// <summary> 처음 심었을때 새싹까지 Animation Controller의 OnStateUpdate에서 실행됨(약5초) </summary>
    public void SproutA(float progress)
    {
        // 1. SproutSpline 값을 받아서 현재 시각에 따라 Mesh 만듦(Animation)
        // 2. FlowerData에서 정해진 위치와 각도를 받아와 그에 따라 새싹이 남
        //    -> 예시: SproutAnimationDurationValues = [ [ 0f, 0.5f ], [ 0.5f, 0.8f ], [ 0.7f, 1.0f ] ]
        // 3. SproutParticleAnimation 실행
        // 4. Finish Init -> Trigger Grow State

        //DrawSproutMesh(progress);
        //DrawSproutLeaf(progress);

        float[] stemProgress = new float[2] { 0f , .7f };
        float[] leafProgress = new float[2] { .7f , 1f };

        float currentStemProgress = (progress - stemProgress[0]) / (stemProgress[1] - stemProgress[0]);
        float currentLeafProgress = (progress - leafProgress[0]) / (leafProgress[1] - leafProgress[0]);

        if( 0f <= currentStemProgress && currentStemProgress <= 1f)
        {
            float currentThicknessMax = SproutPathData.pathMeshProperties.Thickness.max * currentStemProgress;
            float currentThicknessMin = SproutPathData.pathMeshProperties.Thickness.min * currentStemProgress;
            DrawStem(StemSpline, currentThicknessMax, currentThicknessMin, 0f, currentStemProgress);
        }

        if (0f <= currentLeafProgress && currentLeafProgress <= 1f)
        {
            float currentThicknessMax = SproutPathData.pathMeshProperties.Thickness.max * currentLeafProgress;
            float currentThicknessMin = SproutPathData.pathMeshProperties.Thickness.min * currentLeafProgress;
            DrawStem(StemSpline, currentThicknessMax, currentThicknessMin, 0f, currentLeafProgress);
            //SproutLeaf(sproutLeafCount, currentLeafProgress);
        }
    }
    public void SproutB(float progress)
    {

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
        // Stem/Leaf/Flower의 성장을 각각 실행해야할듯
        // GrowStem ->
        // 1. SproutSpline에서 StemSpline으로의 점진적인 BezierCurve Lurp
        // 2. StemSpline에서 Mesh 생성 (시작, 끝 두께 및 상단 끝처리 값 필요)
        //GrowStem(progress);

        // GrowLeaves ->
        // 1. 처음난 새싹은 같이 자라면서 올라가고
        // 2. 특정시점(예: Growth 15%마다 1개씩, Stem의 특정 t position에 생겨나고 자람)
        //GrowLeaves(progress);

        //GrowBud ->
        // 1. Animation with Normalized Value;
        // 2. FlowerBudAnimationDurationValue = []
        //GrowBud(progress);
    }
    public void GrowB(float progress) { }
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
        currentAnimationState = FlowerAnimationStates.Grow;
        // 1. stemSpline 정의
        // 2. FlowerBud Initialize
    }

    #endregion

    #region Bloom Animations by Type
    /// <summary> 성장이 끝나면 시작 봉우리에서 피는 것 까지 에니메이션, 끝나면 활성화 (Grow를 Rj내고 끝내면 약5초 / else 약15초?) </summary>
    public void BloomA(float progress)
    {
        // Flower Petal, Pistils Normalize Value에 따라 열리거나 성장함
    }
    public void BloomB(float progress) { }
    public void BloomC(float progress) { }
    public void BloomD(float progress) { }
    public void BloomE(float progress) { }

    #endregion

    #region Fall Animations by Type
    /// <summary> 활성화 상태에서 비활성화 상태 사이에 천천히 꽃이 지는 에니메이션 </summary>
    public void FallA(float progress)
    {
        // Flower Petal, Pistils Normalize Value에 따라 닫히거나 쪼그라듬
        // Flower Petal 점진적으로 하나씩 이탈하여 Collision과 Gravity 값을 가지며 떨어짐
    }
    public void FallB(float progress) { }
    public void FallC(float progress) { }
    public void FallD(float progress) { }
    public void FallE(float progress) { }

    #endregion

    #region Bloom Animations by Type
    /// <summary> 꽃이 진 상태에서 봉우리까지의 상태로 에니메이션 (꽃이 고유하게 가진 Fall Time) </summary>
    public void RebloomA(float progress)
    {
        // 현재 상태에서 
    }
    public void RebloomB(float progress) { }
    public void RebloomC(float progress) { }
    public void RebloomD(float progress) { }
    public void RebloomE(float progress) { }

    #endregion

    #endregion

    #region Private Utility Functions

    private GameObject InitWithParent(string name, Transform parent)
    {
        GameObject o = new GameObject(name);
        o.transform.SetParent(parent);
        //o.transform.localPosition = Vector3.zero;
        return o;
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

        int vertexRingCount = (int)Math.Round(spline.Length * (endTime - startTime) / maxVertexDistance);
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


                Vector3 lastPoint = endPoint.location + radiusDistanceFromEndPoint - transform.position;
                Debug.Log(transform.name + lastPoint);
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
    private Vector3[] GenerateCircleVertices(Vector3 centerPoint, Vector3 tangent, int vertexCount, float width)
    {
        Vector3[] circleVertexList = new Vector3[vertexCount];
        float radius = width;
        Vector3 firstPoint = new Vector3(radius / 2, 0, 0);
        for (int i = 0; i < vertexCount; i++)
        {
            Vector3 rotationAxis;
            if (i == 0) rotationAxis = Vector3.up;
            else rotationAxis = tangent;

            Vector3 p = Quaternion.AngleAxis(360 / vertexCount * -i, rotationAxis) * firstPoint + centerPoint - transform.position;
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

    private Spline MakeSpline(GameObject o, Node[] nodes)
    {
        Spline spline = o.AddComponent<Spline>();
        foreach (Node n in nodes)
        {
            spline.AddNode(new SplineNode(n.position, n.handleOut));
        }
        return spline;
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
