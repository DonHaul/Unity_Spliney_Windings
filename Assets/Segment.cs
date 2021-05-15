using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Segment : MonoBehaviour
{

    public Anchor anchorforward;
    public Anchor anchorback;

    public LineRenderer lr;

    public void SetSegment(Anchor back,Anchor forward)
    {
        anchorback= back;
        anchorforward = forward;
    }

    // Start is called before the first frame update
    void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }



    public void RenderBezier()
    {

        int resolution = SplineManager.instance.resolution;

        lr.positionCount =  resolution + 1;

        for (int j = 0; j <= resolution; j++)
        {
  
            lr.SetPosition(j, Bezier.BezierPoint(anchorback.transform.position, anchorforward.transform.position,
                anchorback.HandleForward.transform.position, anchorforward.HandleBack.transform.position, (float)j / resolution));

        }

    }
}
