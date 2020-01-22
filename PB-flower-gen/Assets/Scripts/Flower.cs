using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

[Serializable]
public class Flower : MonoBehaviour
{
    #region Information
    // ---- 컨트롤러 클래스에서 수행 ----
    // 1. 위치를 잡는다
    // 2. 해당 위치에 Instantiate
    #endregion

    #region Basic Serializable Info
    [LabelOverride("Flower Form Data")]
    [SerializeField] FlowerFormData data;
    #endregion

    #region Private Fields
    //Initial Datas
    private DateTime initialTime;

    //Flower Data
    private string flowerName;
    private string description;

    private PlantFormType plantFormType;
    private FlowerFormType flowerFormType;

    private GameObject PetalPrefab;
    private GameObject PistilPrefab;
    private GameObject LeafPrefab;

    //Randomize Data
    private RandomPath SproutPath;
    private RandomPath StemPath;
    private RandomPath BranchPath;

    //Distributed Path Data
    private Vector3[] sproutBPath;
    private Vector3[] stemBPath;
    private Vector3[] branchBPath;

    //Stem Data
    private BezierPath mainStemBPath;
    private BezierPath[] branchBPaths;

    //Leaf Position Data
    private Vector2Int LeafCountRange;
    private Vector2 LeafPositionRange;
    private Vector2 LeafPositionRandomValue;

    //Leaf Positions
    private List<Vector3> LeafPositionList;
    private List<float> LeafRotationList;

    //Children Data

    //private Game
    private GameObject Leaves;

    //Animation State
    private FlowerAnimationStates currentAnimationState;
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

    #region Basic Functions

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
        Sprout  =   SproutMethods[(int)plantFormType];
        Grow    =   GrowMethods[(int)plantFormType];
        Bloom   =   BloomMethods[(int)plantFormType];
        Fall    =   FallMethods[(int)plantFormType];
        Rebloom =   RebloomMethods[(int)plantFormType];
    }

    void InitializeFlowerData()
    {
        // FlowerFormData의 DATA를 모두 적절한 방식으로 배당

        initialTime = DateTime.Now;

        flowerName  = data.flowerName;
        description = data.description;

        plantFormType   = data.plantFormType;
        flowerFormType  = data.flowerFormType;

        PetalPrefab     = data.PetalPrefab;
        PistilPrefab    = data.PistilPrefab;
        LeafPrefab      = data.LeafPrefab;

        SproutPath  = data.SproutPath;
        StemPath    = data.StemPath;
        BranchPath  = data.BranchPath;

        sproutBPath = DistributePath(SproutPath);
        stemBPath   = DistributePath(SproutPath);
        branchBPath = DistributePath(SproutPath);

        LeafCountRange          = data.LeafCountRange;
        LeafPositionRange       = data.LeafPositionRange;
        LeafPositionRandomValue = data.LeafPositionRandomValue;

        LeafPositionList = DistributeLeafPositions(LeafCountRange, LeafPositionRange, LeafPositionRandomValue);
        LeafRotationList = DistributeLeafRotations(LeafPositionList.Count);

        SproutAnimationDuration     = data.SproutAnimationDuration;
        GrowAniamtionDuration       = data.GrowAniamtionDuration;
        BloomAnimationDuration      = data.BloomAnimationDuration;
        FallAnimationDuration       = data.FallAnimationDuration;
        RebloomAnimationDuration    = data.RebloomAnimationDuration;

        durationList = new List<float>()
        {
            SproutAnimationDuration,
            GrowAniamtionDuration,
            BloomAnimationDuration,
            FallAnimationDuration,
            RebloomAnimationDuration
        };

        durantionCycle = BloomAnimationDuration + FallAnimationDuration + RebloomAnimationDuration;

        SaveFlowerData();

        SetAnimationState(FlowerAnimationStates.Sprout);
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
    #endregion

    #region Public Animation Functions

    #region Utility Functions for Animation

    /// <summary>
    /// Calculates current Progression of current State
    /// </summary>
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
    /// <summary>
    /// Only call on end of Animation(when progression rate is >= 1
    /// </summary>
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

    #region Sprout Animations by Type

    /// <summary> 처음 심었을때 새싹까지 Animation Controller의 OnStateUpdate에서 실행됨(약5초) </summary>
    public void SproutA(float progress)
    {
        // 1. SproutSpline 값을 받아서 현재 시각에 따라 Mesh 만듦(Animation)
        // 2. FlowerData에서 정해진 위치와 각도를 받아와 그에 따라 새싹이 남
        //    -> 예시: SproutAnimationDurationValues = [ [ 0f, 0.5f ], [ 0.5f, 0.8f ], [ 0.7f, 1.0f ] ]
        // 3. SproutParticleAnimation 실행
        // 4. Finish Init -> Trigger Grow State
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
        GrowStem(progress);

        // GrowLeaves ->
        // 1. 처음난 새싹은 같이 자라면서 올라가고
        // 2. 특정시점(예: Growth 15%마다 1개씩, Stem의 특정 t position에 생겨나고 자람)
        GrowLeaves(progress);

        //GrowBud ->
        // 1. Animation with Normalized Value;
        // 2. FlowerBudAnimationDurationValue = []
        GrowBud(progress);
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
        KillFlower(progress);
        GrowBud(progress);
    }
    public void RebloomB(float progress) { }
    public void RebloomC(float progress) { }
    public void RebloomD(float progress) { }
    public void RebloomE(float progress) { }

    #endregion

    #endregion

    #region Private Functions
    /// <summary>
    /// Private Function for Stem Growth, Used in public function <see cref="Grow()"/>
    /// </summary>
    private void GrowStem(float progress)
    {

    }
    /// <summary>
    /// Private Function for Leaf Growth, Used in public function <see cref="Grow()"/>
    /// </summary>
    private void GrowLeaves(float progress)
    {

    }
    /// <summary>
    /// Private Function for Flower Growth, Used in public function <see cref="Grow()"/>
    /// </summary>
    private void GrowBud(float progress)
    {

    }
    /// <summary>
    /// Private Function for Dissolve Flower, Used in public function <see cref="Rebloom()"/>
    /// </summary>
    private void KillFlower(float progress)
    {

    }

    /// <summary>
    /// Used for Fixing Random Values when a FlowerPrefab is Instantiated
    /// </summary>
    /// <param name="RandomizablePathData"></param>
    /// <returns></returns>
    private Vector3[] DistributePath(RandomPath RandomizablePathData)
    {
        Vector3[] tempPointList = RandomizablePathData.pathPoints;
        foreach(RandomPoint rp in RandomizablePathData.randomPoints)
        {
            tempPointList[rp.randomPoint][(int)rp.randomAxis] = 
                UnityEngine.Random.Range(rp.randomRange.min, rp.randomRange.max);
        }
        return tempPointList;
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
    private List<Vector3> DistributeLeafPositions( Vector2Int countRange, Vector2 positionRange, Vector2 positionRandomValue)
    {
        List<Vector3> positions = new List<Vector3>();


        return positions;
    }
    private List<float> DistributeLeafRotations(int flowerCount)
    {
        return new List<float>();
    }
    #endregion
}
