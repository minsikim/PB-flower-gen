using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
//[RequireComponent(typeof(MeshRenderer))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = CreateCylinderMesh(1f, 1f, 1f, 6, 3);
        mesh = GetComponent<MeshFilter>().mesh;
        mesh.name = "Hello";
        
        //GenerateMesh();
        //UpdateMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateMesh()
    {
        vertices = new Vector3[]
        {
            new Vector3(0,0,0),
            new Vector3(0,0,1),
            new Vector3(1,0,0),
            new Vector3(1,1,1)
        };
        triangles = new int[]
        {
            0,1,2,
            1,3,2
        };
    }
    void UpdateMesh()
    {
        mesh.Clear();
        //mesh.vertices = vertices;
        //mesh.triangles = triangles;
        //mesh.RecalculateNormals();
        //mesh.name = "CylinderMesh";
        //mesh = CreateCylinderMesh(1, 1, 1, 2, 3);
    }

    /// <summary>
    /// Creates a <see cref="Mesh"/> filled with vertices forming a cylinder.
    /// </summary>
    /// <remarks>
    /// The values are as follows:
    /// Vertex Count    = slices * (stacks + 1) + 2
    /// Primitive Count = slices * (stacks + 1) * 2
    /// </remarks>
    /// <param name="bottomRadius">Radius at the negative Y end. Value should be greater than or equal to 0.0f.</param>
    /// <param name="topRadius">Radius at the positive Y end. Value should be greater than or equal to 0.0f.</param>
    /// <param name="length">Length of the cylinder along the Y-axis.</param>
    /// <param name="slices">Number of slices about the Y axis.</param>
    /// <param name="stacks">Number of stacks along the Y axis.</param>
    public static Mesh CreateCylinderMesh(float bottomRadius, float topRadius, float length, int slices, int stacks)
    {
        // if both the top and bottom have a radius of zero, just return null, because invalid
        if (bottomRadius <= 0 && topRadius <= 0)
        {
            return null;
        }
        Debug.Log("Creating Cylinder Mesh");
        Mesh mesh = new Mesh();
        mesh.name = "CylinderMesh";
        float sliceStep = (float)Math.PI * 2.0f / slices;
        float heightStep = length / stacks;
        float radiusStep = (topRadius - bottomRadius) / stacks;
        float currentHeight = -length / 2;
        int vertexCount = (stacks + 1) * slices + 2; //cone = stacks * slices + 1
        int triangleCount = (stacks + 1) * slices * 2; //cone = stacks * slices * 2 + slices
        int indexCount = triangleCount * 3;
        float currentRadius = bottomRadius;

        Vector3[] cylinderVertices = new Vector3[vertexCount];
        Vector3[] cylinderNormals = new Vector3[vertexCount];
        Vector2[] cylinderUVs = new Vector2[vertexCount];

        // Start at the bottom of the cylinder            
        int currentVertex = 0;
        cylinderVertices[currentVertex] = new Vector3(0, currentHeight, 0);
        cylinderNormals[currentVertex] = Vector3.down;
        currentVertex++;
        for (int i = 0; i <= stacks; i++)
        {
            float sliceAngle = 0;
            for (int j = 0; j < slices; j++)
            {
                float x = currentRadius * (float)Math.Cos(sliceAngle);
                float y = currentHeight;
                float z = currentRadius * (float)Math.Sin(sliceAngle);

                Vector3 position = new Vector3(x, y, z);
                cylinderVertices[currentVertex] = position;
                cylinderNormals[currentVertex] = Vector3.Normalize(position);
                cylinderUVs[currentVertex] =
                    new Vector2((float)(Math.Sin(cylinderNormals[currentVertex].x) / Math.PI + 0.5f),
                        (float)(Math.Sin(cylinderNormals[currentVertex].y) / Math.PI + 0.5f));

                currentVertex++;

                sliceAngle += sliceStep;
            }
            currentHeight += heightStep;
            currentRadius += radiusStep;
        }
        cylinderVertices[currentVertex] = new Vector3(0, length / 2, 0);
        cylinderNormals[currentVertex] = Vector3.up;
        currentVertex++;

        mesh.vertices = cylinderVertices;
        mesh.normals = cylinderNormals;
        //mesh.uv = cylinderUVs;
        mesh.triangles = CreateIndexBuffer(vertexCount, indexCount, slices);
        //mesh.RecalculateNormals();
        return mesh;
    }


    /// <summary>
    /// Creates an index buffer for spherical shapes like Spheres, Cylinders, and Cones.
    /// </summary>
    /// <param name="vertexCount">The total number of vertices making up the shape.</param>
    /// <param name="indexCount">The total number of indices making up the shape.</param>
    /// <param name="slices">The number of slices about the Y axis.</param>
    /// <returns>The index buffer containing the index data for the shape.</returns>
    private static int[] CreateIndexBuffer(int vertexCount, int indexCount, int slices)
    {
        int[] indices = new int[indexCount];
        int currentIndex = 0;

        // Bottom circle/cone of shape
        for (int i = 1; i <= slices; i++)
        {
            indices[currentIndex++] = i;
            indices[currentIndex++] = 0;
            if (i - 1 == 0)
                indices[currentIndex++] = i + slices - 1;
            else
                indices[currentIndex++] = i - 1;
        }

        // Middle sides of shape
        for (int i = 1; i < vertexCount - slices - 1; i++)
        {
            indices[currentIndex++] = i + slices;
            indices[currentIndex++] = i;
            if ((i - 1) % slices == 0)
                indices[currentIndex++] = i + slices + slices - 1;
            else
                indices[currentIndex++] = i + slices - 1;

            indices[currentIndex++] = i;
            if ((i - 1) % slices == 0)
                indices[currentIndex++] = i + slices - 1;
            else
                indices[currentIndex++] = i - 1;
            if ((i - 1) % slices == 0)
                indices[currentIndex++] = i + slices + slices - 1;
            else
                indices[currentIndex++] = i + slices - 1;
        }

        // Top circle/cone of shape
        for (int i = vertexCount - slices - 1; i < vertexCount - 1; i++)
        {
            indices[currentIndex++] = i;
            if ((i - 1) % slices == 0)
                indices[currentIndex++] = i + slices - 1;
            else
                indices[currentIndex++] = i - 1;
            indices[currentIndex++] = vertexCount - 1;
        }

        return indices;
    }
}
