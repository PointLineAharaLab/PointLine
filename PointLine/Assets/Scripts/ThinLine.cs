using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThinLine : MonoBehaviour {

    public int LineId = -1;// line
    public int PointId = -1;// point
    public GameObject Point1, Point2, Point3;
    public  Vector3 Vec1, Vec2, Vec3;
    LineRenderer LR;

    public bool Active;
    public GameObject parent = null;

    Color StandardColor = new Color(0.7f, 0.6f, 0.6f);


    // Use this for initialization
    void Start () {
        Active = true;
        LR = GetComponent<LineRenderer>();

        Vec1 = Vec2 = Vec3 = Vector3.zero;

        LR.SetPosition(0, Vec1);
        LR.SetPosition(1, Vec2);
        LR.SetPosition(2, Vec3);
    }

    // Update is called once per frame
    void Update () {
        //if (AppMgr.pts == null) return;
        //if (AppMgr.lns == null) return;
        gameObject.SetActive(Active);
        ///Debug.Log(PointId);
        if(Point1!=null && Point2 != null && Point3 != null)
        {
            Vec1 = Point1.GetComponent<Point>().transform.position;
            Vec1.z = 0.0f;
            Vec2 = Point2.GetComponent<Point>().transform.position;
            Vec2.z = 0.0f;
            Vec3 = Point3.GetComponent<Point>().transform.position;
            Vec3.z = 0.0f;
        }
        else {
            GameObject[] OBJs = FindObjectsOfType<GameObject>();
            GameObject LineObj = null;
            Line LN = null;
            for (int i=0; i<OBJs.Length; i++)
            {
                LN = OBJs[i].GetComponent<Line>();
                if (LN != null)
                {
                    if(LN.Id == LineId)
                    {
                        LineObj = OBJs[i];
                        break;
                    }
                }
            }
            if(LineObj  == null)
            {
                Active = false;
                LineId = -1;
            }
            else
            {
                Point1 = Point2 = Point3 = null;
                for (int i = 0; i < OBJs.Length; i++)
                {
                    Point PT = OBJs[i].GetComponent<Point>();
                    if (PT != null)
                    {
                        if (PT.Id == PointId)
                        {
                            Point2 = OBJs[i];
                            Vec2 = PT.transform.position;
                            Vec2.z = 0.0f;
                        }
                        if (PT.Id == LN.Point1Id)
                        {
                            Point1 = OBJs[i];
                            Vec1 = PT.transform.position;
                            Vec1.z = 0.0f;
                        }
                        if (PT.Id == LN.Point2Id)
                        {
                            Point3 = OBJs[i];
                            Vec3 = PT.transform.position;
                            Vec3.z = 0.0f;
                        }
                    }
                }
                if(Point1==null || Point2==null || Point3 == null)
                {
                    Active = false;
                }
            }
        }
        LR = GetComponent<LineRenderer>();
        LR.SetPosition(0, Vec1);
        LR.SetPosition(1, Vec2);
        LR.SetPosition(2, Vec3);
        Renderer RD = GetComponent<LineRenderer>();
        RD.material.color = StandardColor;
    }
}
