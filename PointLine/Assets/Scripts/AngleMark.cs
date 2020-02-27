using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleMark : MonoBehaviour {

    public Vector3 Origine;
    public Vector3 UnitX;
    public Vector3 UnitY;

    public int Object1Id = -1;
    public int Object2Id = -1;
    public int Object3Id = -1;
    public GameObject Object1, Object2, Object3;
    public GameObject parent = null;
    public bool Active = true;
    public bool ShowValue = false;
    public float Value = 60f;
    public bool RightAngle = true;
    public GameObject TextObject;

	// Use this for initialization
	void Start () {
        ShowValue = false;
		
	}
	
	// Update is called once per frame
	void Update () {
        gameObject.SetActive(Active);
        GetOrigine();//Origin, UnitX, UnitYを決める
        LineRenderer lr = GetComponent<LineRenderer>();
        if (RightAngle)
        {
            //lr.positionCount = 4;
            lr.SetPosition(0, Origine);
            lr.SetPosition(1, Origine + UnitX);
            lr.SetPosition(2, Origine + UnitX + UnitY);
            lr.SetPosition(3, Origine + UnitY);
            lr.loop = true;
        }
        else // 円弧による角度のマークを描画
             // 180度より小さいほうにマークをつける
        {
            float DeclineX = Mathf.Atan2(UnitX.y, UnitX.x);// UnitXの偏角
            float DeclineY = Mathf.Atan2(UnitY.y, UnitY.x);// UnitYの偏角
            if (DeclineX <= DeclineY && DeclineY < DeclineX + Mathf.PI)
            {
            }
            else if (DeclineX + Mathf.PI <= DeclineY)
            {
                DeclineX += Mathf.PI * 2f;
            }
            else if (DeclineX - Mathf.PI <= DeclineY && DeclineY < DeclineX)
            {
            }
            else if (DeclineY < DeclineX - Mathf.PI)
            {
                DeclineY += Mathf.PI * 2f;
            }
            lr.positionCount = Mathf.CeilToInt(Mathf.Abs(DeclineX - DeclineY) / 0.1f);// 0.1 radian = 6 degree
            for (int i = 0; i < lr.positionCount; i++)
            {
                float t = 1f * i / (lr.positionCount - 1);
                float theta = DeclineX * (1f - t) + DeclineY * (t);
                Vector3 pos = 0.5f * new Vector3(Mathf.Cos(theta), Mathf.Sin(theta), 0f);
                lr.SetPosition(i, Origine + pos);
            }
            lr.loop = false;//ループにしない
        }         
        TextObject.transform.localPosition = Origine + 0.9f * (UnitX+UnitY).normalized;
        TextObject.SetActive(ShowValue);

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
        if (RightAngle)
        {
            if (Object1 != null && Object2 != null)
            {
                Line LN1 = Object1.GetComponent<Line>();
                Line LN2 = Object2.GetComponent<Line>();
                if (LN1 == null || LN2 == null)
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
        else
        {
            if (Object1 != null && Object2 != null && Object3 != null)
            {
                Point Point1 = Object1.GetComponent<Point>();
                Point Point2 = Object2.GetComponent<Point>();
                Point Point3 = Object3.GetComponent<Point>();
                Origine = Point2.Vec;
                UnitX = Point1.Vec - Origine;
                UnitX.Normalize();// 長さ１（不要か？）
                UnitY = Point3.Vec - Origine;
                UnitY.Normalize();// 長さ１（不要か？）
            }
            else
            {
                GameObject[] OBJs = FindObjectsOfType<GameObject>();
                for (int i = 0; i < OBJs.Length; i++)
                {
                    Point PT = OBJs[i].GetComponent<Point>();
                    if (PT != null)
                    {
                        if (PT.Id == Object1Id)
                        {
                            Object1 = OBJs[i];
                        }
                        if (PT.Id == Object2Id)
                        {
                            Object2 = OBJs[i];
                        }
                        if (PT.Id == Object3Id)
                        {
                            Object3 = OBJs[i];
                        }
                    }
                }
                if (Object1 == null || Object2 == null || Object3 == null) Active = false;
            }
        }
    }
}
