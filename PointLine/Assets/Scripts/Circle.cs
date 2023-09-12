using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle : MonoBehaviour {

    public int Id;
    public Vector3 CenterVec;
    public int CenterPointId=-1;
    public GameObject CenterPoint;
    public float Radius;
    public float FixedRadius;
    public float para = 0.1f;
    //float StartAngle;
    //float EndAngle;
    public GameObject parent = null;
    public GameObject GameLog = null;
    public string CircleName = "";

    public bool Selected = false;
    public bool Active = true;
    public bool FixRadius = false;

    Color StandardColor = new Color(0.2f, 0.0f, 0.0f);
    Color SelectedColor = new Color(0.5f, 0.5f, 0.5f);


    // Use this for initialization
    void Start () {
        Active = true;
    }
	
	// Update is called once per frame
	void Update () {
        if (parent != null) parent.SetActive(Active);
        if (AppMgr.pts == null) return;
        Renderer rd = GetComponent<Renderer>();
        if (Selected)
            rd.material.color = SelectedColor;
        else
            rd.material.color = StandardColor;
        if (CenterPoint != null)
        {
            CenterVec = CenterPoint.GetComponent<Point>().transform.position;
            CenterVec.z = 0.0f;
        }
        else
        {
            GameObject[] OBJs = FindObjectsOfType<GameObject>();
            for(int i=0; i<OBJs.Length; i++)
            {
                Point PT = OBJs[i].GetComponent<Point>();
                if (PT != null)
                {
                    if(CenterPointId == PT.Id)
                    {
                        CenterPoint = OBJs[i];
                        break;
                    }
                }
            }
            if (CenterPoint == null) Active = false;
        }

        LineRenderer lr = GetComponent<LineRenderer>();
        int poly = (int)Mathf.Floor(40f * Radius);
        lr.positionCount = poly;
        for (int i=0; i<poly; i++)
        {
            lr.SetPosition(i, new Vector3(CenterVec.x + Radius * Mathf.Cos(Mathf.PI * 2* i / poly), CenterVec.y + Radius * Mathf.Sin(Mathf.PI * 2* i / poly)));
        }
        lr.loop = true;
        //ログの更新
        if (GameLog != null)
        {
            GameLog.GetComponent<Log>().Radius = Radius;
        }
    }

    public static void AllCirclesUnselected()
    {
        Circle[] ci = FindObjectsOfType<Circle>();
        for (int i = 0; i < ci.Length; i++)
        {
            ci[i].Selected = false;
        }
    }

    public static void MakeOneCircleSelected(int MOP)
    {
        Circle[] ci = FindObjectsOfType<Circle>();
        for (int i = 0; i < ci.Length; i++)
        {
            if (ci[i].Id == MOP)
            {
                ci[i].Selected = true;
            }
            else
            {
                ci[i].Selected = false;
            }
        }
    }

    public static void AddOneCircleSelected(int MOP)
    {
        Circle[] ci = FindObjectsOfType<Circle>();
        for (int i = 0; i < ci.Length; i++)
        {
            if (ci[i].Id == MOP)
            {
                ci[i].Selected = true;
            }
        }
    }

    public float ExecuteModule()
    {
        if (Active)
        {
            if (!FixRadius)
            {
                FixedRadius = Radius;
                return 0f;
            }
            else
            {
                float difference = (Radius - FixedRadius) * para;
                //Debug.Log(pt1.Vec + ":" + v1 + ";" + difference);
                Radius -= difference;
                return difference;
            }
        }
        return 0f;
    }



}
