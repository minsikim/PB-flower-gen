using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour
{


    private float timeFromLastState = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Control time from state;
        timeFromLastState += Time.deltaTime;

        //Control Actions
        if (Input.GetKeyUp(KeyCode.I)) Init();
    }

    void Init()
    {

    }

    void Grow()
    {

    }
    void Bloom()
    {

    }
    void Fall()
    {

    }
    void Rebloom()
    {

    }
}
