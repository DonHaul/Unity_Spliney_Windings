using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Defines commands and logic to edit the splines
/// </summary>
public class ControlsManager : MonoBehaviour
{
    //singleton
    public static ControlsManager instance;



    bool anchorsOn=true;
    bool splineCloseOn = false;

    //flags
    public bool isCreating;
    public bool isHandling;
    public bool isMiddle;
    public bool isMirrored;
    public bool isMoving;

    [SerializeField]
    Anchor activeAnchor;

    [SerializeField]
    Handle activeHandle;

    public bool SplineCloseOn { get => splineCloseOn; set => splineCloseOn = value; }
    public Anchor ActiveAnchor { get => activeAnchor; set => activeAnchor = value; }
    public Handle ActiveHandle { get => activeHandle; set => activeHandle = value; }

    public void Awake()
    {
        instance = this;
    }

    public void Random()
    {
        //generates a random spline
        SplineManager.instance.Generate();
    }


    public void Fill()
    {
        //tries to fill interior if all conditions are met
        if(splineCloseOn)
        {
            MeshMaker.instance.Fill(SplineManager.instance.Dots);
        }

    }

    //empty everything
    public void CreateNew()
    {
        //generates a random spline
        SplineManager.instance.DeleteAnchors();
    }

    //toggle hadles
    public void HideShowHandles()
    {
        anchorsOn = !anchorsOn;

        if (anchorsOn)
        {
            foreach (var item in SplineManager.instance.Dots)
            {
                item.ShowAnchors();
            }
        }
        else
        {
            foreach (var item in SplineManager.instance.Dots)
            {
                item.HideAnchors();
            }
        }
    }

    public void ToggleCloseSpline()
    {
        splineCloseOn = !splineCloseOn;


        SplineManager.instance.ToggleSplineClose(splineCloseOn);
    }




    // Used to detect keyboard and mouse input
    private void Update()
    {

        //random spline
        if (Input.GetKeyDown(KeyCode.R))
        {
            anchorsOn = true;
            Random();
            MeshMaker.instance.TurnOffMesh();
        }

        //new spline
        if (Input.GetKeyDown(KeyCode.N))
        {
            CreateNew();
            MeshMaker.instance.TurnOffMesh();
        }

        //fill inside
        if (Input.GetKeyDown(KeyCode.F))
        {
            Fill();
        }

        //show hide handles
        if (Input.GetKeyDown(KeyCode.H))
        {
            HideShowHandles();
        }

        //toggle close or open loop
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleCloseSpline();
            MeshMaker.instance.TurnOffMesh();
        }

        //on left click down
        if (Input.GetMouseButtonDown(0))
        {
            MeshMaker.instance.TurnOffMesh();


            Collider2D col = Physics2D.OverlapPoint(Vector3.Scale(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(1, 1, 0)));

            //if pressing alt, on a handle, do a single handle move
            if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.LeftAlt))
            {
                if(col!=null && col.tag=="Handle")
                { 
                    isHandling = true;
                    isMirrored = false;
                    //set as active
                    activeHandle = col.gameObject.GetComponent<Handle>();
                }
            }
            else
            {
                //if normal click on handle, do a mirror move
                if (col!=null && col.tag=="Handle")
                {

                    transform.position = Vector3.Scale(Camera.main.ScreenToWorldPoint(Input.mousePosition),new Vector3(1,1,0));

                    isHandling = true;
                    isMirrored = true;
                    //set as active
                    activeHandle = col.GetComponent<Handle>();



   
                }
                //if normal click on acnhor, drags it
                else if (col != null && col.tag == "Dot")
                {
                    isMoving = true;
                    activeAnchor = col.GetComponent<Anchor>();


                }
                //if normal click on segment, add dot in there
                else if (col != null && col.tag == "Segment")
                {
                    SplineManager.instance.CreateAnchorAtSeg(col.gameObject.GetComponent<Segment>(), Vector3.Scale(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(1, 1, 0)));

                    isCreating = true;
                    isMiddle = true;
                    isHandling = true;
                    isMirrored = true;

                }
                //if clicked in the empty space, add an anchor there
                else
                {
                    SplineManager.instance.CreateAnchor(Vector3.Scale(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(1, 1, 0)));
                    isCreating = true;
                    isHandling = true;
                    isMirrored = true;
                }               
            }

        }
        //on right click on an anchor, delete it
        if (Input.GetMouseButtonDown(1))
        {

            MeshMaker.instance.TurnOffMesh();
            Collider2D col = Physics2D.OverlapPoint(Vector3.Scale(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(1, 1, 0)));


            if (col != null && col.tag == "Dot")
            {
                Debug.Log("Dot to remove");
                SplineManager.instance.RemoveAnchor(col.GetComponent<Anchor>());
            }

           
        }

        //on left click up, reset flags
        if (Input.GetMouseButtonUp(0))
        {
            isCreating = false;
            isHandling = false;
            isMiddle = false;
            isMirrored = false;

            //activeHandle = null;

            activeAnchor = SplineManager.instance.Dots[SplineManager.instance.Dots.Count - 1];
        }

        //actions while dragging left mouse
        if (Input.GetMouseButton(0))
        {

            MeshMaker.instance.TurnOffMesh();


            //if handle, then drag handle
           if (isHandling && !isCreating)
            {
                activeHandle.UpdatePosition(Vector3.Scale(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(1, 1, 0)),isMirrored);
            }
           //after creating, it moves handles mirrored
            if (isCreating)
            {

             if (isHandling)
            {
                //move handle
                activeAnchor.HandleForward.UpdatePosition(Vector3.Scale(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(1, 1, 0)), isMirrored);
               
            }               
                //if its an edge render only previous segment
                activeAnchor.SegmentBack.RenderBezier();


                //if is a point in the midle of a line, update the other segment
                if (isMiddle)
                {
                    if(activeAnchor!=null)
                    {
                        //if its on the middle render also forward edge
                        activeAnchor.SegmentForward.RenderBezier();
                    }
                }



            }
            //else, drag the anchor
            else if (isMoving)
                {
                    if (activeAnchor != null)
                    {          
                        activeAnchor.UpdatePosition(Vector3.Scale(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(1, 1, 0)));
                    }
                }

        }
        
    }
}
