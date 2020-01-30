using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafController : MonoBehaviour
{
    [SerializeField] private float growSpeed = 3f;
    private bool growState = true;
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if(growState == true)
        {
            transform.localScale = transform.localScale + (Vector3.one / growSpeed * Time.deltaTime);
            if (transform.localScale.x > 1f) growState = false;
        }
    }

    void SetInitialScale(float scale)
    {
        transform.localScale = Vector3.one * scale;
    }
}
