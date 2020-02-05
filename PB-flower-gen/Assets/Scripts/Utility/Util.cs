using System;
using System.Collections.Generic;
using UnityEngine;
using SplineMesh;

class Util
{
    #region Basic Helpers

    public static GameObject InitWithParent(GameObject prefab, Transform parent)
    {
        GameObject instantiatedObject = GameObject.Instantiate(prefab, parent);
        instantiatedObject.transform.localPosition = Vector3.zero;
        return instantiatedObject;
    }
    public static GameObject InitWithParent(string name, Transform parent)
    {
        GameObject newObject = new GameObject(name);
        newObject.transform.SetParent(parent);
        newObject.transform.localPosition = Vector3.zero;
        return newObject;
    }
    public static GameObject SetPrefab(GameObject prefab)
    {
        return prefab;
    }
    #endregion

    #region Random Functions
    public static int[] DistributeRandomIntArray(int howMany, Vector2Int range)
    {
        int[] counts = new int[2];

        for (int i = 0; i < howMany; i++)
        {
            counts[i] = RandomRange(range);
        }
        return counts;
    }
    public static int SumArray(int[] array)
    {
        int total = 0;
        foreach(int value in array)
        {
            total = total + value;
        }
        return total;
    }
    public static int RandomRange(Vector2Int range)
    {
        return UnityEngine.Random.Range(range.x, range.y + 1);
    }
    public static float RandomRange(Vector2 range)
    {
        return UnityEngine.Random.Range(range.x, range.y);
    }
    public static int RandomRange(int from, int to)
    {
        return UnityEngine.Random.Range(from, to + 1);
    }
    public static float RandomRange(float from, float to)
    {
        return UnityEngine.Random.Range(from, to);
    }
    public static float RandomRange(float range)
    {
        return UnityEngine.Random.Range(-range, range);
    }
    public static Color GetColorFromRange(Color c1, Color c2)
    {
        Color color = new Color(0, 0, 0, 1);

        color.r = UnityEngine.Random.Range(c1.r, c2.r);
        color.g = UnityEngine.Random.Range(c1.g, c2.g);
        color.b = UnityEngine.Random.Range(c1.b, c2.b);

        return color;
    }
    public static Color[] DistributeColorsFromRange(int LeafCount, Color LeafColorRange1, Color LeafColorRange2)
    {
        Color[] colorArray = new Color[LeafCount];
        for (int i = 0; i < LeafCount; i++)
        {
            colorArray[i] = GetColorFromRange(LeafColorRange1, LeafColorRange2);
        }
        return colorArray;
    }
    #endregion

    #region Spline Functions
    public static Spline InitSpline(GameObject o, Node[] nodes)
    {
        Spline spline = o.AddComponent<Spline>();
        spline = NodeToSpline(spline, nodes);
        return spline;
    }
    public static Spline CreateSplinefromNodes(Node[] nodes)
    {
        Spline spline = new Spline();
        NodeToSpline(spline, nodes);
        return spline;
    }
    public static Spline NodeToSpline(Spline spline, Node[] nodes)
    {
        foreach (Node n in nodes)
        {
            spline.AddNode(new SplineNode(n.position, n.handleOut));
        }
        return spline;
    }
    public static Spline NodeToSpline(Node[] nodes)
    {
        Spline spline = new Spline();
        foreach (Node n in nodes)
        {
            spline.AddNode(new SplineNode(n.position, n.handleOut));
        }
        return spline;
    }
    public static Spline UpdateNodeToSpline(Spline spline, Node[] nodes)
    {
        for (int i = 0; i < spline.nodes.Count; i++)
        {
            spline.nodes[i].Position = nodes[i].position;
            spline.nodes[i].Direction = nodes[i].handleOut;
        }
        return spline;
    }
    public static Spline MatchNodeCount(Spline from, Node[] to)
    {
        int fromCount = from.nodes.Count;
        int toCount = to.Length;
        int difference = toCount - fromCount;
        if (fromCount != toCount)
        {
            for (int i = 0; i < difference; i++)
            {
                //Adds Node between last and before last spline

                SplineNode lastNode = from.nodes[fromCount - 1];
                SplineNode formerLastNode = from.nodes[fromCount - 2];
                SplineNode firstNode = from.nodes[0];

                float fullDist = Vector3.Distance(lastNode.Position, firstNode.Position);
                float formerDist = Vector3.Distance(formerLastNode.Position, firstNode.Position);
                float lastFormerDist = Vector3.Distance(formerLastNode.Position, lastNode.Position);

                float insertTime = ((lastFormerDist / 2) + formerDist) / fullDist;

                CurveSample insertNode = GetSampleAt(from, insertTime);
                from.InsertNode(fromCount - 1, new SplineNode(insertNode.location, insertNode.tangent * 0.1f + insertNode.location));
            }
        }
        return from;
    }

