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
}
