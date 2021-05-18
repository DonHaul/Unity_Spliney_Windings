using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsManager : MonoBehaviour
{

    public static ControlsManager instance;


    // Start is called before the first frame update

    bool anchorsOn=true;
    bool splineCloseOn = false;

    public bool isCreating;
    public bool isHandling;
    public bool isMiddle;
    public bool isMirrored;
    public bool isMoving;


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
        //tries to fill interior is possible

        if(splineCloseOn)
        {
            MeshMaker.instance.Fill(SplineManager.instance.Dots);
        }

    }


    public void CreateNew()
    {
        //generates a random spline
        SplineManager.instance.DeleteAnchors();
    }

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

    // Update is called once per frame

    private void Update()
    {

        //random spline
        if (Input.GetKeyDown(KeyCode.R))
        {
            anchorsOn = true;
            Random();
            MeshMaker.instance.TurnOffMesh();
        }
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            //AutoAnchors();
            SplineManager.instance.DrawBezier();
        }*/
        if (Input.GetKeyDown(KeyCode.N))
        {
            CreateNew();
            MeshMaker.instance.TurnOffMesh();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Fill();
        }

        //show hide handles
        if (Input.GetKeyDown(KeyCode.H))
        {
            HideShowHandles();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleCloseSpline();
        }

        //create
        if (Input.GetMouseButtonDown(0))
        {
            MeshMaker.instance.TurnOffMesh();
            Collider2D col = Physics2D.OverlapPoint(Vector3.Scale(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(1, 1, 0)));

            if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.LeftAlt))
            {
                //CreateAnchor(Vector3.Scale(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(1, 1, 0)));
                //isCreating = true;
                Debug.Log("Alt + Click");

           

                if(col!=null && col.tag=="Handle")
                { 
                Debug.Log(col);
                isHandling = true;
                    isMirrored = false;

                    activeHandle = col.gameObject.GetComponent<Handle>();
                }
            }
            else
            {


                
                Debug.Log(Vector3.Scale(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(1, 1, 0)));
                Debug.Log(col);
                if (col!=null && col.tag=="Handle")
                {

                        transform.position = Vector3.Scale(Camera.main.ScreenToWorldPoint(Input.mousePosition),new Vector3(1,1,0));

                    isHandling = true;
                    isMirrored = true;
                    activeHandle = col.GetComponent<Handle>();
                            /* RenderLine();

                        dot.RenderBezierByHandle(this);*/


   
                }
                else if (col != null && col.tag == "Dot")
                {
                    Debug.Log("Dot to Move");
                    isMoving = true;
                    activeAnchor = col.GetComponent<Anchor>();

                   // SplineManager.instance.RemoveAnchor(col.GetComponent<Anchor>());
                }
                else if (col != null && col.tag == "Segment")
                {
                    Debug.Log("Dot to Add");
                    SplineManager.instance.CreateAnchorAtSeg(col.gameObject.GetComponent<Segment>(), Vector3.Scale(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(1, 1, 0)));

                    isCreating = true;
                    isMiddle = true;

                }
                else
                {
                    SplineManager.instance.CreateAnchor(Vector3.Scale(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(1, 1, 0)));
                    isCreating = true;
                    isHandling = true;
                    //handle movement is mirrored
                    isMirrored = true;

                }



            }

        }
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

        //reset
        if (Input.GetMouseButtonUp(0))
        {
            isCreating = false;
            isHandling = false;
            isMiddle = false;

            isMirrored = false;

            //activeHandle = null;

            activeAnchor = SplineManager.instance.Dots[SplineManager.instance.Dots.Count - 1];
        }

        //drag after create
        if (Input.GetMouseButton(0))
        {

            MeshMaker.instance.TurnOffMesh();

           if (isHandling && !isCreating)
            {

                activeHandle.UpdatePosition(Vector3.Scale(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(1, 1, 0)),isMirrored);
            }
           


             //after creating, it moves handles mirrored
            if (isCreating)
            {

             if (isHandling)
            {
                activeAnchor.HandleForward.UpdatePosition(Vector3.Scale(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(1, 1, 0)), isMirrored); ;
               
            }

                
                //if its an edge render only previous segment
                activeAnchor.SegmentBack.RenderBezier();



                if (isMiddle)
                {
                    if(activeAnchor!=null)
                    {
                        //if its on the middle render also forward edge
                        activeAnchor.SegmentForward.RenderBezier();
                    }
                }



            }
            //if moving the anchor
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
