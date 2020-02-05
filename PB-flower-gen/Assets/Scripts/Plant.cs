using SplineMesh;
using System;
using System.Collections.Generic;
using UnityEngine;

//TODO !!! GameObject들 만들때 LocalData들 다 잡아넣어라

[Serializable]
public class Plant : MonoBehaviour
{

    #region Basic Serializable Info

    [SerializeField] PlantFormData data;
    //TODO 각 프리팹에 달아라 통일은 못시킬듯하다
    public RuntimeAnimatorController NormalizedTimeAnimationController;

    #endregion

    #region Private Fields

    private PlantLocalData _d;

    private GameObject PetalPrefab;
    private GameObject PistilPrefab;
    private GameObject LeafPrefab;
    private GameObject SproutParticles;

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

    //Splines
    private Spline CurrentMainSpline;


    #endregion

    #region Animation Methods

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
        if (true)
        {
            _d = new PlantLocalData(data);
        }
        else
        {
            _d = loadFlowerData();
        }
        AddAnimationMethodsToList();
        DistributeAnimationMethodsByType();
        DistributeGameObjectsByType(_d.PlantFormType);
        transform.RotateAround(transform.position, Vector3.up, _d.Rotation);
    }

    void Start()
    {

    }

    private void Update()
    {
        
    }
    #endregion

    #region Data Managing Functions

    void DistributeGameObjectsByType(PlantFormType type)
    {
        //SetPrefabs
        PetalPrefab         = Util.SetPrefab(_d.PetalPrefab);
        PistilPrefab        = Util.SetPrefab(_d.PistilPrefab); ;
        LeafPrefab          = Util.SetPrefab(_d.LeafPrefab);;
        SproutParticles     = Util.SetPrefab(_d.SproutParticles);;

        LeavesList = new List<GameObject>();
        BranchesList = new List<GameObject>();
        FlowersList = new List<GameObject>();
        PetalsList = new List<GameObject>();

        switch (type)
        {
            case PlantFormType.A:
                Stem    = Util.InitWithParent("Stem", transform);
                Leaves  = Util.InitWithParent("Leaves", Stem.transform);
                Flower  = Util.InitWithParent("Flower", Stem.transform);

                InitStemSplineMesh(Stem, _d.MainStem);
                InitFlowerChildren(Flower, _d.Flower, _d.Petals);
                InitLeavesChildren(Leaves, _d.Leaves);

                break;
            case PlantFormType.B:
                Stem = Util.InitWithParent("Stem", transform);
                Leaves = Util.InitWithParent("Leaves", Stem.transform);
                Flower = Util.InitWithParent("Flower", Stem.transform);

                CurrentMainSpline = Util.InitSpline(Stem, _d.MainStem.SproutNodes);
                InitMesh(Stem);
                ApplyColorToMesh(Stem, _d.MainStem.Color);

                InitFlowerChildren(Flower, _d.Flower, _d.Petals);
                InitLeavesChildren(Leaves, _d.Leaves);

                break;
            case PlantFormType.C:
                Stem = Util.InitWithParent("Stem", transform);
                StemBranches = Util.InitWithParent("StemBranches", Stem.transform);
                Flower = Util.InitWithParent("Flower", Stem.transform);

                break;
            case PlantFormType.D:
                Stem = Util.InitWithParent("Stem", transform);
                Branches = Util.InitWithParent("Branches", Stem.transform);
                Leaves = Util.InitWithParent("Leaves", Stem.transform);

                break;
            case PlantFormType.E:
                Tree = Util.InitWithParent("Tree", transform);
                Branches = Util.InitWithParent("Branches", Tree.transform);

                break;
            default:
                Console.WriteLine("PlantFormType is Broken");
                break;
        }
    }

    void SavePlantData()
    {

    }
    private PlantLocalData loadFlowerData()
    {
        //현재 시각과 대비해서 현재 스테잇을 정하는 것이 가장 중요
        return new PlantLocalData();
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
        Sprout  = SproutMethods [(int)_d.PlantFormType];
        Grow    = GrowMethods   [(int)_d.PlantFormType];
        Bloom   = BloomMethods  [(int)_d.PlantFormType];
        Fall    = FallMethods   [(int)_d.PlantFormType];
        Rebloom = RebloomMethods[(int)_d.PlantFormType];
    }

    #endregion

    #region Random Data Distribution

    private void InitStemSplineMesh(GameObject stemObject, StemLocalData stemData)
    {
        stemObject.AddComponent<StemLocalData>();
        stemData = stemObject.GetComponent<StemLocalData>().AssignValues(stemData);
        CurrentMainSpline = Util.InitSpline(Stem, stemData.SproutNodes);
        InitMesh(Stem);
        ApplyColorToMesh(Stem, stemData.Color);
    }
    private void InitFlowerChildren(GameObject flowerObject, FlowerLocalData flowerData, List<PetalLocalData> petalDatas)
    {
        flowerObject.AddComponent<FlowerLocalData>();
        flowerData = flowerObject.GetComponent<FlowerLocalData>().AssignValues(flowerData);
        GameObject Pistil = Util.InitWithParent(PistilPrefab, flowerObject.transform);
        GameObject Petals = Util.InitWithParent("Petals", flowerObject.transform);

        List<GameObject> tempPetalLayers = new List<GameObject>();
        for (int i = 0; i < flowerData.PetalLayerCount; ++i)
        {
            GameObject petalLayer = Util.InitWithParent("PetalLayer" + i, Petals.transform);
            tempPetalLayers.Add(petalLayer);
        }

        for (int i = 0; i < petalDatas.Count; ++i)
        {
            GameObject petal = Util.InitWithParent(PetalPrefab, tempPetalLayers[petalDatas[i].PetalLayer].transform);
            petal.AddComponent<PetalLocalData>();
            PetalLocalData petalData  = petal.GetComponent<PetalLocalData>().AssignValues(petalDatas[i]);
            petal.transform.Rotate(new Vector3(0, petalData.Rotation, 0));
            petal.GetComponent<Animator>().SetFloat("Time", petalData.StartTime);
            petal.transform.Find("Plane").GetComponent<SkinnedMeshRenderer>().material.color = petalData.Color;

            PetalsList.Add(petal);
        }

        UpdateTransformOnSpline(flowerObject, CurrentMainSpline, 1f, 0f, 0f);
        flowerObject.SetActive(false);
    }
    private void InitLeavesChildren(GameObject Leaves, List<LeafLocalData> LeafDatas)
    {
        for (int i = 0; i < LeafDatas.Count; i++)
        {
            GameObject leaf = Instantiate(LeafPrefab, Leaves.transform);
            leaf.AddComponent<LeafLocalData>();
            LeafLocalData leafData = leaf.GetComponent<LeafLocalData>().AssignValues(LeafDatas[i]);
            UpdateTransformOnSpline(leaf, CurrentMainSpline, leafData.SproutPosition, leafData.Rotation, 0f);
            LeavesList.Add(leaf);
        }
    }
    #endregion

    #region Public Animation Functions

    #region Animation Progress and Animation State

    public FlowerAnimationState GetCurrentState()
    {
        return _d.CurrentAnimationState;
    }
    public void SetAnimationState(FlowerAnimationState state)
    {
        _d.CurrentAnimationState = state;
        _d.LastStateChangedTime = DateTime.Now;
    }
    public float GetProgression()
    {
        float currentStateProgressionSeconds = (float)(DateTime.Now - _d.LastStateChangedTime).TotalSeconds;
        float currentStateAnimationDuration = _d.AnimationDurationList[(int)_d.CurrentAnimationState];
        float progression = currentStateProgressionSeconds / currentStateAnimationDuration;

        return progression;
    }
    public void SwitchToNextState()
    {
        if (_d.CurrentAnimationState != FlowerAnimationState.Rebloom)
        {
            _d.CurrentAnimationState = _d.CurrentAnimationState.Next();
        }
        else
        {
            _d.CurrentAnimationState = FlowerAnimationState.Bloom;
        }
        _d.LastStateChangedTime = DateTime.Now;
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
        //TODO !!!! AddComponent를 이미 존재하는 데이타로 하게끔
        //TODO Progress Scriptable Object 에서 설정하게끔 바꿔라
        float[] stemProgress = new float[2] { 0f , .8f };
        float[] leafProgress = new float[2] { .5f , 1f };

        float currentStemProgress = (progress - stemProgress[0]) / (stemProgress[1] - stemProgress[0]);
        float currentLeafProgress = (progress - leafProgress[0]) / (leafProgress[1] - leafProgress[0]);

        if( 0f <= currentStemProgress && currentStemProgress <= 1f)
        {
            float currentThicknessMin = _d.MainStem.SproutThickness.x * currentStemProgress;
            float currentThicknessMax = _d.MainStem.SproutThickness.y* currentStemProgress;
            DrawStem(CurrentMainSpline, currentThicknessMax, currentThicknessMin, 0f, currentStemProgress);
        }

        if (0f <= currentLeafProgress && currentLeafProgress <= 1f)
        {
            for (int i = 0; i < LeavesList.Count; i++)
            {
                LeafLocalData leafData = _d.Leaves[i];
                float currentScale = leafData.SproutScale * currentLeafProgress;
                UpdateTransformOnSpline(LeavesList[i], CurrentMainSpline, leafData.SproutPosition * progress, leafData.Rotation, currentScale);
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
        ////TODO Progress Scriptable Object 에서 설정하게끔 바꿔라
        ////Stem Grow Progress , Stem Mesh Progress, Bug Growth Progress, Leaf Growth Progress 
        //float[] stemProgress = new float[2] { 0f, 1f };
        ////float[] leafProgress = new float[2] { .5f, 1f };

        //float currentStemProgress = (progress - stemProgress[0]) / (stemProgress[1] - stemProgress[0]);
        ////float currentLeafProgress = (progress - leafProgress[0]) / (leafProgress[1] - leafProgress[0]);

        //if (0f <= currentStemProgress && currentStemProgress <= 1f)
        //{
        //    Node[] lerpedNodeList = LerpNodeList(SproutPathNodes, StemPathNodes, CurrentMainSpline, progress);
        //    CurrentMainSpline = UpdateNodeToSpline(CurrentMainSpline, lerpedNodeList);
        //    float currentThicknessMax = (StemPathData.pathMeshProperties.Thickness.max - SproutPathData.pathMeshProperties.Thickness.max) * currentStemProgress + SproutPathData.pathMeshProperties.Thickness.max;
        //    float currentThicknessMin = (StemPathData.pathMeshProperties.Thickness.min = SproutPathData.pathMeshProperties.Thickness.min) * currentStemProgress + SproutPathData.pathMeshProperties.Thickness.min;
        //    DrawStem(CurrentMainSpline, currentThicknessMax, currentThicknessMin, 0f, 1f);
        //    UpdateTransformOnSpline(Flower, CurrentMainSpline, 1f, 0f, progress);
        //}

        //foreach (GameObject leaf in LeavesList)
        //{
        //    LeafLocalData leafData = leaf.GetComponent<LeafLocalData>();
        //    float positionTime = 0f;
        //    if (leafGrowRelation == LeafGrowRelation.Same)
        //    {
        //        positionTime = leafData.FinalPosition;
        //    }
        //    else
        //    {
        //        positionTime = progress * (leafData.FinalPosition - leafData.SproutPosition) + leafData.SproutPosition;
        //    }
        //    UpdateTransformOnSpline(
        //        leaf, CurrentMainSpline,
        //        positionTime,
        //        leafData.Rotation, 
        //        progress * (1f - leafData.SproutScale) + leafData.SproutScale);
        //}
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
            float tempFallTime = Util.RandomRange(0f, 5f);
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



    #region Private Animation Functions
    private void UpdateTransformOnSpline(GameObject objectToUpdate, Spline spline, float positionTime, float yEulerAngle, float localScale)
    {
        CurveSample p = Util.GetSampleAt(spline, positionTime);
        UpdatePositionOnSpline(objectToUpdate, p.location);
        UpdateRotationOnSpline(objectToUpdate, yEulerAngle, p.tangent);
        UpdateLocalScale(objectToUpdate, localScale);
        objectToUpdate.transform.RotateAround(transform.position, Vector3.up, _d.Rotation);
    }
    private void UpdateTransformOnSpline(GameObject objectToUpdate, Spline spline, float positionTime)
    {
        CurveSample p = Util.GetSampleAt(spline, positionTime);
        UpdatePositionOnSpline(objectToUpdate, p.location);
        UpdateRotationOnSpline(objectToUpdate, 0f, p.tangent);
    }
    private void UpdatePositionOnSpline(GameObject objectToUpdate, Vector3 p)
    {
        objectToUpdate.transform.position = p + transform.position;
    }
    private void UpdatePositionOnSpline(GameObject objectToUpdate, Spline spline, float positionTime)
    {
        CurveSample p = Util.GetSampleAt(spline, positionTime);
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
        CurveSample p = Util.GetSampleAt(spline, position);
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
        obj.GetComponent<Animator>().SetFloat(floatName, progress);
    }
    public void InvokeAnimation(string methodName, float seconds)
    {
        try
        {
            Invoke(methodName, seconds);
        }
        catch (MissingMethodException e)
        {
            throw new MissingMethodException("Wrong Method name to Invoke", e);
        }
    }
    #endregion

    #region Mesh Functions
    public static void InitMesh(GameObject obj)
    {
        obj.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
        meshFilter.mesh = new Mesh();
    }
    public static void ApplyColorToMesh(GameObject o, Color c)
    {
        MeshRenderer renderer = o.GetComponent<MeshRenderer>();
        renderer.material.color = c;
    }
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
            CurveSample centerSamplePoint = Util.GetSampleAt(spline, startTime + (vertexInterval * i));

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
                CurveSample endPoint = Util.GetSampleAt(spline, endTime);
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

}
