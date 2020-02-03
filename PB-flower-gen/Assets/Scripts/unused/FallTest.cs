using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Example();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Example()
    {
        //Physics.gravity = new Vector3(0, -1.0F, 0);
        Debug.Log(Physics.gravity);
    }
}
