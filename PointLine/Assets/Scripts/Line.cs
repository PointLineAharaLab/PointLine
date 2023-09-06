using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : Object 
{

    public int Point1Id=-1, Point2Id=-1;
    public int Id = 0;
    public bool Active;

    public GameObject parent = null;
    public GameObject child = null;
    public GameObject GameLog = null;
    public GameObject TextName = null;
    public GameObject Point1 = null, Point2 = null;
    public string LineName="";

    public bool Selected = false;
    public int Isometry = -1;
    Color StandardColor = new Color(0.3f, 0.3f, 0.6f);
    Color SelectedColor = new Color(0.2f, 0.2f, 0.4f); 
    LineRenderer lr;
    public bool Bracket = false;
    public string BracketText = "";
    public enum BracketMarkType : int
    {
        SINGLE = 1,
        DOUBLE = 2,
        CIRCLE = 5,
        TEXT = 10,
        WHITE_BOX = 20
    };
    public int BracketMark = (int)BracketMarkType.SINGLE;


    private Vector3 vec0,vec1;
    public Vector3 Vec0
    {
        get { return vec0; }
        set {
            vec0 = value;
            vec0.z = 0f;
        }
    }
    public Vector3 Vec1
    {
        get { return vec1; }
        set {
            vec1 = value;
            vec1.z = 0f;
        }
    }

    public void SetVec0(float _x, float _y)
    {
        vec0.x = _x;
        vec0.y = _y;
    }
    public void SetVec1(float _x, float _y)
    {
        vec1.x = _x;
        vec1.y = _y;
    }

    // Use this for initialization
    void Start()
    {
        this.thisis = "Line";
        lr = GetComponent<LineRenderer>();

        Vec0 = new Vector3(0, 0, 0);
        Vec1 = new Vector3(0, 0, 0);

        lr.SetPosition(0, Vec0);
        lr.SetPosition(1, Vec1);
        Active = true;
        Isometry = -1;
    }

    // Update is called once per frame
    void Update()
    {
        if (AppMgr.pts == null) return;
        if (parent != null) parent.SetActive(Active);
        if (Point1 != null && Point2 != null)
        {
            Vec0 = Point1.GetComponent<Point>().transform.position;
            vec0.z = 0.0f;
            Vec1 = Point2.GetComponent<Point>().transform.position;
            vec1.z = 0.0f;
        }
        else
        {
            Point1 = Point2 = null;
            GameObject[] OBJs = FindObjectsOfType<GameObject>();
            for (int i = 0; i < OBJs.Length; i++)
            {
                Point PT = OBJs[i].GetComponent<Point>();
                if (PT != null)
                {
                    if (PT.Id == Point1Id)
                    {
                        Point1 = OBJs[i];
                    }
                    if (PT.Id == Point2Id)
                    {
                        Point2 = OBJs[i];
                    }
                }
            }
            if (Point1 == null)
            {
                Point1Id = -1;
                Active = false;
            }
            if (Point2 == null)
            {
                Point2Id = -1;
                Active = false;
            }
        }
        lr.SetPosition(0, Vec0);
        lr.SetPosition(1, Vec1);
        Renderer rd = GetComponent<Renderer>();
        if (Selected)
        {
            if (Isometry>=0)
                rd.material.color = Util.IsometrySelectedColor[Isometry%10];
            else
                rd.material.color = SelectedColor;
        }
        else
        {
            if (Isometry>=0)
                rd.material.color = Util.IsometryColor[Isometry % 10];
            else
                rd.material.color = StandardColor;
        }
    }

    public static void AllLinesUnselected()
    {
        if (AppMgr.lns == null) return;
        for (int i = 0; i < AppMgr.lns.Length; i++)
        {
            AppMgr.lns[i].Selected = false;
        }
    }

    public static void MakeOneLineSelected(int MOP)
    {
        if (AppMgr.lns == null) return;
        for (int i = 0; i < AppMgr.lns.Length; i++)
        {
            if (AppMgr.lns[i].Id == MOP)
            {
                AppMgr.lns[i].Selected = true;
            }
            else
            {
                AppMgr.lns[i].Selected = false;
            }
        }
    }

    public static void AddOneLineSelected(int MOP)
    {
        if (AppMgr.lns == null) return;
        for (int i = 0; i < AppMgr.lns.Length; i++)
        {
            if (AppMgr.lns[i].Id == MOP)
            {
                AppMgr.lns[i].Selected = true;
            }
        }
    }

    public Vector3 GetVector(double _x, double _y)
    {
        double a = Vec1.x - Vec0.x;
        double c = Vec1.y - Vec0.y;
        double b = -c;
        double d = a;
        double n = Math.Sqrt(a * d - c * b);
        if (n == 0.0)
        {
            return Vector3.zero;
        }
        a /= n;
        b /= n;
        c /= n;
        d /= n;
        double p = _x - Vec0.x;
        double q = _y - Vec0.y;
        // as + bt = _x
        // cs + dt = _y
        double s = a * p + c * q;
        double t = b * p + d * q;
        return new Vector3((float)(s / n), (float)t);
    }


    public Vector3 GetNewPosition(Vector3 X)
    {
        double a = Vec1.x - Vec0.x;
        double c = Vec1.y - Vec0.y;
        double b = -c;
        double d = a;
        double n = Math.Sqrt(a * d - c * b);
        if (n == 0.0)
        {
            return Vector3.zero;
        }
        a /= n;
        b /= n;
        c /= n;
        d /= n;
        double p = X.x - Vec0.x;
        double q = X.y - Vec0.y;
        // as + bt = _x
        // cs + dt = _y
        double s = a * p + c * q;
        double t = b * p + d * q;
        t *= 0.8;
        return new Vector3((float)(a * s + b * t + Vec0.x), (float)(c * s + d * t + Vec0.y));
    }

    //void getNewCenter(Circle X)
    //{
    //    double a = Vec1.x - Vec0.x;
    //    double c = Vec1.y - Vec0.y;
    //    double b = -c;
    //    double d = a;
    //    n = sqrt(a * d - c * b);
    //    if (n == 0.0)
    //    {
    //        return;
    //    }
    //    a /= n;
    //    b /= n;
    //    c /= n;
    //    d /= n;
    //    double p = X.Vec0.x - Vec0.x;
    //    double q = X.Vec0.y - Vec0.y;
    //    // as + bt = _x
    //    // cs + dt = _y
    //    double s = a * p + c * q;
    //    double t = b * p + d * q;
    //    double dr = abs(t) - X.radius;
    //    X.radius += 0.25 * dr;
    //    if (t > 0)
    //    {
    //        X.Vec0.x -= 0.25 * dr * b;
    //        X.Vec0.y -= 0.25 * dr * d;
    //    }
    //    else
    //    {
    //        X.Vec0.x += 0.25 * dr * b;
    //        X.Vec0.y += 0.25 * dr * d;
    //    }
    //    return;
    //}


    public bool GetDistance(Vector3 v)
    {
        //Debug.Log("Vec0 = " + Vec0.ToString());
        //Debug.Log("Vec1 = " + Vec1.ToString());
        //Debug.Log("mouse position = " + v.ToString());
        Vector3 st = GetVector(v.x, v.y);
        //Debug.Log("st = " + st.ToString());
        if (0.1 < st.x && st.x < 0.9 && -0.25 < st.y && st.y < 0.25)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}