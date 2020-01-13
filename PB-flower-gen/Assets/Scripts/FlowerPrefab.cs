using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerPrefab : MonoBehaviour
{
    [SerializeField] public GameObject PetalPrefab;

    [Header("Flower Part")]
    [SerializeField] public int PetalAmount;
    [SerializeField] public int PetalLayers;
    [Range(0.0f, 0.2f)]
    [SerializeField] public float LayerDifference;

    private GameObject Petals;
    private GameObject petalLayer;
    private bool timeState = false;
    void Start()
    {
        Petals = transform.GetChild(0).GetChild(0).gameObject;
        for (int i = 0; i < PetalLayers; ++i)
        {
            GameObject petalLayer = new GameObject("PetalLayer" + i);
            petalLayer.transform.SetParent(Petals.transform);
            petalLayer.transform.localPosition = new Vector3(0, 0, 0);

            for (int j = 0; j < PetalAmount; ++j)
            {
                GameObject petal = Instantiate(PetalPrefab, petalLayer.transform);
                petal.transform.Rotate(new Vector3(0, (360 / PetalAmount * j) + i * 30, 0));
                petal.GetComponent<Animator>().SetFloat("Time", 0.0f + ((PetalLayers-i) * LayerDifference) );
            }
        }
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
}