    public static Node[] MatchNodeCount(Node[] from, Node[] to, Spline fromSpline)
    {
        int fromCount = from.Length;
        int toCount = to.Length;
        int difference = toCount - fromCount;

        List<Node> tempNodeList = new List<Node>(from);

        if (fromCount != toCount)
        {
            for (int i = 0; i < difference; i++)
            {
                //Adds Node between last and before last spline

                SplineNode lastNode = fromSpline.nodes[fromCount - 1];
                SplineNode formerLastNode = fromSpline.nodes[fromCount - 2];
                SplineNode firstNode = fromSpline.nodes[0];

                float fullDist = Vector3.Distance(lastNode.Position, firstNode.Position);
                float formerDist = Vector3.Distance(formerLastNode.Position, firstNode.Position);
                float lastFormerDist = Vector3.Distance(formerLastNode.Position, lastNode.Position);

                float insertTime = ((lastFormerDist / 2) + formerDist) / fullDist;

                CurveSample insertNodeSample = GetSampleAt(fromSpline, insertTime);
                Node insertNode = new Node(insertNodeSample.location, insertNodeSample.tangent * 0.1f + insertNodeSample.location);
                tempNodeList.Insert(tempNodeList.Count - 1, insertNode);

                fromSpline.InsertNode(fromSpline.nodes.Count - 1, new SplineNode(insertNode.position, insertNode.handleOut));
            }
        }
        Node[] insertedNodeArray = tempNodeList.ToArray();

        return insertedNodeArray;
    }
    public static Node[] MatchNodeCount(Node[] from, Node[] to)
    {
        Spline fromSpline = NodeToSpline(from);

        return MatchNodeCount(from, to, fromSpline);
    }
    public static Node[] LerpNodeList(Node[] from, Node[] to, Spline spline, float progress)
    {
        //Check Node Count to Lerp
        if (from.Length != to.Length)
        {
            spline = MatchNodeCount(spline, to);
            from = MatchNodeCount(from, to, spline);
        }

        Node[] lerpedNodeList = new Node[to.Length];

        for (int i = 0; i < lerpedNodeList.Length; i++)
        {
            lerpedNodeList[i] = LerpNode(from[i], to[i], progress);
        }

        return lerpedNodeList;
    }
    public static Node LerpNode(Node from, Node to, float progress)
    {
        Vector3 lerpedPosition = (to.position - from.position) * progress + from.position;
        Vector3 lerpedhandleOut = (to.handleOut - from.handleOut) * progress + from.handleOut;
        Node changedNode = new Node(lerpedPosition, lerpedhandleOut);

        return changedNode;
    }

    public static CurveSample GetSampleAt(Spline spline, float t)
    {
        return spline.GetSample(t * (spline.nodes.Count - 1));
    }
    public static Vector3 GetPointAt(Spline spline, float t)
    {
        return spline.GetSample(t * (spline.nodes.Count - 1)).location;
    }
    public static Vector3 GetDirectionAt(Spline spline, float t)
    {
        return spline.GetSample(t * (spline.nodes.Count - 1)).tangent;
    }
    #endregion

}

