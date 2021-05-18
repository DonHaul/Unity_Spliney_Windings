using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Contains all logic related to generating the splines
/// </summary>
public class SplineManager : MonoBehaviour
{
    public static SplineManager instance;

    

    [SerializeField]
    GameObject Anchorfab;

    [SerializeField]
    GameObject Handlefab;

    [SerializeField]
    GameObject Segmentsfab;

    //used to autogenerate handles
    [SerializeField]
    float defaultMagnitude = 1f;

    //list of anchors
    public List<Anchor> Dots;

    //how many points inbetween anchors
    [SerializeField]
    public int resolution=10;

    [SerializeField]
    Vector2 camerabounds;

    //hold values of handles b4 smoothing when Close Loop is used
    Vector2 backB4;
    Vector2 forwardB4;
    



    // Start is called before the first frame update
    void Awake()
    {
        //setup singleton
        instance = this;
    }


    //grab existing dots on the game
    private void Start()
    {
        List<GameObject> DotsGO = new List<GameObject>();
               
        if (DotsGO.Count == 0)
        {

            Debug.LogWarning("Order May nto be the intended one");
            DotsGO = new List<GameObject>(GameObject.FindGameObjectsWithTag("Dot"));
        }
        //AnchorsGO = new List<GameObject>(GameObject.FindGameObjectsWithTag("Anchor"));

        if (DotsGO.Count == 1)
        {
            Debug.LogError("No Beziers for just one point ;)");
        }



        for (int i = 0; i < DotsGO.Count; i++)
        {
            Dots.Add(DotsGO[i].GetComponent<Anchor>());
        }

        //initialize handles
        AutoHandles();
        //draw all beziers
        DrawBezier();
        //set active anchor as the last one
        ControlsManager.instance.ActiveAnchor = Dots[Dots.Count - 1];
    }

    //draw all beziers
    public  void DrawBezier()
    {
        for (int i = 0; i < Dots.Count-1; i++)
        {
            Dots[i].SegmentForward.RenderBezier();
        }
    }
    

    //delete all anchors ands attached segments/handles
    public void DeleteAnchors()
    {
        for (int i = 0; i < Dots.Count; i++)
        {
            Dots[i].Destroy();

        }

        Dots.Clear();
    }

 
    /// Generate Handles for a set of Anchors
    void AutoHandles()
    {


        Segment prevSeg = null;


        //for every anchor
        for (int i = 0; i < Dots.Count; i++)
        {

            //if is the first anchor
            if(i==0)
            {
                //add anchor
                //set handle back and forward
                Dots[i].SetHandle(Instantiate(Handlefab, Dots[i].transform.position + (Dots[i + 1].transform.position - Dots[i].transform.position).normalized * defaultMagnitude, Quaternion.identity), 1);
                Dots[i].SetHandle(Instantiate(Handlefab, Dots[i].transform.position - (Dots[i + 1].transform.position - Dots[i].transform.position).normalized * defaultMagnitude, Quaternion.identity), -1);

                //set segment forward
                Dots[i].SegmentForward = Instantiate(Segmentsfab).GetComponent<Segment>();
                Dots[i].SegmentForward.SetSegment(Dots[i], Dots[i + 1]);

                //store created segment to attach to next anchor
                prevSeg = Dots[i].SegmentForward;


            }
            //if is last anchor
            else if (i == Dots.Count-1)
            {
                //set handle back and forward
                Dots[i].SetHandle(Instantiate(Handlefab, Dots[i].transform.position + (Dots[i  - 1].transform.position- Dots[i].transform.position).normalized*defaultMagnitude, Quaternion.identity),-1);
                Dots[i].SetHandle(Instantiate(Handlefab, Dots[i].transform.position - (Dots[i - 1].transform.position - Dots[i].transform.position).normalized * defaultMagnitude, Quaternion.identity), 1);


                //dont create a segment, only attach last one
                Dots[i].SegmentBack = prevSeg;
                
    
            }
            else
            {

                //set handle back and forward
                Dots[i].SetHandle(Instantiate(Handlefab, Dots[i].transform.position + (Dots[i + 1].transform.position - Dots[i - 1].transform.position).normalized*defaultMagnitude, Quaternion.identity),1);
                Dots[i].SetHandle(Instantiate(Handlefab, Dots[i].transform.position + (Dots[i - 1].transform.position - Dots[i + 1].transform.position).normalized* defaultMagnitude , Quaternion.identity),-1);

                //create a new segment
                Dots[i].SegmentForward = Instantiate(Segmentsfab).GetComponent<Segment>(); ;
                Dots[i].SegmentForward.SetSegment(Dots[i], Dots[i + 1]);


                //attach previous segment
                Dots[i].SegmentBack = prevSeg;

                //store created segment to attach to next anchor
                prevSeg = Dots[i].SegmentForward;
            }

        }
    }



