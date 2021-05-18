using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Segment : MonoBehaviour
{

    public Anchor anchorforward;
    public Anchor anchorback;

    public LineRenderer lr;
    public EdgeCollider2D ec;

    public Vector2[] v;

    public void SetSegment(Anchor back,Anchor forward)
    {
        anchorback= back;
        anchorforward = forward;

        anchorback.SegmentForward = this;
        anchorforward.SegmentBack = this;
    }

    // Start is called before the first frame update
    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        ec = GetComponent<EdgeCollider2D>();
    }



    public void RenderBezier()
    {
        

        int resolution = SplineManager.instance.resolution;

        v = new Vector2[resolution+1];
        Vector3[] v3 = new Vector3[resolution+1];

        lr.positionCount =  resolution + 1;
        //ec.pointCount = resolution + 1;

        for (int j = 0; j <= resolution; j++)
        {

            /* lr.SetPosition(j, Bezier.BezierPoint(anchorback.transform.position, anchorforward.transform.position,
                 anchorback.HandleForward.transform.position, anchorforward.HandleBack.transform.position, (float)j / resolution));*/
            v[j]= Bezier.BezierPoint(anchorback.transform.position, anchorforward.transform.position,
            anchorback.HandleForward.transform.position, anchorforward.HandleBack.transform.position, (float)j / resolution);
            v3[j] = v[j];
        }
        lr.SetPositions(v3);
        ec.points = v;

    }

    public void SetupCollider()
    {
        /*Vector3[] p = new Vector3[lr.positionCount];
        Vector2[] p2 = new Vector2[lr.positionCount];

        lr.GetPositions(p);

        for (int i = 0; i < lr.positionCount; i++)
        {
            Debug.Log(i);
            Debug.Log(lr.GetPosition(i));
            p2[i] = lr.GetPosition(i);//adding position to convert to global
            Debug.Log(p2[i]);
        }

        

        ec.points = (p2);

        ec.edgeRadius = lr.startWidth;*/
    }
}
