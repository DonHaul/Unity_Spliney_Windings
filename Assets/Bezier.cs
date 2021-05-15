using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Bezier
{
    // Start is called before the first frame update

    // Update is called once per frame
    public static Vector2  BezierPoint(Vector2 dot1, Vector2 dot2, Vector2 anch1, Vector2 anch2, float val)
    {
        //Debug.Log("Bezier is:");

        Vector2 v = Mathf.Pow((1 - val), 3) * dot1 + 3 * Mathf.Pow((1 - val), 2) * val * anch1 + 3 * (1 - val) * Mathf.Pow(val, 2) * anch2 + Mathf.Pow((val), 3) * dot2;
        ;
        //Debug.Log(v);

        return v;
    }



}
