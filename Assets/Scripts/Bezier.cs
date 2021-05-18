using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Bezier
{
    public static Vector2  BezierPoint(Vector2 dot1, Vector2 dot2, Vector2 anch1, Vector2 anch2, float val)
    {
        //equation taken from https://www.youtube.com/watch?v=pnYccz1Ha34
         return Mathf.Pow((1 - val), 3) * dot1 + 3 * Mathf.Pow((1 - val), 2) * val * anch1 + 3 * (1 - val) * Mathf.Pow(val, 2) * anch2 + Mathf.Pow((val), 3) * dot2;
    }
}
