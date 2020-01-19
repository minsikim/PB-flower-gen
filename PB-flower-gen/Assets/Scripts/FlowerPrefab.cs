using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;


public class FlowerPrefab : MonoBehaviour
{
   
    [Header("Flower Part")]
    [SerializeField] public GameObject PetalPrefab;
    [SerializeField] public int PetalAmount;
    [SerializeField] public int PetalLayers;
    [Range(0.0f, 0.2f)]
    [SerializeField] public float OpenLayerDifference;
    [Range(0.0f, 0.2f)]
    [SerializeField] public float CloseLayerDifference;
    [Range(0.0f, 60f)]
    [SerializeField] public float BloomSpeed = 3f;

    [Header("Stem Part")]
    [SerializeField] public GameObject StemSpline;
    //#TODO
    [Range(0.0f, 0.2f)]
    [SerializeField] public float RandomizeSpline;

    [Header("Leaves")]
    [SerializeField] public GameObject LeafPrefab;
    [Range(1,15)]
    [SerializeField] public int LeafAmount;

    private GameObject Petals;
    private GameObject Leaves;
    private GameObject petalLayer;
    private VertexPath stemPath;
    private VertexPath currPath;
    private int lastLeafAmount;
    private List<Vector3> lastLeafPosition = new List<Vector3>();
    private List<float[]> lastLeafRotation = new List<float[]>();
    private bool timeState = false;
    private bool growState = false;
    void Start()
    {
        Leaves = transform.Find("Stem/Leaves").gameObject;
        stemPath = StemSpline.GetComponent<StemGenerator>().pathCreator.path;
        GeneratePetals();
        GenerateLeaves();
        UpdateFlowerAngle();
    }

    // Update is called once per frame
    void Update()
    {
        currPath = StemSpline.GetComponent<StemGenerator>().GetVPath();

        if (Input.GetKeyDown(KeyCode.B)) ToggleBloomState();
        if (Input.GetKeyDown(KeyCode.G)) growState = !growState;

        BloomFlower();
        GrowFlower();

        if (LeafAmount != lastLeafAmount) GenerateLeaves();
        
    }

    public void ToggleBloomState()
    {
        timeState = !timeState;
    }
    public void ToggleGrowState()
    {
        growState = !growState;
    }
    private void BloomFlower()
    {
        if (timeState == true)
        {
            for (int i = 0; i < Petals.transform.childCount; i++)
            {
                for (int j = 0; j < Petals.transform.GetChild(i).childCount; j++)
                {
                    Animator anim = Petals.transform.GetChild(i).GetChild(j).GetComponent<Animator>();
                    if (anim.GetFloat("Time") < 1.0f - (i * OpenLayerDifference))
                    {
                        anim.SetFloat("Time", anim.GetFloat("Time") + 1f / BloomSpeed * Time.deltaTime);
                    }
                }
            }
        }
        if (timeState == false)
        {
            for (int i = 0; i < Petals.transform.childCount; i++)
            {
                for (int j = 0; j < Petals.transform.GetChild(i).childCount; j++)
                {
                    Animator anim = Petals.transform.GetChild(i).GetChild(j).GetComponent<Animator>();
                    if (anim.GetFloat("Time") > 0.0f + ((Petals.transform.childCount - i) * CloseLayerDifference))
                    {
                        anim.SetFloat("Time", anim.GetFloat("Time") - 1f / BloomSpeed * Time.deltaTime);
                    }
                }
            }
        }
    }
    private void GrowFlower()
    {
        if(growState == true)
        {
            //Grow Bezier Curve
            Vector3 currV = StemSpline.GetComponent<PathCreator>().bezierPath.GetPoint(3);
            StemSpline.GetComponent<PathCreator>().bezierPath.MovePoint(3, currV + Vector3.up * 5f / BloomSpeed * Time.deltaTime);
            //Get current Vertex Path
            if (currPath.length > 10) growState = false;

            //Generate Stem Mesh
            StemSpline.GetComponent<StemGenerator>().GenerateStemMesh();

            //Update path to current Path
            stemPath = StemSpline.GetComponent<StemGenerator>().GetVPath();

            //Update rest
            UpdateLeaves();
            UpdateFlowerAngle();
        }
    }
    private void GeneratePetals()
    {
        Petals = transform.Find("Flower/Petals").gameObject;
        for (int i = 0; i < PetalLayers; ++i)
        {
            GameObject petalLayer = new GameObject("PetalLayer" + i);
            petalLayer.transform.SetParent(Petals.transform);
            petalLayer.transform.localPosition = new Vector3(0, 0, 0);

            for (int j = 0; j < PetalAmount; ++j)
            {
                GameObject petal = Instantiate(PetalPrefab, petalLayer.transform);
                petal.transform.Rotate(new Vector3(0, (360 / PetalAmount * j) + i * 30, 0));
                petal.GetComponent<Animator>().SetFloat("Time", 0.0f + ((PetalLayers - i) * CloseLayerDifference));
            }
        }
    }
    private void GenerateLeaves()
    {
        lastLeafPosition.Clear();
        lastLeafRotation.Clear();
        foreach (Transform child in Leaves.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        
        for (int i = 0; i < LeafAmount; ++i)
        {
            GameObject leaf = Instantiate(LeafPrefab, Leaves.transform);

            Vector3 tempPosition = stemPath.GetPointAtDistance(1f / LeafAmount * i * 10);
            Vector3 tempRotation = stemPath.GetNormalAtDistance(1f / LeafAmount * i * 10);

            leaf.transform.position = tempPosition;
            float normalAngle = Vector3.Angle(tempRotation, Vector3.right);
            normalAngle = tempRotation.y > 0 ? normalAngle : -normalAngle;
            float[] currAngle = new float[] { Random.Range(0, 360), normalAngle };
            leaf.transform.Rotate(new Vector3(0, currAngle[0], currAngle[1]));

            lastLeafPosition.Add(tempPosition);
            lastLeafRotation.Add(currAngle);
        }
        lastLeafAmount = LeafAmount;
        stemPath = StemSpline.GetComponent<PathCreator>().path;
    }
    private void UpdateLeaves()
    {

        for (int i = 0; i < LeafAmount; ++i)
        {
            GameObject leaf = Leaves.transform.GetChild(i).gameObject;

            Vector3 tempPosition = stemPath.GetPointAtDistance(1f / LeafAmount * i * 10);
            Vector3 tempRotation = stemPath.GetNormalAtDistance(1f / LeafAmount * i * 10);

            leaf.transform.position = tempPosition;
            float normalAngle = Vector3.Angle(tempRotation, Vector3.right);
            normalAngle = tempRotation.y > 0 ? normalAngle : -normalAngle;
            leaf.transform.rotation.SetEulerAngles(0, 0, normalAngle - leaf.transform.localRotation.z);
        }
        lastLeafAmount = LeafAmount;
    }

    private void UpdateFlowerAngle()
    {
        Vector3 rotationVector = stemPath.GetPoint(stemPath.NumPoints-1) - stemPath.GetPoint(stemPath.NumPoints-2);
        transform.Find("Flower").rotation = Quaternion.FromToRotation(Vector3.up, rotationVector);
        transform.Find("Flower").position = stemPath.GetPoint(stemPath.NumPoints - 1);
    }
}
