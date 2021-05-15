using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineManager : MonoBehaviour
{
    // Start is called before the first frame update

    public static SplineManager instance;

    public List<GameObject> DotsGO;
 


    public GameObject Anchorsfab;
    public GameObject Segmentsfab;

    public float defaultMagnitude = 1f;

    public LineRenderer lr;

    public List<LineRenderer> lrs;

    public List<Anchor> Dots;

    public float step = 0.1f;

    public int resolution=10;

    public bool showAnchors;

    public bool anchorsMirrored;



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
        AutoAnchors();
        DrawBezier();
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

    void AutoAnchors()
    {

        //DeleteAnchors();

        Segment prevSeg = null;


        for (int i = 0; i < DotsGO.Count; i++)
        {
            if(i==0)
            {
                //add anchor

               Dots[i].SetAnchor(Instantiate(Anchorsfab, Dots[i].transform.position+(Dots[i+1].transform.position-Dots[i].transform.position).normalized* defaultMagnitude, Quaternion.identity),1);


                Dots[i].SegmentForward = Instantiate(Segmentsfab).GetComponent<Segment>();
                Dots[i].SegmentForward.SetSegment(Dots[i], Dots[i + 1]);

                prevSeg = Dots[i].SegmentForward;


            }
            else if (i == DotsGO.Count-1)
            {
                //add anchor
                Dots[i].SetAnchor(Instantiate(Anchorsfab, Dots[i].transform.position + (Dots[i  - 1].transform.position- Dots[i].transform.position).normalized*defaultMagnitude, Quaternion.identity),-1);

                Dots[i].SegmentBack = prevSeg;
    
            }
            else
            {
                Dots[i].SetAnchor(Instantiate(Anchorsfab, Dots[i].transform.position + (Dots[i + 1].transform.position - Dots[i - 1].transform.position).normalized*defaultMagnitude, Quaternion.identity),1);

                Dots[i].SetAnchor(Instantiate(Anchorsfab, Dots[i].transform.position + (Dots[i - 1].transform.position - Dots[i + 1].transform.position).normalized* defaultMagnitude , Quaternion.identity),-1);

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
            foreach (var item in Dots)
            {
                item.HideAnchors();
            }

        }

        if(Input.GetKeyDown(KeyCode.L))
        {
            foreach (var item in Dots)
            {
                item.ShowAnchors();
            }
        }
    }


}