    //open or close spline
    public void ToggleSplineClose(bool isOn)
    {


        if(isOn)
        {

            //create a segment
            Dots[0].SegmentBack = Instantiate(Segmentsfab).GetComponent<Segment>();

            //assign segment anchors
            Dots[0].SegmentBack.SetSegment(Dots[Dots.Count - 1], Dots[0]);

            //save current handle positions
            backB4 = Dots[0].HandleBack.transform.position;
            forwardB4 = Dots[Dots.Count - 1].HandleForward.transform.position;

            //smoothen handles of first and last anchor to get  "good" transition
            Dots[0].HandleBack.UpdatePosition(((Dots[0].HandleBack.transform.position - Dots[0].transform.position) + (Dots[Dots.Count - 1].transform.position - Dots[0].transform.position)) / 2 + Dots[0].transform.position);
            Dots[Dots.Count - 1].HandleForward.UpdatePosition(((Dots[Dots.Count - 1].HandleForward.transform.position - Dots[Dots.Count - 1].transform.position) + (Dots[0].transform.position - Dots[Dots.Count - 1].transform.position)) / 2 + Dots[Dots.Count - 1].transform.position);

        }
        else
        {
            //remove closing segment
            Destroy(Dots[0].SegmentBack.gameObject);

            //restore handle positions
            Dots[0].HandleBack.UpdatePosition(backB4);        
            Dots[Dots.Count - 1].HandleForward.UpdatePosition(forwardB4);
        }
    }


    //creates an anchor in the middle of a segment
    public void CreateAnchorAtSeg(Segment seg,Vector2 pos)
    {

        //create it
        Anchor a = Instantiate(Anchorfab, pos, Quaternion.identity).GetComponent<Anchor>();

        //insert in middle of list
        Dots.Insert(Dots.IndexOf(seg.Anchorback),a);
        
        //setup handles
        a.SetHandle(Instantiate(Handlefab, a.transform.position, Quaternion.identity), 1);
        a.SetHandle(Instantiate(Handlefab, a.transform.position, Quaternion.identity), -1);

        //calculate a new segment
        a.SegmentBack = Instantiate(Segmentsfab).GetComponent<Segment>();    
        a.SegmentBack.SetSegment(seg.Anchorback, a);
        a.SegmentBack.RenderBezier();
        a.SegmentBack.Anchorback.SegmentForward = a.SegmentBack;

        //set as actuve abcgir
        ControlsManager.instance.ActiveAnchor = a;

        //attach forward segment
        a.SegmentForward = seg;
        a.SegmentForward.Anchorback = a;
        a.SegmentForward.RenderBezier();
 

    }


    //create anchor out of nowhere
    public void CreateAnchor(Vector2 pos)
    {
        //if spline is closed, destroy is first
        if (ControlsManager.instance.SplineCloseOn)
        {
            Destroy(Dots[0].SegmentBack.gameObject);
        }

        //Create Anchor
        Anchor a = Instantiate(Anchorfab,new Vector3 (pos.x,pos.y,-1f), Quaternion.identity).GetComponent<Anchor>();
        Dots.Add(a);
        
        //setup handles
        a.SetHandle(Instantiate(Handlefab, a.transform.position, Quaternion.identity), 1);
        a.SetHandle(Instantiate(Handlefab, a.transform.position, Quaternion.identity), -1);

        //attach to previous anchor
        a.SegmentBack = Instantiate(Segmentsfab).GetComponent<Segment>();
        a.SegmentBack.SetSegment(ControlsManager.instance.ActiveAnchor, a);
        a.SegmentBack.RenderBezier();
        a.SegmentBack.Anchorback.SegmentForward = a.SegmentBack;

        //set as active anchor
        ControlsManager.instance.ActiveAnchor = a;

        //readd closing segment if required
        if(ControlsManager.instance.SplineCloseOn)
        { 
        ToggleSplineClose(ControlsManager.instance.SplineCloseOn);
        }
    }

    public void RemoveAnchor(Anchor a)
    {
        //attach previous to next

        //if not last point
        if(a.SegmentForward!=null)
        { 
            //attach 2 anchors on each side
            a.SegmentBack.Anchorforward = a.SegmentForward.Anchorforward;
            a.SegmentBack.RenderBezier();
            Destroy(a.SegmentForward.gameObject);

            //connect previous to this;
            a.SegmentForward.Anchorforward.SegmentBack = a.SegmentBack;
        }

        //if last element
        if(Dots.IndexOf(a)==Dots.Count-1)
        {
            //set activeAncor to previous
            ControlsManager.instance.ActiveAnchor = Dots[Dots.Count - 2];

            //remove line
            Destroy(a.SegmentBack.gameObject);

        
        }

        //delete it self
        Dots.Remove(a);      
        Destroy(a.HandleBack.gameObject);
        Destroy(a.HandleForward.gameObject);
        Destroy(a.gameObject);
    }


    //create a random set of anchors
    public void Generate()
    {
        //between 3 and 8
        int amount = Random.Range(3, 9);

        GameObject go;
        Vector2 pos;

        //delete previous anchors
        DeleteAnchors();

        
        //generatenew anchors
        for (int i = 0; i < amount; i++)
        {


            //generate random positions
            pos = new Vector3(Random.Range(-camerabounds.x, camerabounds.x), Random.Range(-camerabounds.y, camerabounds.y),-1f);


            go = Instantiate(Anchorfab, pos, Quaternion.identity);
            Dots.Add(go.GetComponent<Anchor>());
        }

        //generate handles for the current anchors
        AutoHandles();

        //render lines
        DrawBezier();
        
    }


}
