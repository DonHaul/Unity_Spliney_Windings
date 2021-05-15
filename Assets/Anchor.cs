using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anchor : MonoBehaviour
{
    public Handle HandleBack;
    public Handle HandleForward;

    public Segment SegmentBack;
    public Segment SegmentForward;


    bool rerender = false;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;


        if(HandleBack != null)
        Gizmos.DrawLine(transform.position, HandleBack.transform.position);

        if(HandleForward != null)
            Gizmos.DrawLine(transform.position, HandleForward.transform.position);

    }

    public void SetHandle(GameObject handle, int dir=1)
    {
        if(dir==1)
        {
            HandleForward = handle.GetComponent<Handle>();
        }
        else if(dir==-1)
        {
            HandleBack = handle.GetComponent<Handle>(); 
        }
        else
        {
            Debug.LogError("This dir is invalid");
        }



        //set corrresponding dot
        handle.GetComponent<Handle>().dot = this;
        handle.GetComponent<Handle>().RenderLine();
    }

    public void ShowAnchors()
    {
        if (HandleBack != null)
        {
            HandleBack.gameObject.SetActive(true);
        }

        if (HandleForward != null)
        {
            HandleForward.gameObject.SetActive(true);
        }
    }


    public void HideAnchors()
    {
        if (HandleBack != null)
        {
            HandleBack.gameObject.SetActive(false);
        }

        if (HandleForward != null)
        {
            HandleForward.gameObject.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        
    }

    private void OnMouseDrag()
    {

        transform.position = Vector3.Scale(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(1, 1, 0));

        HandleForward.RenderLine();
        HandleBack.RenderLine();
        //rerender handle lines
        if (SegmentForward != null)
        {
            
            SegmentForward.RenderBezier();
        }

        if (SegmentBack != null)
        {
            
            SegmentBack.RenderBezier();
        }
               
    }

    public void RenderBezierByHandle(Handle h)
    {
        if (h==HandleForward && SegmentForward!= null)
        {
            SegmentForward.RenderBezier();
        }

        if (h == HandleBack && SegmentBack!=null)
        {
            SegmentBack.RenderBezier();
        }
    }

}
