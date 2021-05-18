using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MeshMaker : MonoBehaviour
{

    public static MeshMaker instance;

    Mesh m;
    MeshFilter mf;


    [SerializeField]
    List<int> idList;

    [SerializeField]
    Vector3[] vertices;

    [SerializeField]
    Vector2[] vertices2D;

    [SerializeField]
    int[] triIdx;

    [SerializeField]
    int loopThreshold=100;

    public List<Vector2> verts;

    MeshRenderer mr;


    private void Awake()
    {
        instance = this;

       mf = GetComponent<MeshFilter>();
        m = new Mesh();
        mf.mesh = m;

        mr = GetComponent < MeshRenderer>();
    }

    public void TurnOffMesh()
    {
        mr.enabled = false;
    }


    public void Fill(List<Anchor> anchors)
    {
        mr.enabled = true;

        verts = new List<Vector2>();

        //add all points
        for (int i = 0; i < anchors.Count; i++)
        {
            for (int j = 0; j < anchors[i].SegmentForward.v.Length-1; j++)
            {
                Debug.Log("ADDDED");
                verts.Add(anchors[i].SegmentForward.v[j]);
            }
            
        }




        vertices2D = verts.ToArray();
        
        vertices = new Vector3[vertices2D.Length];



        //populate 3d vertices to draw mesh later
        for (int i = 0; i < vertices2D.Length; i++)
        {
            vertices[i] = vertices2D[i];
            
        }
        m.vertices = vertices;


        if (isClockWise(vertices2D) == false)
        {

            Debug.Log("is not clockwise");
            Array.Reverse(vertices);
            Array.Reverse(vertices2D);
        }



        if(isPolygonArea(vertices2D)==false)
        {
            Debug.LogWarning("The current polygon is invalid. Some Edges are intersecting eachother");
            return;
        }
        


        triIdx= Triangulate(vertices2D);
        m.triangles = triIdx;
    }

    T GetItem<T>(List<T> lst, int index)
    {
        
        if (index >= lst.Count)
        {

            //circle back
            return lst[index % lst.Count];
        }
        else if (index < 0)
        {
            return lst[index % lst.Count + lst.Count];
        }
        else
        {
            //Debug.Log(index);
            //Debug.Log(lst.Count);
            return lst[index];
        }
    }

    Rect RectfromSegment(Vector2 a,Vector2 b)
    {

        Rect r = new Rect();
        r.xMin = Math.Min(a.x, b.x);
        r.xMax = Math.Max(a.x, b.x);

        r.yMin = Math.Min(a.y, b.y);
        r.yMax = Math.Max(a.y, b.y);

        return r;
    }

    bool isPolygonArea(Vector2[] v)
    {
     

        int nexti;
        int nextj;

        //iterate every segment
        for (int i = 0; i < v.Length; i++)
        {
            nexti = i + 1;

            if (i == v.Length - 1)
                nexti = 0;


           //iterate every other segment;
            for (int j = i+1; j < v.Length; j++)
            {
                nextj = j + 1;

                if (j == v.Length - 1)
                    nextj = 0;


                if(checkIntersect(v[i], v[nexti], v[j], v[nextj]))
                {
                    return false;
                }

            }
        }

        return true;


    }

    bool checkIntersect(Vector2 a,Vector2 b,Vector2 c,Vector2 d)
    {
    https://stackoverflow.com/questions/3838329/how-can-i-check-if-two-segments-intersect




        //check if rectangles insertsect
        Rect r1 = RectfromSegment(a, b);
        Rect r2 = RectfromSegment(c, d);



      
        //if rectangles do overlap
        if ( r1.Overlaps(r2))
        {
            //check line intersection

            //https://stackoverflow.com/questions/3838329/how-can-i-check-if-two-segments-intersect

            //calculate y=mx+b for both

            float m1 = (a.y - b.y) / (a.x - b.x); //slope
            float m2 = (c.y - d.y) / (c.x - d.x);  //slope
            float b1 = a.y - m1 * a.x;//y in the origin
            float b2 = c.y - m2 * c.x;

            //parallel
            if(m1==m2)
            {
                return false;
            }

            //they meet when m1*x+b1 = m2*x+b2 -- which results in
            float x = (b2 - b1) / (m1 - m2);
            float y = m1 * x + b1;

            //check if point found (x,y) is in rect of other segment (r2)
            if(r2.Contains(new Vector2(x,y)))
            {
                return true;
            }
            


        }


        return false;

        //check if share same y, and same x

    }

    bool isClockWise(Vector2[] v)
    {
        if(v.Length==0)
        {
            return false;
        }

        float sum=0;
        //https://stackoverflow.com/questions/1165647/how-to-determine-if-a-list-of-polygon-points-are-in-clockwise-order
        for (int i = 0; i < v.Length - 1; i++)
        {
           sum+= (v[i + 1].x - v[i].x) * (v[i + 1].y + v[i].y); 
        }
        //edge between first and last
        Debug.Log(v.Length);
        Debug.Log(v[0]);
        Debug.Log(v[v.Length - 1]);
        sum += (v[0].x - v[0].x) * (v[v.Length - 1].y + v[v.Length - 1].y);

        return sum > 0;
    }

    int[] Triangulate(Vector2[] vertices)
    {
        if (vertices == null)
        {
            Debug.LogWarning("Empty array Passed");
        }
        if (vertices.Length < 3)
        {
            Debug.LogWarning("There is a 3 vertices minimum");
        }

        int maxIter = 100000;

        //Check if simple polygon, if no edges cross each other.
        //check if edges that are collinear

        //Debug.Log(isClockWise(vertices));

     

        List<int> indexList = new List<int>();
        for (int i = 0; i < vertices.Length; i++)
        {
            indexList.Add(i);
        }

        Debug.Log("Count is");
        Debug.Log(indexList.Count);


        int triangleIndexCount = 0;

        int TotlaltriangleCount = vertices.Length - 2;

        int[] triangles = new int[TotlaltriangleCount * 3];


        int iter = 0;
        //loop until there is only one triangle left
        while(indexList.Count>3 && iter<maxIter)
        {

            //loop all vertices
            for (int i = 0; i < indexList.Count; i++)
            {
                //current
                int a = indexList[i];

               

                //previous
                int b = GetItem(indexList, i - 1);
                //next
                /*Debug.Log("ITEM WAS:");
                Debug.Log(b);
                */

                int c = GetItem(indexList, i + 1);
               /* Debug.Log("ITEM WAS:");
                Debug.Log(c);
                */
                bool isConvex = CheckConvex(vertices[a], vertices[b], vertices[c]);


                /*Debug.Log(new Vector3Int(b,a, c));
                Debug.Log("Is Convex?");
                Debug.Log(isConvex);*/

                if(isConvex)
                { 
                if (/*CheckConvex(vertices[a],vertices[b],vertices[c]) && */IsNoVertexInTriangle(a,b,c,vertices))
                {


                    //it is an ear
                    //add to tirangles and remove from list
                    triangles[triangleIndexCount++] = b;
                    triangles[triangleIndexCount++] = a;
                    triangles[triangleIndexCount++] = c;

                    //remove one vertex
                    indexList.RemoveAt(i);


                        break;
                }
                }


            }
            iter++;
        }

        if(iter==maxIter)
        {
            Debug.LogWarning("ERROR: maximum iterations reached");
        }


        //add last 3 vertices for final triangle
        triangles[triangleIndexCount++] = indexList[0];
        triangles[triangleIndexCount++] = indexList[1];
        triangles[triangleIndexCount++] = indexList[2];


        return triangles;
    }

    bool IsNoVertexInTriangle(int a,int b, int c,Vector2[] vertices)
    {
        for (int j = 0; j < vertices.Length; j++)
        {
            if (j == a || j == b || j == c)
            {
                continue;
            }


            //must be passed clockwise
            if(  IsInTriangle(vertices[j], vertices[b], vertices[a],vertices[c]))
            {
                return false;
            }
        }
        return true;
    }


    bool IsInTriangle(Vector2 p,Vector2 a, Vector2 b,Vector2 c)
    {
        Vector2 ab = b - a;
        Vector2 bc = c - b;
        Vector2 ca = a - c;

        Vector2 ap = p - a;
        Vector2 bp = p - b;
        Vector2 cp = p - c;

        float cross1 = Vector3.Cross(ab, ap).z;
        float cross2 = Vector3.Cross(bc, bp).z;
        float cross3 = Vector3.Cross(ca, cp).z;

        if(cross1>0 || cross2>0 || cross3>0)
        {
            return false;
        }

        return true;
    }





        bool CheckConvex(Vector2 a, Vector2 b, Vector2 c)
    {
        return Vector3.Cross(b-a,c-a).z > 0;
    }
}
