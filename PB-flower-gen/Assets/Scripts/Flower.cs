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

    //Stem Data
    private BezierPath mainStemBPath;
    private BezierPath[] branchBPaths;

    //Children Data
    //private Game
    private GameObject Leaves;

    //Animation Process Trackers
    private float initAnimationPosition = 0f;
    private float growAnimationPosition = 0f;
    private float bloomAnimationPosition = 0f;
    private float fallAnimationPosition = 0f;
    private float rebloomAnimationPosition = 0f;
    #endregion

    #region Life Cycles
    void Start()
    {
        // 1. Distribute Data to private variables
        InitializeFlowerData();

        // 2. PlantType과 FlowerType에 따라서 전체 형태에 대한 데이터 저장(FlowerFormData)
        SaveInitialFlowerData();
    }
    #endregion

    #region Basic Functions
    void InitializeFlowerData()
    {
        initialTime = DateTime.Now;

        flowerName = data.flowerName;
        description = data.description;

        plantFormType = data.plantFormType;
        flowerFormType = data.flowerFormType;

        PetalPrefab = data.PetalPrefab;
        PistilPrefab = data.PistilPrefab;
        LeafPrefab = data.LeafPrefab;

        SproutPath = data.SproutPath;
        StemPath = data.StemPath;
        BranchPath = data.BranchPath;

        

    //    -> FlowerFormData 저장 필요 정보
    //       MainStemSpline, BranchSpline의 갯수/위치/Y축각도(Semi-Random)
    //       Spline들의 조정값(보통 한쪽+모든 핸들 고정, 마지막 포인트 상하 랜덤)
    //       Leaves의 각 Leaf 갯수/위치/Y축각도(Semi-Random)
    //       SproutSpline, SproutAnimationDurationValues 등 받아옴
}
    void UpdateFlowerData()
    {
        
    }
    /// <summary>
    /// Save Flower Data to Save Data
    /// </summary>
    void SaveInitialFlowerData()
    {

    }
    #endregion

    #region Public Animation Functions
    /// <summary> 처음 심었을때 새싹까지 Animation Controller의 OnStateUpdate에서 실행됨(약5초) </summary>
    public void Sprout()
    {
        // 1. SproutSpline 값을 받아서 현재 시각에 따라 Mesh 만듦(Animation)
        // 2. FlowerData에서 정해진 위치와 각도를 받아와 그에 따라 새싹이 남
        //    -> 예시: SproutAnimationDurationValues = [ [ 0f, 0.5f ], [ 0.5f, 0.8f ], [ 0.7f, 1.0f ] ]
        // 3. SproutParticleAnimation 실행
        // 4. Finish Init -> Trigger Grow State
    }
    /// <summary> Grow 단계 이전에 처리할 것들 </summary>
    public void OnGrowStart()
    {
        // 1. stemSpline 정의
        // 2. FlowerBud Initialize
    }
    /// <summary> 새싹에서 봉우리까지의 성장 (약1시간) </summary>
    public void Grow()
    {
        // Stem/Leaf/Flower의 성장을 각각 실행해야할듯
        // GrowStem ->
        // 1. SproutSpline에서 StemSpline으로의 점진적인 BezierCurve Lurp
        // 2. StemSpline에서 Mesh 생성 (시작, 끝 두께 및 상단 끝처리 값 필요)
        GrowStem();

        // GrowLeaves ->
        // 1. 처음난 새싹은 같이 자라면서 올라가고
        // 2. 특정시점(예: Growth 15%마다 1개씩, Stem의 특정 t position에 생겨나고 자람)
        GrowLeaves();

        //GrowBud ->
        // 1. Animation with Normalized Value;
        // 2. FlowerBudAnimationDurationValue = []
        GrowBud();
    }

    /// <summary> 성장이 끝나면 시작 봉우리에서 피는 것 까지 에니메이션, 끝나면 활성화 (Grow를 Rj내고 끝내면 약5초 / else 약15초?) </summary>
    public void Bloom()
    {
        // Flower Petal, Pistils Normalize Value에 따라 열리거나 성장함
    }

    /// <summary> 활성화 상태에서 비활성화 상태 사이에 천천히 꽃이 지는 에니메이션 </summary>
    public void Fall()
    {
        // Flower Petal, Pistils Normalize Value에 따라 닫히거나 쪼그라듬
        // Flower Petal 점진적으로 하나씩 이탈하여 Collision과 Gravity 값을 가지며 떨어짐
    }

    /// <summary> 꽃이 진 상태에서 봉우리까지의 상태로 에니메이션 (꽃이 고유하게 가진 Fall Time) </summary>
    public void Rebloom()
    {
        // 현재 상태에서 
        KillFlower();
        GrowBud();
    }
    #endregion

    #region Private Functions
    /// <summary>
    /// Private Function for Stem Growth, Used in public function <see cref="Grow()"/>
    /// </summary>
    private void GrowStem()
    {

    }
    /// <summary>
    /// Private Function for Leaf Growth, Used in public function <see cref="Grow()"/>
    /// </summary>
    private void GrowLeaves()
    {

    }
    /// <summary>
    /// Private Function for Flower Growth, Used in public function <see cref="Grow()"/>
    /// </summary>
    private void GrowBud()
    {

    }
    /// <summary>
    /// Private Function for Dissolve Flower, Used in public function <see cref="Rebloom()"/>
    /// </summary>
    private void KillFlower()
    {

    }
    #endregion
}
