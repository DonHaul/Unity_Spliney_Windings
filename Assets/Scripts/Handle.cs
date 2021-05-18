using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handle : MonoBehaviour
{
    private Anchor dot;

    LineRenderer lr;

    public Anchor Dot { get => dot; set => dot = value; }

    // Update is called once per frame
    void Awake()
    {
        lr= GetComponent<LineRenderer>();
    }
   

    //rerender line that goes from anchor to handle
    public void RenderLine()
    {
        lr.positionCount = 2;
        lr.SetPosition(0, Dot.transform.position);
        lr.SetPosition(1, transform.position);
    }


    //upon moving handle, update sibling handle and corresponding segments
    public void UpdatePosition(Vector2 pos, bool mirrorAlso = false)
    {

        //-2 puts it in frotn of the anchors (-1) and of the segments (0)
        transform.position = new Vector3(pos.x, pos.y, -2f);

        //update line and segment
        RenderLine();
        Dot.RenderBezierByHandle(this);

        //update line and segment of sibling handle
        if (mirrorAlso)
        {
            //control the one you are not handling
            if (Dot.HandleBack != this)
            {

                Dot.HandleBack.transform.position = Dot.transform.position - (Dot.HandleForward.transform.position - Dot.transform.position);
                Dot.HandleBack.RenderLine();
                Dot.RenderBezierByHandle(Dot.HandleBack);
            }
            else
            {
                Dot.HandleForward.transform.position = Dot.transform.position - (Dot.HandleBack.transform.position - Dot.transform.position);
                Dot.HandleForward.RenderLine();
                Dot.RenderBezierByHandle(Dot.HandleForward);

            }



        }
    }
}
