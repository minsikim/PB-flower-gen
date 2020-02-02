using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnFlower : MonoBehaviour
{
    [SerializeField] GameObject SpawnPrefab;
    [SerializeField] GameObject FlowerCount;
    [SerializeField] int MaxFlowerCount;
    private int CurrFlowerCount = 0;

    public bool autoSpawn = false;
    public float spawnInterval = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        if(autoSpawn) InvokeRepeating("InitFlower", 0f, spawnInterval);
    }

    public void InitFlower()
    {
        if (MaxFlowerCount <= CurrFlowerCount) return;

        Instantiate(SpawnPrefab);

        float z1 = -13f;
        float z2 = 37f;

        float x1 = 8f;
        float x2 = 22f;

        float zRandom = UnityEngine.Random.Range(z1, z2);
        float xCal = ((x2 - x1) * ((zRandom-z1) / x2-z1)) + x1;
        float x = UnityEngine.Random.Range(-1f, 1f) * ( xCal / 5);

        SpawnPrefab.transform.position = new Vector3(x, 0, zRandom);

        CurrFlowerCount++;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    RaycastHit hit;
        //    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //    if (Physics.Raycast(ray, out hit))
        //    {
        //        Instantiate(SpawnPrefab);
        //        SpawnPrefab.transform.position = hit.point;

        //        //int currentFlowerCount = Convert.ToInt32(FlowerCount.GetComponent<Text>().text);
        //        //FlowerCount.GetComponent<Text>().text = (currentFlowerCount + 1).ToString();
        //    }

        //}
        //FlowerCount.GetComponent<Text>().text = (1 / Time.deltaTime).ToString();
    }

    //public void SetGrowState()
    //{
    //    GameObject[] Flowers = GameObject.FindGameObjectsWithTag("Flower");
    //    foreach(GameObject f in Flowers)
    //    {
    //        f.GetComponent<FlowerPrefab>().ToggleGrowState();
    //    }
    //}
    //public void SetBloomState()
    //{
    //    GameObject[] Flowers = GameObject.FindGameObjectsWithTag("Flower");
    //    foreach (GameObject f in Flowers)
    //    {
    //        f.GetComponent<FlowerPrefab>().ToggleBloomState();
    //    }
    //}
}
