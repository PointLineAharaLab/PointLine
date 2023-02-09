using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineBracket : MonoBehaviour
{
    public int Point1Id = -1, Point2Id = -1;
    public int Id = 0;
    public bool Active;

    public GameObject parent = null;
    public Line parentLine = null;
    public GameObject Point1 = null;
    public GameObject Point2 = null;
    public Vector3 vec0;
    public Vector3 vec1;
    public Vector3 vec2;
    public Vector3 vec3;
    public float height = 0.7f;
    public string LineName = "";

    //public bool Selected = false;
    //public int Isometry = -1;
    Color StandardColor = new Color(0.5f, 0.5f, 0.8f);
    Color SelectedColor = new Color(0.4f, 0.4f, 0.6f);
    public LineRenderer lr;
    public string BracketText = "";
    public int BracketMark = 0;

    // Start is called before the first frame update
    void Start()
    {
        parentLine = GetComponentInParent<Line>();
        parent = parentLine.gameObject;
        Point1 = parentLine.Point1;
        Point2 = parentLine.Point2;
        lr = GetComponent<LineRenderer>();
        BracketText = parentLine.BracketText;
        BracketMark = parentLine.BracketMark;
    }

    // Update is called once per frame
    void Update()
    {
        if (Point1 != null && Point2 != null)
        {
            vec0 = Point1.GetComponent<Point>().transform.position;
            vec0.z = 0.0f;
            vec3 = Point2.GetComponent<Point>().transform.position;
            vec3.z = 0.0f;
            GetVectors();

        }
    }

    void GetVectors()
    {
        Vector3 ori = (vec0 + vec3) * 0.5f;
        float a = (vec3.x - vec0.x) * 0.5f;
        float c = (vec3.y - vec0.y) * 0.5f ;
        float b = -c;
        float d = a;
        if (ori.x * b + ori.y * d < 0)// ŠO‘¤‚ÉŒü‚¯‚é
        {
            b = -b;
            d = -d;
        }
        float n = Mathf.Sqrt(b * b + d * d);
        b /= (n / height);
        d /= (n / height);
        vec1.x = ori.x + b - a * 0.75f;
        vec1.y = ori.y + d - c * 0.75f;
        vec2.x = ori.x + b + a * 0.75f;
        vec2.y = ori.y + d + c * 0.75f;
        int poly = (int)Mathf.Floor(n * 5f);
        lr.positionCount = poly+1;
        for (int i = 0; i <= poly; i++)
        {
            float t = 1.0f * i / poly;
            Vector3 v01 = (vec0 * (1.0f - t) + vec1 * t);
            Vector3 v12 = (vec1 * (1.0f - t) + vec2 * t);
            Vector3 v23 = (vec2 * (1.0f - t) + vec3 * t);
            Vector3 v012 = (v01 * (1.0f - t) + v12 * t);
            Vector3 v123 = (v12 * (1.0f - t) + v23 * t);
            Vector3 v0123 = (v012 * (1.0f - t) + v123 * t);
            lr.SetPosition(i, v0123);
        }
        lr.loop = false;

    }


}
