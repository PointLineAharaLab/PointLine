using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleMark : MonoBehaviour {

    public Vector3 Origine;
    public Vector3 UnitX;
    public Vector3 UnitY;

    public int Object1Id = -1;
    public int Object2Id = -1;
    public GameObject Object1, Object2;

    public GameObject parent = null;
    public bool Active = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        gameObject.SetActive(Active);
        GetOrigine();
        LineRenderer lr = GetComponent<LineRenderer>();
        //lr.positionCount = 4;
        lr.SetPosition(0, Origine);
        lr.SetPosition(1, Origine+UnitX);
        lr.SetPosition(2, Origine+UnitX+UnitY);
        lr.SetPosition(3, Origine+UnitY);
        lr.loop = true;

    }

    void  GetIntersection(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
    {
        float s, t;
        float a, b, c, d, p, q;
        //x1+s*(x2-x1) = x3+t*(x4-x3) 
        //y1+s*(y2-y1) = y3+t*(y4-y3)
        // s*(x2-x1) - t*(x4-x3) = x3-x1;
        // s*(y2-y1) - t*(y4-y3) = y3-y1;
        a = x2 - x1;
        b = -x4 + x3;
        c = y2 - y1;
        d = -y4 + y3;
        p = x3 - x1;
        q = y3 - y1;
        s = (p * d - b * q) / (a * d - b * c);
        t = (a * q - p * c) / (a * d - b * c);
        Origine.x = x1 + s * (x2 - x1);
        Origine.y = y1 + s * (y2 - y1);
        Vector3 v = new Vector3(x2 - x1, y2 - y1, 0f);
        v.Normalize();
        if (s < 0.5f)
        {
            UnitX = v * 0.5f;
        }
        else
        {
            UnitX = v * (-0.5f);
        }
        v = new Vector3(x4 - x3, y4 - y3, 0f);
        v.Normalize();
        if (t < 0.5f)
        {
            UnitY = v * 0.5f;
        }
        else
        {
            UnitY = v * (-0.5f);
        }

    }

    void GetOrigine()
    {
        if(Object1 != null && Object2 != null)
        {
            Line LN1 = Object1.GetComponent<Line>();
            Line LN2 = Object2.GetComponent<Line>();
            if(LN1 == null || LN2 == null)
            {
                Active = false;
                return;
            }
            Point p11 = LN1.Point1.GetComponent<Point>();
            Point p12 = LN1.Point2.GetComponent<Point>();
            Point p21 = LN2.Point1.GetComponent<Point>();
            Point p22 = LN2.Point2.GetComponent<Point>();
            if (p11 == null || p12 == null || p21 == null || p22 == null)
            {
                Active = false;
                return;
            }
            GetIntersection(
                p11.Vec.x, p11.Vec.y, 
                p12.Vec.x, p12.Vec.y, 
                p21.Vec.x, p21.Vec.y, 
                p22.Vec.x, p22.Vec.y);
        }
        else
        {
            GameObject[] OBJs = FindObjectsOfType<GameObject>();
            for (int i = 0; i < OBJs.Length; i++)
            {
                Line LN = OBJs[i].GetComponent<Line>();
                if (LN != null)
                {
                    if (LN.Id == Object1Id)
                    {
                        Object1 = OBJs[i];
                    }
                    if (LN.Id == Object2Id)
                    {
                        Object2 = OBJs[i];
                    }
                }
            }
            if (Object1 == null || Object2 == null) Active = false;
        }
    }
}
