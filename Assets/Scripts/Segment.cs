using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class contains the segment - each segment connects 2 anchors, defined as anchorforward and anchor backwards
/// </summary>
public class Segment : MonoBehaviour
{

    [SerializeField]
    private Anchor anchorforward;
    [SerializeField]
    private Anchor anchorback;

    LineRenderer lr;
    EdgeCollider2D ec;

    Vector2[] v;

    public Anchor Anchorforward { get => anchorforward; set => anchorforward = value; }
    public Anchor Anchorback { get => anchorback; set => anchorback = value; }
    public Vector2[] V { get => v; set => v = value; }


    //Setup segment and connected anchors
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


    //render bezier lines for this segment
    public void RenderBezier()
    {
        //how many intermediary points
        int resolution = SplineManager.instance.resolution;

        //setup vertices and output
        V = new Vector2[resolution+1];
        Vector3[] v3 = new Vector3[resolution+1];

        //set line render length
        lr.positionCount =  resolution + 1;

        //for each point in the interpolation
        for (int j = 0; j <= resolution; j++)
        {

            //calculate the bezier using the anchors and the respective handles that control this segment
            V[j]= Bezier.BezierPoint(anchorback.transform.position, anchorforward.transform.position,
            anchorback.HandleForward.transform.position, anchorforward.HandleBack.transform.position, (float)j / resolution);
            v3[j] = V[j];
        }

        //update positions of line renderer
        lr.SetPositions(v3);

        //update positions of linerenderer
        ec.points = V;

    }
}
