using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class StemGenerator : MonoBehaviour
{
    public PathCreator pathCreator;
    private VertexPath vPath;
    private Mesh StemMesh;

    [SerializeField] int vertexCount = 8;
    [Range(0.0f, 0.2f)]
    [SerializeField] float radius = .1f;

    void Start()
    {
        StemMesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh = StemMesh;
        StemMesh.name = "StemMesh";
        GenerateStemMesh();
    }

    void Update()
    {
        GenerateStemMesh();
    }

    void GetVPath()
    {
        vPath = pathCreator.path;
    }
    Mesh GenerateStemMesh()
    {
        GetVPath();
        StemMesh.Clear();

        //Generate Vertex List
        List<Vector3> meshVertices = new List<Vector3>();
        //Generate Triangle List
        List<int> meshTriangles = new List<int>();

        //for each Vertex Path Generate 8 vertices for a circle
        for (int i = 0; i < vPath.NumPoints; i++)
        {
            Vector3 centerPoint = vPath.GetPoint(i);
            Vector3[] tempList = GenerateCircleVertices(centerPoint);
            meshVertices.AddRange(tempList);
        }

        //Generate triangles for Quads
        for (int i = 0; i < vPath.NumPoints - 1; i++)
        {
            int initialPoint = i * vertexCount;
            int[] tempList = GenerateCircleTriangles(initialPoint);
            meshTriangles.AddRange(tempList);
        }

        //assign vertices to mesh
        StemMesh.SetVertices(meshVertices);
        //assign triangles to mesh
        StemMesh.SetTriangles(meshTriangles, 0);

        for (int i = 0; i < vPath.NumPoints; i++)
        {

        }

            //Recalculate stuff
        StemMesh.RecalculateNormals();
        StemMesh.RecalculateBounds();

        //meshTriangles.ForEach(temp => Debug.Log(temp));

        return StemMesh;
    }

    private Vector3[] GenerateCircleVertices(Vector3 centerPoint )
    {

        //Generate Vertices Array Count
        Vector3[] circleVertexList = new Vector3[vertexCount];

        Vector3 firstPoint = new Vector3(radius, 0, 0);

        for (int i = 0; i < vertexCount; i++)
        {
            Vector3 p = Quaternion.AngleAxis(360 / vertexCount * -i, Vector3.up) * firstPoint + centerPoint - transform.position;
            circleVertexList[i] = p;
            //Generate a sphere on points for Test 
            //GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //sphere.transform.position = p;
            //sphere.transform.localScale = Vector3.one * .1f;
            //float tempColor = .8f / vertexCount * (i + 1);
            //sphere.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(1f, tempColor, tempColor));

        }

        return circleVertexList;
    }
    private int[] GenerateCircleTriangles(int initPoint)
    {

        //Generate Cylinder Triangles according to 
        int[] circleTriangleList = new int[vertexCount * 6];

        for (int i = 0; i < vertexCount; i++)
        {
            int[] tempQuad;
            if (i == vertexCount - 1)
            {
                tempQuad = GenerateQuad(
                    initPoint + i,
                    initPoint + i - vertexCount + 1,
                    initPoint + i + vertexCount,
                    initPoint + i + 1
                );
            }
            else
            {
                tempQuad = GenerateQuad(
                    initPoint + i,
                    initPoint + i + 1,
                    initPoint + i + vertexCount,
                    initPoint + i + vertexCount + 1
                );
            }

            for (int j = 0; j < tempQuad.Length; j++)
            {
                circleTriangleList[i * 6 + j] = tempQuad[j];
            }
        }

        return circleTriangleList;
    }
    private int[] GenerateQuad(int a, int b, int c, int d)
    {
        //Generate Quad Array consisting of 6 integers
        //Clockwise = a > c > d > b
        int[] quadRoutine = new int[6];

        quadRoutine[0] = a;
        quadRoutine[1] = c;
        quadRoutine[2] = d;
        quadRoutine[3] = a;
        quadRoutine[4] = d;
        quadRoutine[5] = b;

        return quadRoutine;
    }
}
