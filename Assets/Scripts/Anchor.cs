using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Anchors are the points the segments connect to.
/// Each segment has 2 segments one back and one forward
/// and two handle (back and forward) that influence the corresponding segments
/// </summary>
public class Anchor : MonoBehaviour
{
    [SerializeField]
    private Handle handleBack;
    [SerializeField]
    private Handle handleForward;

    [SerializeField]
    private Segment segmentBack;
    [SerializeField]
    private Segment segmentForward;

    public Handle HandleBack { get => handleBack; set => handleBack = value; }
    public Handle HandleForward { get => handleForward; set => handleForward = value; }
    public Segment SegmentBack { get => segmentBack; set => segmentBack = value; }
    public Segment SegmentForward { get => segmentForward; set => segmentForward = value; }



    //assign handles
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
        

        //set the handle to connect to the current anchor
        handle.GetComponent<Handle>().Dot = this;
        handle.GetComponent<Handle>().RenderLine();
    }


    //hides handles if exist
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

    //shows handles if exist
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

    //destroy this anchor and all connected elements (handles and segments)
    public void Destroy()
    {
        if(SegmentBack !=null)
        {
            Destroy(SegmentBack.gameObject);
        }
        if(SegmentForward !=null)
        {
            Destroy(SegmentForward.gameObject);
        }
        Destroy(HandleBack.gameObject);
        Destroy(HandleForward.gameObject);
        Destroy(gameObject);
    }


    //rerender bezier segment for handle h
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


    //update handles and segments upon moving anchor;
    public void UpdatePosition(Vector2 pos)
    {
        //-1 makes the anchor be in front of the segments, but behind the handles
        transform.position = new Vector3(pos.x, pos.y, -1f);

        if (SegmentBack != null)
        {
            SegmentBack.RenderBezier();
        }
        if (SegmentForward != null)
        {
            SegmentForward.RenderBezier();
        }

        if (HandleForward != null)
        {
            HandleForward.RenderLine();
        }

        if (HandleBack != null)
        {
            HandleBack.RenderLine();
        }
    }
}
