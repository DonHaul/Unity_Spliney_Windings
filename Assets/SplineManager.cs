using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineManager : MonoBehaviour
{
    // Start is called before the first frame update


    public List<GameObject> DotsGO;
    public List<GameObject> AnchorsGO;
    public List<GameObject> SegmentsGO;


    public GameObject Anchorsfab;
    public GameObject Segmentsfab;

    public float defaultMagnitude = 1f;

    public LineRenderer lr;

    public List<LineRenderer> lrs;

    public List<Dot> Dots;

    public float step = 0.1f;

    public int resolution=10;

    public bool showAnchors;

    public bool anchorsMirrored;



    // Start is called before the first frame update
    void Awake()
    {
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

        for (int i = 0; i < SegmentsGO.Count; i++)
        {
            lrs[i] = SegmentsGO[i].GetComponent<LineRenderer>();
        }

        for (int i = 0; i < DotsGO.Count; i++)
        {
            Dots.Add( DotsGO[i].GetComponent<Dot>());
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
            Debug.Log(i);
            lrs[i].positionCount = resolution+1;

            //inbetweens

            for (int j = 0; j <= resolution; j++)
            {

                lrs[i].SetPosition(j, BezierPoint(Dots[i].transform.position, Dots[i+1].transform.position,
                    Dots[i].AnchorForward.transform.position, Dots[i+1].AnchorBack.transform.position, (float)j / resolution));

            }

        }
        

    }

    void DeleteAnchors()
    {
        for (int i = 0; i < AnchorsGO.Count; i++)
        {
            Destroy(AnchorsGO[i]);
        }

        AnchorsGO.Clear();
    }

    void AutoAnchors()
    {

        DeleteAnchors();

        GameObject prevSeg = null;


        for (int i = 0; i < DotsGO.Count; i++)
        {
            if(i==0)
            {
                //add anchor

               Dots[i].SetAnchor(Instantiate(Anchorsfab, Dots[i].transform.position+(Dots[i+1].transform.position-Dots[i].transform.position).normalized* defaultMagnitude, Quaternion.identity),1);


                Dots[i].SegmentForward = Instantiate(Segmentsfab);
                SegmentsGO.Add(Dots[i].SegmentForward);
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

                Dots[i].SegmentForward = Instantiate(Segmentsfab);
                SegmentsGO.Add(Dots[i].SegmentForward);

                Dots[i].SegmentBack = prevSeg;
            }

        }

        for (int i = 0; i < SegmentsGO.Count; i++)
        {
            lrs.Add(SegmentsGO[i].GetComponent<LineRenderer>());
        }
    }

    // Update is called once per frame
    Vector2 BezierPoint(Vector2 dot1,Vector2 dot2, Vector2 anch1, Vector2 anch2, float val)
    {
        Debug.Log("Bezier is:");

        Vector2 v = Mathf.Pow((1 - val), 3) * dot1 + 3 * Mathf.Pow((1 - val), 2) * val * anch1 + 3 * (1 - val)*Mathf.Pow(val, 2) * anch2 + Mathf.Pow((val), 3) * dot2;
;
        Debug.Log(v);

            return v;
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
