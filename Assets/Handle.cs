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
    private void On()
    {
        Debug.Log("Mouse Click");
    }

    private void OnMouseDrag()
    {
        transform.position = Vector3.Scale(Camera.main.ScreenToWorldPoint(Input.mousePosition),new Vector3(1,1,0));

        RenderLine();

        dot.RenderBezierByHandle(this);


    }

    public void RenderLine()
    {
        

        lr.positionCount = 2;
        lr.SetPosition(0, dot.transform.position);

        lr.SetPosition(1, transform.position);
    }
}
