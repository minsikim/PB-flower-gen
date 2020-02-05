using System;
using UnityEngine;

class Util
{
    public static int[] DistributeRandomIntArray(int howMany, Vector2Int range)
    {
        int[] counts = new int[2];

        for (int i = 0; i < howMany; i++)
        {
            counts[i] = RandomRange(range);
        }
        return counts;
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
}

