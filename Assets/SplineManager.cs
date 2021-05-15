using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineManager : MonoBehaviour
{
    // Start is called before the first frame update

    public static SplineManager instance;

    public List<GameObject> DotsGO;


    public GameObject Anchorfab;
    public GameObject Handlefab;
    public GameObject Segmentsfab;

    public float defaultMagnitude = 1f;

    public LineRenderer lr;

    public List<LineRenderer> lrs;

    public List<Anchor> Dots;

    public float step = 0.1f;

    public int resolution=10;

    public bool anchorsOn;
    public bool isCreating;
    public bool isHandling;
    public bool isMiddle;

    public bool anchorsMirrored;

    [SerializeField]
    public enum Tool { SplineTool, None };

    public Tool ActiveTool=Tool.None;

    Anchor activeAnchor;
    Handle activeHandle;


    // Start is called before the first frame update
    void Awake()
    {
        instance = this;

        lr = GetComponent<LineRenderer>();

        if(DotsGO.Count==0)
        {

            Debug.LogWarning("Order May nto be the intended one");
        DotsGO = new List<GameObject>(GameObject.FindGameObjectsWithTag("Dot"));
        }
        //AnchorsGO = new List<GameObject>(GameObject.FindGameObjectsWithTag("Anchor"));

        if(DotsGO.Count==1)
        {
            Debug.LogError("No Beziers for just one point ;)");
        }



        for (int i = 0; i < DotsGO.Count; i++)
        {
            Dots.Add( DotsGO[i].GetComponent<Anchor>());
        }

        
    }

    private void Start()
    {
        //DrawLines();
        AutoHandles();
        DrawBezier();

        activeAnchor = Dots[Dots.Count - 1];
    }


    private void DrawLines()
    {
        lr.positionCount = DotsGO.Count;
        for (int i = 0; i < DotsGO.Count; i++)
        {
            lr.SetPosition(i, DotsGO[i].transform.position);
        }

    }

    private void DrawBezier()
    {
        for (int i = 0; i < Dots.Count-1; i++)
        {
            Dots[i].SegmentForward.RenderBezier();
        }
        

    }
    /*
    void DeleteAnchors()
    {
        for (int i = 0; i < AnchorsGO.Count; i++)
        {
            Destroy(AnchorsGO[i]);
        }

        AnchorsGO.Clear();
    }*/

    void AutoHandles()
    {

        //DeleteAnchors();

        Segment prevSeg = null;


        for (int i = 0; i < Dots.Count; i++)
        {
            if(i==0)
            {
                //add anchor

                Dots[i].SetHandle(Instantiate(Handlefab, Dots[i].transform.position + (Dots[i + 1].transform.position - Dots[i].transform.position).normalized * defaultMagnitude, Quaternion.identity), 1);

                Dots[i].SetHandle(Instantiate(Handlefab, Dots[i].transform.position - (Dots[i + 1].transform.position - Dots[i].transform.position).normalized * defaultMagnitude, Quaternion.identity), -1);


                Dots[i].SegmentForward = Instantiate(Segmentsfab).GetComponent<Segment>();
                Dots[i].SegmentForward.SetSegment(Dots[i], Dots[i + 1]);

                prevSeg = Dots[i].SegmentForward;


            }
            else if (i == Dots.Count-1)
            {
                //add anchor
                Dots[i].SetHandle(Instantiate(Handlefab, Dots[i].transform.position + (Dots[i  - 1].transform.position- Dots[i].transform.position).normalized*defaultMagnitude, Quaternion.identity),-1);

                Dots[i].SetHandle(Instantiate(Handlefab, Dots[i].transform.position - (Dots[i - 1].transform.position - Dots[i].transform.position).normalized * defaultMagnitude, Quaternion.identity), 1);

                Dots[i].SegmentBack = prevSeg;
    
            }
            else
            {


                Dots[i].SetHandle(Instantiate(Handlefab, Dots[i].transform.position + (Dots[i + 1].transform.position - Dots[i - 1].transform.position).normalized*defaultMagnitude, Quaternion.identity),1);

                Dots[i].SetHandle(Instantiate(Handlefab, Dots[i].transform.position + (Dots[i - 1].transform.position - Dots[i + 1].transform.position).normalized* defaultMagnitude , Quaternion.identity),-1);

                Dots[i].SegmentForward = Instantiate(Segmentsfab).GetComponent<Segment>(); ;
                Dots[i].SegmentForward.SetSegment(Dots[i], Dots[i + 1]);


                
                Dots[i].SegmentBack = prevSeg;

                prevSeg = Dots[i].SegmentForward;
            }

        }
    }



    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            //AutoAnchors();
            DrawBezier();
        }

        if (Input.GetKeyDown(KeyCode.J))
        {

            anchorsOn = !anchorsOn;

            if (anchorsOn)
            {
                foreach (var item in Dots)
                {
                    item.ShowAnchors();
                }
            }
            else
            { 
            foreach (var item in Dots)
            {
                item.HideAnchors();
            }
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ActiveTool = Tool.SplineTool;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ActiveTool = Tool.None;
        }

        
        if (ActiveTool==Tool.SplineTool)
        {
            //create
            if (Input.GetMouseButtonDown(0) )
            {
                if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.LeftAlt))
                    {
                    //CreateAnchor(Vector3.Scale(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(1, 1, 0)));
                    //isCreating = true;
                    Debug.Log("Alt + Click");

                    Collider2D col = Physics2D.OverlapPoint(Vector3.Scale(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(1, 1, 0)));

                    Debug.Log(col);
                    isHandling = true;

                    activeHandle = col.gameObject.GetComponent<Handle>();

                }
                else
                {


                    Collider2D col = Physics2D.OverlapPoint(Vector3.Scale(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(1, 1, 0)));
                    Debug.Log(Vector3.Scale(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(1, 1, 0)));
                    Debug.Log(col);
                    if(col!=null && col.tag=="Dot")
                    {
                        Debug.Log("Dot to remove");
                        RemoveAnchor(col.GetComponent<Anchor>());
                    }
                    else if (col != null && col.tag == "Segment")
                    {
                        Debug.Log("Dot to Add");
                        CreateAnchorAtSeg(col.gameObject.GetComponent<Segment>(), Vector3.Scale(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(1, 1, 0)));

                        isCreating = true;
                        isMiddle = true;

                    }
                    else
                    { 
                    CreateAnchor(Vector3.Scale(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(1, 1, 0)));
                        isCreating = true;
                        
                    }



                }

            }
            //reset
            if (Input.GetMouseButtonUp(0))
            {
                isCreating = false;
                isHandling = false;
                isMiddle = false;

                activeAnchor = Dots[Dots.Count - 1];
            }


  

            //drag after create
            if (Input.GetMouseButton(0))
            {

                if(isCreating)
                { 
                activeAnchor.HandleForward.transform.position = Vector3.Scale(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(1, 1, 0));
                activeAnchor.HandleForward.RenderLine();

                //mirror handle
                activeAnchor.HandleBack.transform.position = activeAnchor.transform.position - (activeAnchor.HandleForward.transform.position - activeAnchor.transform.position);
                activeAnchor.HandleBack.RenderLine();

                activeAnchor.SegmentBack.RenderBezier();



                    if(isMiddle)
                    {
                        activeAnchor.SegmentForward.RenderBezier();
                    }



                }
                else if(isHandling)
                {
                    if(activeHandle!=null)
                    {
                        activeHandle.transform.position = Vector3.Scale(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(1, 1, 0));
                        activeHandle.dot.RenderBezierByHandle(activeHandle);
                        activeHandle.RenderLine();
                    }
                }

        }
        }
    }

    public void CreateAnchorAtSeg(Segment seg,Vector2 pos)
    {

        Anchor a = Instantiate(Anchorfab, pos, Quaternion.identity).GetComponent<Anchor>();

        

        Dots.Insert(Dots.IndexOf(seg.anchorback),a);
        //a.SetupAnchor()

        //a.HideAnchors();

        a.SetHandle(Instantiate(Handlefab, a.transform.position, Quaternion.identity), 1);

        a.SetHandle(Instantiate(Handlefab, a.transform.position, Quaternion.identity), -1);

        //update last segment
        //activeAnchor.SegmentForward.SetSegment(activeAnchor, a);

        //sticth to last segment;
        //a.SegmentBack = activeAnchor.SegmentForward;


        a.SegmentBack = Instantiate(Segmentsfab).GetComponent<Segment>();



        a.SegmentBack.SetSegment(seg.anchorback, a);
        a.SegmentBack.RenderBezier();
        a.SegmentBack.SetupCollider();


        //connect previous to this
        a.SegmentBack.anchorback.SegmentForward = a.SegmentBack;

        activeAnchor = a;


        a.SegmentForward = seg;
        a.SegmentForward.anchorback = a;

        a.SegmentForward.RenderBezier();
        a.SegmentForward.SetupCollider();

    }

    public void CreateAnchor(Vector2 pos)
    {

        Anchor a = Instantiate(Anchorfab, pos, Quaternion.identity).GetComponent<Anchor>();
        Dots.Add(a);
        //a.SetupAnchor()

        //a.HideAnchors();

        a.SetHandle(Instantiate(Handlefab, a.transform.position, Quaternion.identity), 1);

        a.SetHandle(Instantiate(Handlefab, a.transform.position, Quaternion.identity), -1);

        //update last segment
        //activeAnchor.SegmentForward.SetSegment(activeAnchor, a);

        //sticth to last segment;
        //a.SegmentBack = activeAnchor.SegmentForward;


        a.SegmentBack = Instantiate(Segmentsfab).GetComponent<Segment>();



        a.SegmentBack.SetSegment(activeAnchor, a);
        a.SegmentBack.RenderBezier();
        a.SegmentBack.SetupCollider();


        //connect previous to this
        a.SegmentBack.anchorback.SegmentForward = a.SegmentBack;

        activeAnchor = a;

    }

    public void RemoveAnchor(Anchor a)
    {
        //attach previous to next

        //if not last point
        if(a.SegmentForward!=null)
        { 
        a.SegmentBack.anchorforward = a.SegmentForward.anchorforward;
            a.SegmentBack.RenderBezier();
            Destroy(a.SegmentForward.gameObject);

            //connect previous to this;
            a.SegmentForward.anchorforward.SegmentBack = a.SegmentBack;
        }

        //if last element
        if(Dots.IndexOf(a)==Dots.Count-1)
        {
            //set activeAncor to previous
            activeAnchor = Dots[Dots.Count - 2];

            //remove line
            Destroy(a.SegmentBack.gameObject);

        
        }

        //delete it self
        Dots.Remove(a);
        

        
        Destroy(a.HandleBack.gameObject);
        Destroy(a.HandleForward.gameObject);

        Destroy(a.gameObject);
    }


}
