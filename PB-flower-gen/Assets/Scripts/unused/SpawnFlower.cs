using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnFlower : MonoBehaviour
{
    [SerializeField] GameObject SpawnPrefab;
    [SerializeField] GameObject FlowerCount;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Instantiate(SpawnPrefab);
                SpawnPrefab.transform.position = hit.point;

                int currentFlowerCount = Convert.ToInt32(FlowerCount.GetComponent<Text>().text);
                FlowerCount.GetComponent<Text>().text = (currentFlowerCount + 1).ToString();
            }

        }
    }

    public void SetGrowState()
    {
        GameObject[] Flowers = GameObject.FindGameObjectsWithTag("Flower");
        foreach(GameObject f in Flowers)
        {
            f.GetComponent<FlowerPrefab>().ToggleGrowState();
        }
    }
    public void SetBloomState()
    {
        GameObject[] Flowers = GameObject.FindGameObjectsWithTag("Flower");
        foreach (GameObject f in Flowers)
        {
            f.GetComponent<FlowerPrefab>().ToggleBloomState();
        }
    }
}
