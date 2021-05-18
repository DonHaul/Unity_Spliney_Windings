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

    

    [SerializeField]
    Vector2 camerabounds;




    Vector2 backB4;
    Vector2 forwardB4;
    



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

        ControlsManager.instance.ActiveAnchor = Dots[Dots.Count - 1];
    }


    private void DrawLines()
    {
        lr.positionCount = DotsGO.Count;
        for (int i = 0; i < DotsGO.Count; i++)
        {
            lr.SetPosition(i, DotsGO[i].transform.position);
        }

    }

    public  void DrawBezier()
    {
        for (int i = 0; i < Dots.Count-1; i++)
        {
            Dots[i].SegmentForward.RenderBezier();
        }
        

    }
    
    public void DeleteAnchors()
    {
        for (int i = 0; i < Dots.Count; i++)
        {
            Dots[i].Destroy();

        }

        Dots.Clear();
    }

    /// <summary>
    /// Generate Handles for a set of Anchors
    /// </summary>
    void AutoHandles()
    {


        Segment prevSeg = null;


        //for every anchor
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




    public void ToggleSplineClose(bool isOn)
    {
        if(isOn)
        {

            //turn all this into function
            Dots[0].SegmentBack = Instantiate(Segmentsfab).GetComponent<Segment>();

            Dots[0].SegmentBack.SetSegment(Dots[Dots.Count - 1], Dots[0]);

            backB4 = Dots[0].HandleBack.transform.position;
            forwardB4 = Dots[Dots.Count - 1].HandleForward.transform.position;

            //smoothing
            Dots[0].HandleBack.transform.position = ((Dots[0].HandleBack.transform.position - Dots[0].transform.position) + (Dots[Dots.Count - 1].transform.position - Dots[0].transform.position)) / 2 + Dots[0].transform.position;
            Dots[Dots.Count - 1].HandleForward.transform.position = ((Dots[Dots.Count - 1].HandleForward.transform.position - Dots[Dots.Count - 1].transform.position) + (Dots[0].transform.position - Dots[Dots.Count - 1].transform.position)) / 2 + Dots[Dots.Count - 1].transform.position;

            Dots[0].HandleBack.RenderLine();
            Dots[Dots.Count - 1].HandleForward.RenderLine();

            Dots[0].SegmentBack.RenderBezier();
        }
        else
        {
            Destroy(Dots[0].SegmentBack.gameObject);
            Dots[0].HandleBack.transform.position = backB4;
            Dots[Dots.Count - 1].HandleForward.transform.position = forwardB4;
            Dots[0].HandleBack.RenderLine();
            Dots[Dots.Count - 1].HandleForward.RenderLine();
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


        ControlsManager.instance.ActiveAnchor = a;


        a.SegmentForward = seg;
        a.SegmentForward.anchorback = a;

        a.SegmentForward.RenderBezier();
        a.SegmentForward.SetupCollider();

    }

    public void CreateAnchor(Vector2 pos)
    {

        if (ControlsManager.instance.SplineCloseOn)
        {Destroy(Dots[0].SegmentBack.gameObject);

        }

        Anchor a = Instantiate(Anchorfab,new Vector3 (pos.x,pos.y,-1f), Quaternion.identity).GetComponent<Anchor>();
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



        a.SegmentBack.SetSegment(ControlsManager.instance.ActiveAnchor, a);
        a.SegmentBack.RenderBezier();
        a.SegmentBack.SetupCollider();


        //connect previous to this
        a.SegmentBack.anchorback.SegmentForward = a.SegmentBack;

        ControlsManager.instance.ActiveAnchor = a;

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

    public void Generate()
    {
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
