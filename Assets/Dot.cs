using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    public GameObject AnchorBack;
    public GameObject AnchorForward;

    public GameObject SegmentBack;
    public GameObject SegmentForward;


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;


        if(AnchorBack!=null)
        Gizmos.DrawLine(transform.position, AnchorBack.transform.position);

        if(AnchorForward!=null)
            Gizmos.DrawLine(transform.position, AnchorForward.transform.position);

    }

    public void SetAnchor(GameObject anchor,int dir=1)
    {
        if(dir==1)
        {
            AnchorForward = anchor;
        }
        else if(dir==-1)
        {
            AnchorBack = anchor;
        }
        else
        {
            Debug.LogError("This dir is invalid");
        }

        

        //set corrresponding dot
        anchor.GetComponent<Anchor>().dot = this;
        anchor.GetComponent<Anchor>().RenderLine();
    }

    public void ShowAnchors()
    {
        if (AnchorBack != null)
        {
            AnchorBack.SetActive(true);
        }

        if (AnchorForward != null)
        {
            AnchorForward.SetActive(true);
        }
    }


    public void HideAnchors()
    {
        if (AnchorBack != null)
        {
            AnchorBack.SetActive(false);
        }

        if (AnchorForward != null)
        {
            AnchorForward.SetActive(false);
        }
    }
}
