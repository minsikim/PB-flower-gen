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
    [SerializeField] public float LayerDifference;

    [Header("Stem Part")]
    [SerializeField] public GameObject StemSpline;
    //#TODO
    [Range(0.0f, 0.2f)]
    [SerializeField] public float RandomizeSpline;

    [Header("Leaves")]
    [SerializeField] public GameObject LeafPrefab;
    [SerializeField] public int LeafAmount;

    private GameObject Petals;
    private GameObject Leaves;
    private GameObject petalLayer;
    private bool timeState = false;
    void Start()
    {
        GeneratePetals();
        GenerateLeaves();
//#TODO
        GenerateStem();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            timeState = !timeState;
        }
        if (timeState == true)
        {
            for (int i = 0; i < Petals.transform.childCount; i++)
            {
                for (int j = 0; j < Petals.transform.GetChild(i).childCount; j++)
                {
                    Animator anim = Petals.transform.GetChild(i).GetChild(j).GetComponent<Animator>();
                    if (anim.GetFloat("Time") < 1.0f - (i * LayerDifference))
                    {
                        anim.SetFloat("Time", anim.GetFloat("Time") + 0.01f);
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
                    if (anim.GetFloat("Time") > 0.0f + ((Petals.transform.childCount - i) * LayerDifference))
                    {
                        anim.SetFloat("Time", anim.GetFloat("Time") - 0.01f);
                    }
                }
            }
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
                petal.GetComponent<Animator>().SetFloat("Time", 0.0f + ((PetalLayers - i) * LayerDifference));
            }
        }
    }
    private void GenerateLeaves()
    {
        Leaves = transform.Find("Stem/Leaves").gameObject;
        VertexPath stemPath = StemSpline.GetComponent<StemGenerator>().pathCreator.path;
        for (int i = 0; i < LeafAmount; ++i)
        {
            GameObject leaf = Instantiate(LeafPrefab, Leaves.transform);

            Vector3 tempPosition = stemPath.GetPointAtDistance(1f / LeafAmount * i * 10);
            Vector3 tempRotation = stemPath.GetNormalAtDistance(1f / LeafAmount * i * 10);
            
            //Quaternion tempRotation = stemPath.GetRotationAtDistance(1f / LeafAmount * i * 10);
            leaf.transform.position = tempPosition;
            float normalAngle = Vector3.Angle(tempRotation, Vector3.right);
            normalAngle = tempRotation.y > 0 ? normalAngle : -normalAngle;
            leaf.transform.Rotate(new Vector3(0, Random.Range(0, 360), normalAngle));
        }
    }
    private void GenerateStem()
    {

    }
}
