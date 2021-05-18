using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/// <summary>
/// This script is highly based on https://www.youtube.com/watch?v=QAdfkylpYwc
/// </summary>
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
    int loopThreshold= 100000;

    public List<Vector2> verts;

    MeshRenderer mr;


    private void Awake()
    {
        //setup singleton
        instance = this;

        //setup mesh
        mf = GetComponent<MeshFilter>();
        m = new Mesh();
        mf.mesh = m;
        mr = GetComponent < MeshRenderer>();
    }


    //turn of mesh, used every time a change is made to the spline
    public void TurnOffMesh()
    {
        mr.enabled = false;
    }

    //will the the inside of the polygon
    //
    public void Fill(List<Anchor> anchors)
    {
        //reenable mesh renderer
        mr.enabled = true;



        verts = new List<Vector2>();

        //cycles every segment and adds every point in them to the verts array
        for (int i = 0; i < anchors.Count; i++)
        {
            //for every point in the segment in front
            for (int j = 0; j < anchors[i].SegmentForward.V.Length-1; j++)
            {
                verts.Add(anchors[i].SegmentForward.V[j]);
            }            
        }

        //convert to array
        vertices2D = verts.ToArray();
        
        vertices = new Vector3[vertices2D.Length];



        //populate 3d vertices to draw mesh later
        for (int i = 0; i < vertices2D.Length; i++)
        {
            vertices[i] = vertices2D[i];
        }
        m.vertices = vertices;


        //if points are clockwise, reverse it
        if (isClockWise(vertices2D) == false)
        {
            Array.Reverse(vertices);
            Array.Reverse(vertices2D);
        }


        //if some lines intersect each other exit
        if(isPolygonArea(vertices2D)==false)
        {
            Debug.LogWarning("The current polygon is invalid. Some Edges are intersecting eachother");
            return;
        }
        
        
        triIdx= Triangulate(vertices2D);
        m.triangles = triIdx;
    }

    //fetch item, if over array length rotates to beginning
    T GetItem<T>(List<T> lst, int index)
    {
       
        if (index >= lst.Count)
        {
            //circle back to beginning
            return lst[index % lst.Count];
        }
        else if (index < 0)
        {
            //circles to the end
            return lst[index % lst.Count + lst.Count];
        }
        else
        {
            return lst[index];
        }
    }

    //creates a  rectangle that encapsulates a segment
    Rect RectfromSegment(Vector2 a,Vector2 b)
    {

        Rect r = new Rect();
        r.xMin = Math.Min(a.x, b.x);
        r.xMax = Math.Max(a.x, b.x);

        r.yMin = Math.Min(a.y, b.y);
        r.yMax = Math.Max(a.y, b.y);

        return r;
    }


    //verifies if vertices form a valide polygon (no intersecting edges)
    bool isPolygonArea(Vector2[] v)
    {
        int nexti;
        int nextj;

        //iterate every segment
        for (int i = 0; i < v.Length; i++)
        {
            //fetch nest vertex
            nexti = i + 1;
            if (i == v.Length - 1)
                nexti = 0;


           //iterate every other segment;
            for (int j = i+1; j < v.Length; j++)
            {

                //fetch next vertex
                nextj = j + 1;
                if (j == v.Length - 1)
                    nextj = 0;


                //verify if they intersect
                if(checkIntersect(v[i], v[nexti], v[j], v[nextj]))
                {
                    return false;
                }

            }
        }

        return true;


    }


    //verify is A-B intersects C-D
    bool checkIntersect(Vector2 a,Vector2 b,Vector2 c,Vector2 d)
    {
    https://stackoverflow.com/questions/3838329/how-can-i-check-if-two-segments-intersect

               
        //form rectanges from bound vertices
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

            //parallel then skip
            if(m1==m2)
            {
                return false;
            }

            //they intersect when m1*x+b1 = m2*x+b2 -- which results in
            float x = (b2 - b1) / (m1 - m2);
            float y = m1 * x + b1;

            //check if point found (x,y) is in rect of other segment (r2)
            if(r2.Contains(new Vector2(x,y)))
            {
                return true;
            }
        }
        return false;

    }

    //check order of vertices based on  https://stackoverflow.com/questions/1165647/how-to-determine-if-a-list-of-polygon-points-are-in-clockwise-order
    bool isClockWise(Vector2[] v)
    {
        if(v.Length==0)
        {
            return false;
        }

        float sum=0;
        
        for (int i = 0; i < v.Length - 1; i++)
        {
           sum+= (v[i + 1].x - v[i].x) * (v[i + 1].y + v[i].y); 
        }
        //edge between first and last
        sum += (v[0].x - v[0].x) * (v[v.Length - 1].y + v[v.Length - 1].y);

        return sum > 0;
    }


    //highly based on     https://www.youtube.com/watch?v=QAdfkylpYwc
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
        while(indexList.Count>3 && iter<loopThreshold)
        {

            //loop all vertices
            for (int i = 0; i < indexList.Count; i++)
            {
                //current
                int a = indexList[i];
                              

                //previous
                int b = GetItem(indexList, i - 1);


                int c = GetItem(indexList, i + 1);

                bool isConvex = CheckConvex(vertices[a], vertices[b], vertices[c]);


                //if convex and no vertex inside , then add triangle
                if(isConvex &&  IsNoVertexInTriangle(a,b,c,vertices))
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
            iter++;
        }

        if(iter==loopThreshold)
        {
            Debug.LogWarning("ERROR: maximum iterations reached");
        }


        //add last 3 vertices for final triangle
        triangles[triangleIndexCount++] = indexList[0];
        triangles[triangleIndexCount++] = indexList[1];
        triangles[triangleIndexCount++] = indexList[2];


        return triangles;
    }


    //Is there a vertex inside the triangle  - https://www.youtube.com/watch?v=QAdfkylpYwc
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

    //check if vertex p, is in triangle abc
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
