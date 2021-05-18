using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handle : MonoBehaviour
{
    public Anchor dot;

    LineRenderer lr;

    // Update is called once per frame
    void Awake()
    {
        lr= GetComponent<LineRenderer>();
    }
   

    public void RenderLine()
    {
        

        lr.positionCount = 2;
        lr.SetPosition(0, dot.transform.position);

        lr.SetPosition(1, transform.position);
    }

    public void UpdatePosition(Vector2 pos, bool mirrorAlso = false)
    {
        transform.position = new Vector3(pos.x, pos.y, -2f);
        RenderLine();
        dot.RenderBezierByHandle(this);

        if (mirrorAlso)
        {
            //control the one you are not handling
            if (dot.HandleBack != this)
            {

                dot.HandleBack.transform.position = dot.transform.position - (dot.HandleForward.transform.position - dot.transform.position);
                dot.HandleBack.RenderLine();
                dot.RenderBezierByHandle(dot.HandleBack);
            }
            else
            {
                dot.HandleForward.transform.position = dot.transform.position - (dot.HandleBack.transform.position - dot.transform.position);
                dot.HandleForward.RenderLine();
                dot.RenderBezierByHandle(dot.HandleForward);

            }



        }
    }
}
