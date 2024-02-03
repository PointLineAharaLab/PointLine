using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module : MonoBehaviour {

    public int Type=0;//[1]
    public int Object1Id = -1;//[2]
    public int Object2Id = -1;//[3]
    public int Object3Id = -1;//[4]
    public int Id=-1;//[5]
    public bool Active = true;//[6]
    public float Ratio1 = 1f;//[7]
    public float Ratio2 = 1f;//[8]
    public float Constant = Mathf.PI/2f;//[9]
    public bool ShowConstant = false;//[10]
    public bool FixAngle = false;//角度を固定するかどうかのフラグ//[11]
    public bool FixRatio = true;//比を固定するかどうかのフラグ//[12]
    public float Parameter = 0.1f;//[13]
    public float ParaWeight = 0.2f;//[14]
    public string ModuleName = "";//[15]
    public int Object4Id = -1;//[16]
    public int PolygonOption = 0;//[17]
    public float err=0f;

    public GameObject Object1, Object2, Object3, Object4;

    public GameObject parent = null;
    public GameObject GameLog = null;


    public Vector3 PreVec;// 軌跡を描くための変数

    // Use this for initialization
    void Start () {
        Active = true;
        //FixAngle = false;
        //FixRatio = true;
    }

    #region Point2Point
    private float ModulePOINT_ON_POINT()
    {        //二つの頂点を1つにする
        if(Object1 != null && Object2 != null)
        {
            Point p1 = Object1.GetComponent<Point>();
            Point p2 = Object2.GetComponent<Point>();
            if (p1 == null || p2 == null)
            {
                Active = false;
                return 0f;
            }
            Vector3 v1 = p1.Vec;
            Vector3 v2 = p2.Vec;
            Vector3 v1New = (1f-Parameter) * v1 + Parameter * v2;
            Vector3 v2New = Parameter * v1 + (1f - Parameter) * v2;
            float err = 0;
            if (!p1.Fixed)
            {
                p1.Vec = v1New;
                err += (p1.Vec - v1New).magnitude;
            }
            if (!p2.Fixed)
            {
                p2.Vec = v2New;
                err += (p2.Vec - v2New).magnitude;
            }
            return err;
        }
        else 
        {
            GameObject[] OBJs = FindObjectsOfType<GameObject>();
            for(int i=0; i<OBJs.Length; i++)
            {
                Point PT = OBJs[i].GetComponent<Point>();
                if(PT != null)
                {
                    if(PT.Id == Object1Id)
                    {
                        Object1 = OBJs[i];
                    }
                    if (PT.Id == Object2Id)
                    {
                        Object2 = OBJs[i];
                    }
                }
            }
            if (Object1 == null || Object2 == null) Active = false;
            return 0f;
        }
    }
    #endregion

    #region PointOnLine
    private static Vector3 Rotate90(Vector3 V)
    {
        return new Vector3(-V.y, V.x, 0f);
    }

    public static float GetDistanceOfLineNPoint(float x0, float y0, float x1, float y1, float x2, float y2)
    {// (x0,y0)-(x1,y1): line
        // (x2,y2) : point
        float a = x1 - x0;
        float c = y1 - y0;
        float b = -c;
        float d = a;
        float n = Mathf.Sqrt(a * d - c * b);
        if (n < 0.0001f)
        {
            return 0f;
        }
        a /= n;
        b /= n;
        c /= n;
        d /= n;
        float p = x2 - x0;
        float q = y2 - y0;
        // as + bt = p
        // cs + dt = q
        //float s = a * p + c * q;
        //float t = b * p + d * q;
        float ret = b * p + d * q;
        return ret;
    }

    private float ModulePOINT_ON_LINE()
    {
        // 点を直線の上に載せる。
        if (parent != null) parent.SetActive(Active);
        if (Object1 != null && Object2 != null)
        {
            Point p1 = Object1.GetComponent<Point>();
            Line l2 = Object2.GetComponent<Line>();
            if (p1 == null || l2 == null)
            {
                Active = false;
                return 0f;
            }
            Point p21 = l2.Point1.GetComponent<Point>();
            Point p22 = l2.Point2.GetComponent<Point>();
            if (p21 == null || p22 == null)
            {
                Active = false;
                return 0f;
            }
            float InnerT = Vector3.Dot(p1.Vec - p21.Vec, p22.Vec - p21.Vec) / Vector3.Dot(p22.Vec - p21.Vec, p22.Vec - p21.Vec);
            float err = 0f;
            if (0f <= InnerT && InnerT <= 1f)
            {
                float Distance;
                Vector3 DVec, PVec;
                DVec = Rotate90(p22.Vec - p21.Vec);
                DVec.Normalize();
                PVec = p1.Vec - p21.Vec;
                Distance = Vector3.Dot(DVec, PVec) * Parameter;

                if (!p1.Fixed)
                {// 点を直線に近づける
                    p1.Vec -= (Distance * 0.5f) * DVec;
                    err += Mathf.Abs(Distance) * 0.5f;
                }
                if (!p21.Fixed)
                { //直線を点に近づける
                    p21.Vec += (Distance * ParaWeight * 0.5f) * DVec;
                    err += Mathf.Abs(Distance * ParaWeight) * 0.5f;
                }
                if (!p22.Fixed)
                { //直線を点に近づける
                    p22.Vec += (Distance * ParaWeight * 0.5f) * DVec;
                    err += Mathf.Abs(Distance * ParaWeight) * 0.5f;
                }
            }
            else if (InnerT < 0f)
            {
                // p21 を 線分p1-p22へ近づける
                float Distance;
                Vector3 DVec, PVec;
                DVec = Rotate90(p22.Vec - p1.Vec);
                DVec.Normalize();
                PVec = p21.Vec - p1.Vec;
                Distance = Vector3.Dot(DVec, PVec) * Parameter;

                if (!p21.Fixed)
                {// 点を直線に近づける
                    p21.Vec -= (Distance * 0.5f) * DVec;
                    err += Mathf.Abs(Distance) * 0.5f;
                }
                if (!p1.Fixed)
                { //直線を点に近づける
                    p1.Vec += (Distance * 0.5f) * DVec;
                    err += Mathf.Abs(Distance) * 0.5f;
                }
                if (!p22.Fixed)
                { //直線を点に近づける
                    p22.Vec += (Distance * 0.5f) * DVec;
                    err += Mathf.Abs(Distance) * 0.5f;
                }
            }
            else if (1f < InnerT) {
                // p22 を 線分p21-p1へ近づける
                float Distance;
                Vector3 DVec, PVec;
                DVec = Rotate90(p1.Vec - p21.Vec);
                DVec.Normalize();
                PVec = p22.Vec - p21.Vec;
                Distance = Vector3.Dot(DVec, PVec) * Parameter;

                if (!p22.Fixed)
                {// 点を直線に近づける
                    p22.Vec -= (Distance * 0.5f) * DVec;
                    err += Mathf.Abs(Distance) * 0.5f;
                }
                if (!p21.Fixed)
                { //直線を点に近づける
                    p21.Vec += (Distance * 0.5f) * DVec;
                    err += Mathf.Abs(Distance) * 0.5f;
                }
                if (!p1.Fixed)
                { //直線を点に近づける
                    p1.Vec += (Distance * 0.5f) * DVec;
                    err += Mathf.Abs(Distance) * 0.5f;
                }
            }
            return err;
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
                }
                Line LN = OBJs[i].GetComponent<Line>();
                if (LN != null)
                {
                    if (LN.Id == Object2Id)
                    {
                        Object2 = OBJs[i];
                    }
                }
            }
            if (Object1 == null || Object2 == null)
            {
                Active = false;
            }
            return 0f;
        }
    }
    #endregion

    #region PointOnCircle
    private float  ModulePOINT_ON_CIRCLE()
    {        // 点を円の上に載せる
        
        Point p1 = null, p21 = null;
        Circle c2 = null;
        if (parent != null) parent.SetActive(Active);
        if (Object1 != null && Object2 != null)
        {
            p1 = Object1.GetComponent<Point>();
            c2 = Object2.GetComponent<Circle>();
            p21 = c2.CenterPoint.GetComponent<Point>();
            if (p1 == null || c2 == null || p21 == null)
            {
                Active = false;
                return 0f;
            }
            float dist = Vector3.Distance(p21.Vec, p1.Vec);
            float rad = c2.Radius;
            float delta = (dist - rad) * Parameter * 0.33f;
            float err = Mathf.Abs(delta);
            float radNew = rad + delta;
            c2.Radius = radNew;
            Vector3 v2 = p1.Vec - p21.Vec;
            v2.Normalize();
            if (!p1.Fixed)
            {
                p1.Vec = p1.Vec - delta * v2;
                err += Mathf.Abs(delta);
            }
            if (!p21.Fixed)
            { 
                p21.Vec = p21.Vec + delta * v2;
                err += Mathf.Abs(delta);
            }
            return err;
        }
        else
        {
            GameObject[] OBJs = FindObjectsOfType<GameObject>();
            for (int i = 0; i < OBJs.Length; i++)
            {
                Point PT = OBJs[i].GetComponent<Point>();
                if(PT != null)
                {
                    if(PT.Id == Object1Id)
                    {
                        Object1 = OBJs[i];
                    }
                }
                Circle CI = OBJs[i].GetComponent<Circle>();
                if(CI != null)
                {
                    if(CI.Id == Object2Id)
                    {
                        Object2 = OBJs[i];
                    }
                }
            }
            if(Object1 == null || Object2 == null)
            {
                Active = false;
            }
            return 0f;
        }
    }
    #endregion


    #region LINES_ISOMETRY
    private float ModuleLINES_ISOMETRY()
    {
        gameObject.SetActive(Active);
        if(Object1 != null && Object2 != null)
        {
            Line LN1 = Object1.GetComponent<Line>();
            Line LN2 = Object2.GetComponent<Line>();
            if(LN1 == null || LN2 == null)
            {
                Active = false;
                return 0f;
            }
            Point p11 = LN1.Point1.GetComponent<Point>();
            Point p12 = LN1.Point2.GetComponent<Point>();
            Point p21 = LN2.Point1.GetComponent<Point>();
            Point p22 = LN2.Point2.GetComponent<Point>();
            if(p11 == null|| p12 == null|| p21 == null||p22 == null)
            {
                Active = false;
                return 0f;
            }
            Vector3 VecA, VecB;
            float NormA, NormB, Delta;
            float err = 0f;
            VecA = p12.Vec - p11.Vec;
            VecB = p22.Vec - p21.Vec;
            NormA = VecA.magnitude;
            NormB = VecB.magnitude;
            VecA.Normalize();
            VecB.Normalize();
            //Delta = (NormB - NormA) * 0.125f;//等長
            Delta = (NormB * Ratio1 - NormA * Ratio2) / (Ratio1 + Ratio2) * Parameter;//Ratioの値によって変わる
            //  線分の長さを等しくする
            if (FixRatio)// 比が固定されていれば
            {
                if (!p11.Fixed)
                {
                    p11.Vec -= (Delta * Ratio2 / (Ratio1 + Ratio2)) * VecA;
                    err += Mathf.Abs(Delta * Ratio2 / (Ratio1 + Ratio2));
                }
                if (!p21.Fixed) {
                    p21.Vec += (Delta * Ratio1 / (Ratio1 + Ratio2)) * VecB;
                    err += Mathf.Abs(Delta * Ratio1 / (Ratio1 + Ratio2));
                }
                if (!p12.Fixed)
                {
                    p12.Vec += (Delta * Ratio2 / (Ratio1 + Ratio2)) * VecA;
                    err += Mathf.Abs(Delta * Ratio2 / (Ratio1 + Ratio2));
                }
                if (!p22.Fixed)
                {
                    p22.Vec -= (Delta * Ratio1 / (Ratio1 + Ratio2)) * VecB;
                    err += Mathf.Abs(Delta * Ratio1 / (Ratio1 + Ratio2));
                }
                return err;
            }
            else
            {
                Ratio1 = NormA;
                Ratio2 = NormB;
                return 0f;
            }
            
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
            return 0f;
        }
    }
    #endregion

    #region LINES_PERPENDICULAR
    private float ModuleLINES_PERPENDICULAR()
    {
        gameObject.SetActive(Active);
        if (Object1 != null && Object2 != null)
        {
            Line LN1 = Object1.GetComponent<Line>();
            Line LN2 = Object2.GetComponent<Line>();
            if (LN1 == null || LN2 == null)
            {
                Active = false;
                return 0f;
            }
            Point p11 = LN1.Point1.GetComponent<Point>();
            Point p12 = LN1.Point2.GetComponent<Point>();
            Point p21 = LN2.Point1.GetComponent<Point>();
            Point p22 = LN2.Point2.GetComponent<Point>();
            if (p11 == null || p12 == null || p21 == null || p22 == null)
            {
                Active = false;
                return 0f;
            }
            float x11 = p11.Vec.x;
            float y11 = p11.Vec.y;
            float x12 = p12.Vec.x;
            float y12 = p12.Vec.y;
            float x21 = p21.Vec.x;
            float y21 = p21.Vec.y;
            float x22 = p22.Vec.x;
            float y22 = p22.Vec.y;
            float theta1 = Mathf.Atan2(y12 - y11, x12 - x11);
            float theta2 = Mathf.Atan2(y22 - y21, x22 - x21);
            float Delta = theta2 - theta1;
            if (Delta > Mathf.PI)
                Delta -= Mathf.PI * 3 / 2;
            else if (Delta > 0)
                Delta -= Mathf.PI / 2;
            else if (Delta > -Mathf.PI)
                Delta += Mathf.PI / 2;
            else if (Delta >= -Mathf.PI * 2)
                Delta += Mathf.PI * 3 / 2;
            Delta *= Parameter;
            float CosDelta = Mathf.Cos(Delta);
            float SinDelta = Mathf.Sin(Delta);
            float x1c = (x11 + x12) * 0.5f;
            float y1c = (y11 + y12) * 0.5f;
            float x2c = (x21 + x22) * 0.5f;
            float y2c = (y21 + y22) * 0.5f;
            Vector3 NewVec = Vector3.zero;
            float err = 0f;
            NewVec.x = (x11 - x1c) * CosDelta - (y11 - y1c) * SinDelta + x1c;
            NewVec.y = +(x11 - x1c) * SinDelta + (y11 - y1c) * CosDelta + y1c;
            if (!p11.Fixed)
            {
                p11.Vec = NewVec;
                err += Util.Magnitude(x11 - x1c, y11 - y1c) * Mathf.Abs(Delta);
            }
            NewVec.x = (x12 - x1c) * CosDelta - (y12 - y1c) * SinDelta + x1c;
            NewVec.y = +(x12 - x1c) * SinDelta + (y12 - y1c) * CosDelta + y1c;
            if (!p12.Fixed)
            {
                p12.Vec = NewVec;
                err += Util.Magnitude(x12 - x1c, y12 - y1c) * Mathf.Abs(Delta);
            }
            NewVec.x = (x21 - x2c) * CosDelta + (y21 - y2c) * SinDelta + x2c;
            NewVec.y = -(x21 - x2c) * SinDelta + (y21 - y2c) * CosDelta + y2c;
            if (!p21.Fixed)
            {
                p21.Vec = NewVec;
                err += Util.Magnitude(x21 - x2c, y21 - y2c) * Mathf.Abs(Delta);
            }
            NewVec.x = (x22 - x2c) * CosDelta + (y22 - y2c) * SinDelta + x2c;
            NewVec.y = -(x22 - x2c) * SinDelta + (y22 - y2c) * CosDelta + y2c;
            if (!p22.Fixed)
            {
                p22.Vec = NewVec;
                err += Util.Magnitude(x22 - x2c, y22 - y2c) * Mathf.Abs(Delta);
            }
            return err;
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
            return 0f;
        }
    }
    #endregion

    #region LINES_PARALLEL
    private float ModuleLINES_PARALLEL()
    {//直線を平行にする
        gameObject.SetActive(Active);
        if (Object1 != null && Object2 != null)
        {
            Line LN1 = Object1.GetComponent<Line>();
            Line LN2 = Object2.GetComponent<Line>();
            if (LN1 == null || LN2 == null)
            {
                Active = false;
                return 0f;
            }
            Point p11 = LN1.Point1.GetComponent<Point>();
            Point p12 = LN1.Point2.GetComponent<Point>();
            Point p21 = LN2.Point1.GetComponent<Point>();
            Point p22 = LN2.Point2.GetComponent<Point>();
            if (p11 == null || p12 == null || p21 == null || p22 == null)
            {
                Active = false;
                return 0f;
            }
            float x11 = p11.Vec.x;
            float y11 = p11.Vec.y;
            float x12 = p12.Vec.x;
            float y12 = p12.Vec.y;
            float x21 = p21.Vec.x;
            float y21 = p21.Vec.y;
            float x22 = p22.Vec.x;
            float y22 = p22.Vec.y;
            float theta1 = Mathf.Atan2(y12 - y11, x12 - x11);
            float theta2 = Mathf.Atan2(y22 - y21, x22 - x21);
            float Delta = theta2 - theta1;
            if (Delta > Mathf.PI * 3 / 2)
                Delta -= Mathf.PI * 2;
            else if (Delta > Mathf.PI / 2)
                Delta -= Mathf.PI;
            else if (Delta > -Mathf.PI / 2)
                Delta += 0;
            else if (Delta >= -Mathf.PI * 3 / 2)
                Delta += Mathf.PI;
            else
                Delta += Mathf.PI * 2;
            Delta *= Parameter;
            float x1c = (x11 + x12) * 0.5f;
            float y1c = (y11 + y12) * 0.5f;
            float x2c = (x21 + x22) * 0.5f;
            float y2c = (y21 + y22) * 0.5f;
            float err = 0f;
            float magLN1 = Util.Magnitude(x11 - x12, y11 - y12);
            float magLN2 = Util.Magnitude(x21 - x22, y21 - y22);
            Vector3 NewVec = Vector3.zero;
            float CosDelta = Mathf.Cos(Delta * magLN2 / (magLN1 + magLN2));
            float SinDelta = Mathf.Sin(Delta * magLN2 / (magLN1 + magLN2));
            NewVec.x = (x11 - x1c) * CosDelta - (y11 - y1c) * SinDelta + x1c;
            NewVec.y = +(x11 - x1c) * SinDelta + (y11 - y1c) * CosDelta + y1c;
            if (!p11.Fixed)
            {
                p11.Vec = NewVec;
                err += Util.Magnitude(x11 - x1c, y11 - y1c) * Mathf.Abs(Delta);
            }
            NewVec.x = (x12 - x1c) * CosDelta - (y12 - y1c) * SinDelta + x1c;
            NewVec.y = +(x12 - x1c) * SinDelta + (y12 - y1c) * CosDelta + y1c;
            if (!p12.Fixed) {
                p12.Vec = NewVec;
                err += Util.Magnitude(x12 - x1c, y12 - y1c) * Mathf.Abs(Delta);
            }
            CosDelta = Mathf.Cos(Delta * magLN1 / (magLN1 + magLN2));
            SinDelta = Mathf.Sin(Delta * magLN1 / (magLN1 + magLN2));
            NewVec.x = (x21 - x2c) * CosDelta + (y21 - y2c) * SinDelta + x2c;
            NewVec.y = -(x21 - x2c) * SinDelta + (y21 - y2c) * CosDelta + y2c;
            if (!p21.Fixed)
            {
                p21.Vec = NewVec;
                err += Util.Magnitude(x21 - x2c, y21 - y2c) * Mathf.Abs(Delta);
            }
            NewVec.x = (x22 - x2c) * CosDelta + (y22 - y2c) * SinDelta + x2c;
            NewVec.y = -(x22 - x2c) * SinDelta + (y22 - y2c) * CosDelta + y2c;
            if (!p22.Fixed)
            {
                p22.Vec = NewVec;
                err += Util.Magnitude(x22 - x2c, y22 - y2c) * Mathf.Abs(Delta);
            }
            return err;
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
        return 0f;
    }
    #endregion

    #region LINE_HORIZONTAL
    private float ModuleLINE_HORIZONTAL()
    {//直線を平行にする
        gameObject.SetActive(Active);
        if (Object1 != null)
        {
            Line LN1 = Object1.GetComponent<Line>();
            if (LN1 == null)
            {
                Active = false;
                return 0f;
            }
            Point p11 = LN1.Point1.GetComponent<Point>();
            Point p12 = LN1.Point2.GetComponent<Point>();
            if (p11 == null || p12 == null)
            {
                Active = false;
                return 0f;
            }
            float x11 = p11.Vec.x;
            float y11 = p11.Vec.y;
            float x12 = p12.Vec.x;
            float y12 = p12.Vec.y;
            float theta1 = -Mathf.Atan2(y12 - y11, x12 - x11);
            float Delta = theta1;
            if (Delta > Mathf.PI / 2)
                Delta -= Mathf.PI;
            else if (Delta > -Mathf.PI / 2)
                Delta += 0;
            else if (Delta >= -Mathf.PI * 3 / 2)
                Delta += Mathf.PI;
            Delta *= Parameter;
            float CosDelta = Mathf.Cos(Delta);
            float SinDelta = Mathf.Sin(Delta);
            float x1c = (x11 + x12) * 0.5f;
            float y1c = (y11 + y12) * 0.5f;
            float err = 0f;
            Vector3 NewVec = Vector3.zero;
            NewVec.x = (x11 - x1c) * CosDelta - (y11 - y1c) * SinDelta + x1c;
            NewVec.y = +(x11 - x1c) * SinDelta + (y11 - y1c) * CosDelta + y1c;
            if (!p11.Fixed)
            {
                p11.Vec = NewVec;
                err += Util.Magnitude(x11 - x1c, y11 - y1c) * Mathf.Abs(Delta);
            }
            NewVec.x = (x12 - x1c) * CosDelta - (y12 - y1c) * SinDelta + x1c;
            NewVec.y = +(x12 - x1c) * SinDelta + (y12 - y1c) * CosDelta + y1c;
            if (!p12.Fixed)
            {
                p12.Vec = NewVec;
                err += Util.Magnitude(x12 - x1c, y12 - y1c) * Mathf.Abs(Delta);
            }
            return err;
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
                }
            }
            if (Object1 == null) Active = false;
        }
        return 0f;
    }
    #endregion

    #region CIRCLE_TANGENT_LINE
    private float ModuleCIRCLE_TANGENT_LINE()
    {//円を直線に接させる
        //Object1Id : 円：
        //Object2Id : 直線
        gameObject.SetActive(Active);
        if (Object1 != null && Object2 != null)
        {
            Circle CI1 = Object1.GetComponent<Circle>();
            Line LN2 = Object2.GetComponent<Line>();
            if (CI1 == null || LN2 == null)
            {
                Active = false;
                return 0f;
            }
            Point p11 = CI1.CenterPoint.GetComponent<Point>();
            Point p21 = LN2.Point1.GetComponent<Point>();
            Point p22 = LN2.Point2.GetComponent<Point>();
            if (p11 == null || p21 == null || p22 == null)
            {
                Active = false;
                return 0f;
            }
            float Radius = CI1.Radius;
            Vector3 DVec = Rotate90(p22.Vec - p21.Vec);
            DVec.Normalize();
            Vector3 PVec = p11.Vec - p21.Vec;
            float Distance = Vector3.Dot(DVec, PVec);
            if (Distance < 0)
            {
                DVec *= -1f;
                Distance *= -1f;
            }
            float Delta = (Distance - Radius) * Parameter;
            float err = 0f;
            // 円の中心を直線に近づける
            if (!p11.Fixed)
            {
                p11.Vec -= (Delta * 0.33f)* DVec;
                err += Mathf.Abs(Delta * 0.33f);
            }
            //円の半径を適切に変化させる
            CI1.Radius += Delta;
            err += Mathf.Abs(Delta * 0.33f);
            // 直線を円に近づける
            if (!p21.Fixed)
            {
                p21.Vec += (Delta * 0.33f) * DVec;
                err += Mathf.Abs(Delta * 0.33f);
            }
            if (!p22.Fixed)
            {
                p22.Vec += (Delta * 0.33f) * DVec;
                err += Mathf.Abs(Delta * 0.33f);
            }
            return err;
        }
        else
        {
            GameObject[] OBJs = FindObjectsOfType<GameObject>();
            for (int i = 0; i < OBJs.Length; i++)
            {
                Circle CI = OBJs[i].GetComponent<Circle>();
                if (CI != null)
                {
                    if (CI.Id == Object1Id)
                    {
                        Object1 = OBJs[i];
                    }
                }
                Line LN = OBJs[i].GetComponent<Line>();
                if (LN != null)
                {
                    if (LN.Id == Object2Id)
                    {
                        Object2 = OBJs[i];
                    }
                }
            }
            if (Object1 == null || Object2 == null) Active = false;
        }
        return 0f;
    }

    #endregion

    #region CIRCLE_TANGENT_CIRCLE
    /// <summary>円を円に（外）接させる,    Object1Id : 円１,    Object2Id : 円２    円と中心を探す</summary>
    private float ModuleCIRCLE_TANGENT_CIRCLE()
    {
        gameObject.SetActive(Active);
        if (Object1 != null && Object2 != null)
        {
            Circle CI1 = Object1.GetComponent<Circle>();
            Circle CI2 = Object2.GetComponent<Circle>();
            if (CI1 == null || CI2 == null)
            {
                Active = false;
                return 0f;
            }
            Point p11 = CI1.CenterPoint.GetComponent<Point>();
            Point p21 = CI2.CenterPoint.GetComponent<Point>();
            if (p11 == null || p21 == null)
            {
                Active = false;
                return 0f;
            }
            Vector3 c1c = p11.Vec;
            float c1r = CI1.Radius;
            Vector3 c2c = p21.Vec;
            float c2r = CI2.Radius;
            Vector3 DVec12 = c2c - c1c;
            float normD = DVec12.magnitude;
            DVec12.Normalize();
            Vector3 DVec21 = -1f * DVec12;
            float DeltaIn = normD - Mathf.Abs(c1r - c2r);
            float DeltaOut = normD - (c1r + c2r);
            float Delta = 0f;
            float err = 0f;
            if (Mathf.Abs(DeltaIn) > Mathf.Abs(DeltaOut))
            {// 外接していると思われる
                Delta = DeltaOut * Parameter * 0.25f;
                //円１の中心を動かす
                if (!p11.Fixed)
                {
                    p11.Vec += Delta * DVec12;
                    err += Mathf.Abs(Delta);
                }
                //円２の中心を動かす
                if (!p21.Fixed)
                {
                    p21.Vec += Delta * DVec21;
                    err += Mathf.Abs(Delta);
                }
                //円１の半径を調整する
                CI1.Radius += Delta;
                err += Mathf.Abs(Delta);
                //円２の半径を調整する
                CI2.Radius += Delta;
                err += Mathf.Abs(Delta);
                return err;
            }
            else
            {//内接していると思われる
                //Debug.Log("inscribed");
                bool C1out = false;
                if (c1r < c2r)
                {//C2が外側だと思われる
                    C1out = false;
                }
                else
                {
                    C1out = true;
                }
                Delta = DeltaIn * Parameter * 0.25f;
                //円１の中心を動かす
                if (!p11.Fixed)
                {
                    p11.Vec += Delta * DVec12;
                    err += Mathf.Abs(Delta);
                }
                //円２の中心を動かす
                if (!p21.Fixed)
                {
                    p21.Vec += Delta * DVec21;
                    err += Mathf.Abs(Delta);
                }
                //円１の半径を調整する
                if (C1out)
                {
                    CI1.Radius += Delta;
                    err += Mathf.Abs(Delta);
                }
                else
                {
                    CI1.Radius -= Delta;
                    err += Mathf.Abs(Delta);
                }
                //円２の半径を調整する
                if (C1out)
                {
                    CI2.Radius -= Delta;
                    err += Mathf.Abs(Delta);
                }
                else
                {
                    CI2.Radius += Delta;
                    err += Mathf.Abs(Delta);
                }
                return err;
            }
        }
        else
        {
            GameObject[] OBJs = FindObjectsOfType<GameObject>();
            for (int i = 0; i < OBJs.Length; i++)
            {
                Circle CI = OBJs[i].GetComponent<Circle>();
                if (CI != null)
                {
                    if (CI.Id == Object1Id)
                    {
                        Object1 = OBJs[i];
                    }
                    if (CI.Id == Object2Id)
                    {
                        Object2 = OBJs[i];
                    }
                }
            }
            if (Object1 == null || Object2 == null) Active = false;
        }
        return 0f;
    }
    #endregion


    private float ModuleADD_MIDPOINT()
    {
        int i1 = -1, i2 = -1, i3 = -1;
        if (AppMgr.pts == null) return 0f;
        for (int i = 0; i < AppMgr.pts.Length; i++)
        {
            if (AppMgr.pts[i].Id == Object1Id)
            {
                i1 = i;
            }
            if (AppMgr.pts[i].Id == Object2Id)
            {
                i2 = i;
            }
            if (AppMgr.pts[i].Id == Object3Id)
            {
                i3 = i;
            }
        }
        if(i1>=0 && i2>=0 && i3 >= 0)
        {
            float err = 0f;
            if (Ratio1 == Ratio2 || Ratio1 == -Ratio2) { 
                Vector3 v1 = AppMgr.pts[i1].Vec;
                Vector3 v2 = AppMgr.pts[i2].Vec;
                Vector3 v3 = AppMgr.pts[i3].Vec;
                Vector3 NewV1 = - v2 + 2.0f * v3;
                Vector3 NewV2 = - v1 + 2.0f * v3;
                Vector3 NewV3 = 0.5f * v1 + 0.5f * v2;
                NewV1 = Parameter * ParaWeight * NewV1 + (1.0f - Parameter * ParaWeight) * v1;
                NewV2 = Parameter * ParaWeight * NewV2 + (1.0f - Parameter * ParaWeight) * v2;
                NewV3 = Parameter * NewV3 + (1.0f - Parameter) * v3;

                 
                if (!AppMgr.pts[i1].Fixed)
                {
                    AppMgr.pts[i1].Vec = NewV1;
                    err += (v1 - NewV1).magnitude;
                }
                if (!AppMgr.pts[i2].Fixed)
                {
                    AppMgr.pts[i2].Vec = NewV2;
                    err += (v2 - NewV2).magnitude;
                }
                if (!AppMgr.pts[i3].Fixed)
                {
                    AppMgr.pts[i3].Vec = NewV3;
                    err += (v3 - NewV3).magnitude;
                }
                return err;
            }
            else
            {
                Vector3 v1 = AppMgr.pts[i1].Vec;
                Vector3 v2 = AppMgr.pts[i2].Vec;
                Vector3 v3 = AppMgr.pts[i3].Vec;
                Vector3 NewV1 = (Ratio2==0)? v1 : (-Ratio1 * v2 + (Ratio2 + Ratio1) * v3) / Ratio2;
                Vector3 NewV2 = (Ratio1==0)? v2 : (-Ratio2 * v1 + (Ratio2 + Ratio1) * v3) / Ratio1;
                Vector3 NewV3 = (Ratio2 * v1 + Ratio1 * v2) / (Ratio2 + Ratio1);
                float Delta = Parameter;
                float Epsilon = 1.0f - Parameter;
                NewV1 = Delta * NewV1 + Epsilon * v1;
                NewV2 = Delta * NewV2 + Epsilon * v2;
                NewV3 = Delta * NewV3 + Epsilon * v3;
                if (!AppMgr.pts[i1].Fixed)
                {
                    AppMgr.pts[i1].Vec = NewV1;
                    err += (v1 - NewV1).magnitude;
                }
                if (!AppMgr.pts[i2].Fixed)
                {
                    AppMgr.pts[i2].Vec = NewV2;
                    err += (v2 - NewV2).magnitude;
                }
                if (!AppMgr.pts[i3].Fixed)
                {
                    AppMgr.pts[i3].Vec = NewV3;
                    err += (v3 - NewV3).magnitude;
                }
                return err;
            }
        }
        return 0f;
    }

    private float ModuleANGLE()
    {
        gameObject.SetActive(Active);
        if (Object1 == null || Object2 == null || Object3 == null)
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
        Point PA = Object1.GetComponent<Point>();
        Point PB = Object2.GetComponent<Point>();
        Point PC = Object3.GetComponent<Point>();
        if (PA == null || PB == null || PC == null)
        {
            Active = false;
            return 0f;
        }
        Parameter = 0.1f;
        float err = 0f;
        if (PA.Fixed && !PB.Fixed && PC.Fixed)
        {
            float Ax = PA.Vec.x, Ay = PA.Vec.y;
            float Bx = PB.Vec.x, By = PB.Vec.y;
            float Cx = PC.Vec.x, Cy = PC.Vec.y;
            float BA = Util.Magnitude(Ax - Bx, Ay - By);
            float BC = Util.Magnitude(Cx - Bx, Cy - By);
            float AC = Util.Magnitude(Ax - Cx, Ay - Cy);
            float Mx = (Ax * BC + Cx * BA) / (BA + BC);
            float My = (Ay * BC + Cy * BA) / (BA + BC);
            float Dx = Bx - Mx, Dy = By - My;
            float ND = Util.Magnitude(Dx, Dy);
            if (ND < 0.00001f) return 0f;
            Dx /= ND;
            Dy /= ND;
            float DeclineBA = Mathf.Atan2(Ay - By, Ax - Bx);
            float DeclineBC = Mathf.Atan2(Cy - By, Cx - Bx);
            float Angle = DeclineBC - DeclineBA;
            float PI = Mathf.PI;
            if (Angle < -2*PI)
                Angle += 2*PI;
            if (Angle < 0)
                Angle += 2*PI;
            if (Angle >= 2*PI)
                Angle -= 2*PI;
            if (PI <= Angle && Angle < 2*PI)
                Angle = 2*PI - Angle;
            float Error = (Angle - Constant) * AC * 0.1f * Parameter;
            Dx *= Error;
            Dy *= Error;
            PB.Vec += new Vector3(Dx, Dy, 0f);
            return Mathf.Abs(Error);
        }
        {
            float Ax = PA.Vec.x, Ay = PA.Vec.y;
            float Bx = PB.Vec.x, By = PB.Vec.y;
            float Cx = PC.Vec.x, Cy = PC.Vec.y;
            float MidABx = Ax * 0.5f + Bx * 0.5f, MidABy = Ay * 0.5f + By * 0.5f;
            float MidCBx = Cx * 0.5f + Bx * 0.5f, MidCBy = Cy * 0.5f + By * 0.5f;
            float DeclineBA = Mathf.Atan2(Ay - By, Ax - Bx);
            float DeclineBC = Mathf.Atan2(Cy - By, Cx - Bx);
            if (DeclineBC < DeclineBA - Mathf.PI) DeclineBC += Mathf.PI * 2f;
            if (DeclineBC > DeclineBA + Mathf.PI) DeclineBC -= Mathf.PI * 2f;
            float Angle = DeclineBC - DeclineBA;
            float AngleError = (Angle - Constant) * Parameter;
            float MaxError = 0.02f;
            if (Angle >= 0)
            {
                if (AngleError > MaxError) AngleError = MaxError;
                if (AngleError < -MaxError) AngleError = -MaxError;
            }
            else 
            {
                AngleError = (Angle + Constant) * Parameter;
                if (AngleError > MaxError) AngleError = MaxError;
                if (AngleError < -MaxError) AngleError = -MaxError;
            }
            if (FixAngle)
                //角を固定するときは点を動かす
            {
                float NewAx = (Ax - MidABx) * Mathf.Cos(AngleError) - (Ay - MidABy) * Mathf.Sin(AngleError) + MidABx;
                float NewAy = (Ax - MidABx) * Mathf.Sin(AngleError) + (Ay - MidABy) * Mathf.Cos(AngleError) + MidABy;
                float NewBx = (Bx - MidABx) * Mathf.Cos(AngleError) - (By - MidABy) * Mathf.Sin(AngleError) + MidABx;
                float NewBy = (Bx - MidABx) * Mathf.Sin(AngleError) + (By - MidABy) * Mathf.Cos(AngleError) + MidABy;
                Vector3 newPAVec = new Vector3(NewAx, NewAy, 0f);
                Vector3 newPBVec = new Vector3(NewBx, NewBy, 0f);
                if (!PA.Fixed)
                {
                    PA.Vec = newPAVec;
                    err += Util.Magnitude(Ax - MidABx, Ay - MidABy) * Mathf.Abs(AngleError);
                }
                if (!PB.Fixed)
                {
                    PB.Vec = newPBVec;
                    err += Util.Magnitude(Bx - MidABx, By - MidABy) * Mathf.Abs(AngleError);
                }
            }
            else　//角を固定しないときは表示を変える
            {
                Constant = Mathf.Abs(Angle);
            }
        }
        {
            float Ax = PA.Vec.x, Ay = PA.Vec.y;
            float Bx = PB.Vec.x, By = PB.Vec.y;
            float Cx = PC.Vec.x, Cy = PC.Vec.y;
            float MidABx = Ax * 0.5f + Bx * 0.5f, MidABy = Ay * 0.5f + By * 0.5f;
            float MidCBx = Cx * 0.5f + Bx * 0.5f, MidCBy = Cy * 0.5f + By * 0.5f;
            float DeclineBA = Mathf.Atan2(Ay - By, Ax - Bx);
            float DeclineBC = Mathf.Atan2(Cy - By, Cx - Bx);
            if (DeclineBC < DeclineBA - Mathf.PI) DeclineBC += Mathf.PI * 2f;
            if (DeclineBC > DeclineBA + Mathf.PI) DeclineBC -= Mathf.PI * 2f;
            float Angle = DeclineBC - DeclineBA;
            float AngleError = (Angle - Constant) * Parameter;
            float MaxError = 0.02f;
            if (Angle >= 0)
            {
                if (AngleError > MaxError) AngleError = MaxError;
                if (AngleError < -MaxError) AngleError = -MaxError;
            }
            else
            {
                AngleError = (Angle + Constant) * 0.1f;
                if (AngleError > MaxError) AngleError = MaxError;
                if (AngleError < -MaxError) AngleError = -MaxError;
            }
            if (FixAngle)
            {
                float NewBx = (Bx - MidCBx) * Mathf.Cos(-AngleError) - (By - MidCBy) * Mathf.Sin(-AngleError) + MidCBx;
                float NewBy = (Bx - MidCBx) * Mathf.Sin(-AngleError) + (By - MidCBy) * Mathf.Cos(-AngleError) + MidCBy;
                float NewCx = (Cx - MidCBx) * Mathf.Cos(-AngleError) - (Cy - MidCBy) * Mathf.Sin(-AngleError) + MidCBx;
                float NewCy = (Cx - MidCBx) * Mathf.Sin(-AngleError) + (Cy - MidCBy) * Mathf.Cos(-AngleError) + MidCBy;
                Vector3 newPBVec = new Vector3(NewBx, NewBy, 0f);
                Vector3 newPCVec = new Vector3(NewCx, NewCy, 0f);
                if (!PB.Fixed)
                {
                    PB.Vec = newPBVec;
                    err += Util.Magnitude(Bx - MidCBx, By - MidCBy) * Mathf.Abs(AngleError);
                }
                if (!PC.Fixed)
                {
                    PC.Vec = newPCVec;
                    err += Util.Magnitude(Cx - MidCBx, Cy - MidCBy) * Mathf.Abs(AngleError);
                }
            }
            else
            {
                Constant = Mathf.Abs(Angle);
            }
        }
        return err;
    }

    private float ModuleBISECTOR()
    {
        gameObject.SetActive(Active);
        if (Object1 == null || Object2 == null)
        {
            GameObject[] OBJs = FindObjectsOfType<GameObject>();
            for (int i = 0; i < OBJs.Length; i++)
            {
                Module MD = OBJs[i].GetComponent<Module>();
                if (MD != null)
                {
                    if (MD.Id == Object1Id)
                    {
                        Object1 = OBJs[i];
                    }
                    if (MD.Id == Object2Id)
                    {
                        Object2 = OBJs[i];
                    }
                }
            }
            if (Object1 == null || Object2 == null) Active = false;
            return 0f;
        }
        Module md1 = Object1.GetComponent<Module>();
        Module md2 = Object2.GetComponent<Module>();
        Point M1PA = md1.Object1.GetComponent<Point>();
        Point M1PB = md1.Object2.GetComponent<Point>();
        Point M1PC = md1.Object3.GetComponent<Point>();
        Point M2PA = md2.Object1.GetComponent<Point>();
        Point M2PB = md2.Object2.GetComponent<Point>();
        Point M2PC = md2.Object3.GetComponent<Point>();
        if (M1PA == null || M1PB == null || M1PC == null || M2PA == null || M2PB == null || M2PC == null)
        {
            Active = false;
            return 0f;
        }
        float M2PAx = M2PA.Vec.x, M2PAy = M2PA.Vec.y;
        float M2PBx = M2PB.Vec.x, M2PBy = M2PB.Vec.y;
        float M2PCx = M2PC.Vec.x, M2PCy = M2PC.Vec.y;
        //float M2MidABx = (M2PAx + M2PBx) * 0.5f, M2MidABy = (M2PAy + M2PBy) * 0.5f;
        //float M2MidCBx = (M2PCx + M2PBx) * 0.5f, M2MidCBy = (M2PCy + M2PBy) * 0.5f;
        float M2DeclineBA = Mathf.Atan2(M2PAy - M2PBy, M2PAx - M2PBx);
        float M2DeclineBC = Mathf.Atan2(M2PCy - M2PBy, M2PCx - M2PBx);
        if (M2DeclineBC < M2DeclineBA - Mathf.PI) M2DeclineBC += Mathf.PI * 2f;
        if (M2DeclineBC > M2DeclineBA + Mathf.PI) M2DeclineBC -= Mathf.PI * 2f;
        float M2Angle = M2DeclineBC - M2DeclineBA;
        float Constant = Mathf.Abs(M2Angle);
        float err = 0f;
        {
            float Ax = M1PA.Vec.x, Ay = M1PA.Vec.y;
            float Bx = M1PB.Vec.x, By = M1PB.Vec.y;
            float Cx = M1PC.Vec.x, Cy = M1PC.Vec.y;
            float MidABx = (Ax + Bx) * 0.5f, MidABy = (Ay + By) * 0.5f;
            float DeclineBA = Mathf.Atan2(Ay - By, Ax - Bx);
            float DeclineBC = Mathf.Atan2(Cy - By, Cx - Bx);
            if (DeclineBC < DeclineBA - Mathf.PI) DeclineBC += Mathf.PI * 2f;
            if (DeclineBC > DeclineBA + Mathf.PI) DeclineBC -= Mathf.PI * 2f;
            float Angle = DeclineBC - DeclineBA;
            float AngleError = (Angle - Constant) * Parameter;
            float MaxError = 0.02f;
            if (Angle >= 0)
            {
                if (AngleError > MaxError) AngleError = MaxError;
                if (AngleError < -MaxError) AngleError = -MaxError;
            }
            else
            {
                AngleError = (Angle + Constant) * Parameter;
                if (AngleError > MaxError) AngleError = MaxError;
                if (AngleError < -MaxError) AngleError = -MaxError;
            }
            float NewAx = (Ax - MidABx) * Mathf.Cos(AngleError) - (Ay - MidABy) * Mathf.Sin(AngleError) + MidABx;
            float NewAy = (Ax - MidABx) * Mathf.Sin(AngleError) + (Ay - MidABy) * Mathf.Cos(AngleError) + MidABy;
            float NewBx = (Bx - MidABx) * Mathf.Cos(AngleError) - (By - MidABy) * Mathf.Sin(AngleError) + MidABx;
            float NewBy = (Bx - MidABx) * Mathf.Sin(AngleError) + (By - MidABy) * Mathf.Cos(AngleError) + MidABy;
            Vector3 newPAVec = new(NewAx, NewAy, 0f);
            Vector3 newPBVec = new(NewBx, NewBy, 0f);
            if (!M1PA.Fixed && !FixAngle)
            {
                M1PA.Vec = newPAVec;
                err += Util.Magnitude(Ax - MidABx, Ay - MidABy) * Mathf.Abs(AngleError);
            }
            if (!M1PB.Fixed && !FixAngle)
            {
                M1PB.Vec = newPBVec;
                err += Util.Magnitude(Bx - MidABx, By - MidABy) * Mathf.Abs(AngleError);
            }
        }
        {
            float Ax = M1PA.Vec.x, Ay = M1PA.Vec.y;
            float Bx = M1PB.Vec.x, By = M1PB.Vec.y;
            float Cx = M1PC.Vec.x, Cy = M1PC.Vec.y;
            float MidCBx = (Cx + Bx) * 0.5f, MidCBy = (Cy + By) * 0.5f;
            float DeclineBA = Mathf.Atan2(Ay - By, Ax - Bx);
            float DeclineBC = Mathf.Atan2(Cy - By, Cx - Bx);
            if (DeclineBC < DeclineBA - Mathf.PI) DeclineBC += Mathf.PI * 2f;
            if (DeclineBC > DeclineBA + Mathf.PI) DeclineBC -= Mathf.PI * 2f;
            float Angle = DeclineBC - DeclineBA;
            float AngleError = (Angle - Constant) * Parameter;
            float MaxError = 0.02f;
            if (Angle >= 0)
            {
                if (AngleError > MaxError) AngleError = MaxError;
                if (AngleError < -MaxError) AngleError = -MaxError;
            }
            else
            {
                AngleError = (Angle + Constant) * Parameter;
                if (AngleError > MaxError) AngleError = MaxError;
                if (AngleError < -MaxError) AngleError = -MaxError;
            }
            float NewBx = (Bx - MidCBx) * Mathf.Cos(-AngleError) - (By - MidCBy) * Mathf.Sin(-AngleError) + MidCBx;
            float NewBy = (Bx - MidCBx) * Mathf.Sin(-AngleError) + (By - MidCBy) * Mathf.Cos(-AngleError) + MidCBy;
            float NewCx = (Cx - MidCBx) * Mathf.Cos(-AngleError) - (Cy - MidCBy) * Mathf.Sin(-AngleError) + MidCBx;
            float NewCy = (Cx - MidCBx) * Mathf.Sin(-AngleError) + (Cy - MidCBy) * Mathf.Cos(-AngleError) + MidCBy;
            Vector3 newPBVec = new(NewBx, NewBy, 0f);
            Vector3 newPCVec = new(NewCx, NewCy, 0f);
            if (!M1PB.Fixed && !FixAngle)
            {
                M1PB.Vec = newPBVec;
                err += Util.Magnitude(Bx - MidCBx, By - MidCBy) * Mathf.Abs(AngleError);
            }
            if (!M1PC.Fixed && !FixAngle)
            {
                M1PC.Vec = newPCVec;
                err += Util.Magnitude(Cx - MidCBx, Cy - MidCBy) * Mathf.Abs(AngleError);
            }
        }
        float M1PAx = M1PA.Vec.x, M1PAy = M1PA.Vec.y;
        float M1PBx = M1PB.Vec.x, M1PBy = M1PB.Vec.y;
        float M1PCx = M1PC.Vec.x, M1PCy = M1PC.Vec.y;
        //float M1MidABx = (M1PAx + M1PBx) * 0.5f, M1MidABy = (M1PAy + M1PBy) * 0.5f;
        //float M1MidCBx = (M1PCx + M1PBx) * 0.5f, M1MidCBy = (M1PCy + M1PBy) * 0.5f;
        float M1DeclineBA = Mathf.Atan2(M1PAy - M1PBy, M1PAx - M1PBx);
        float M1DeclineBC = Mathf.Atan2(M1PCy - M1PBy, M1PCx - M1PBx);
        if (M1DeclineBC < M1DeclineBA - Mathf.PI) M1DeclineBC += Mathf.PI * 2f;
        if (M1DeclineBC > M1DeclineBA + Mathf.PI) M1DeclineBC -= Mathf.PI * 2f;
        float M1Angle = M1DeclineBC - M1DeclineBA;
        Constant = Mathf.Abs(M1Angle);
        {
            float Ax = M2PA.Vec.x, Ay = M2PA.Vec.y;
            float Bx = M2PB.Vec.x, By = M2PB.Vec.y;
            float Cx = M2PC.Vec.x, Cy = M2PC.Vec.y;
            float MidABx = (Ax + Bx) * 0.5f, MidABy = (Ay + By) * 0.5f;
            float DeclineBA = Mathf.Atan2(Ay - By, Ax - Bx);
            float DeclineBC = Mathf.Atan2(Cy - By, Cx - Bx);
            if (DeclineBC < DeclineBA - Mathf.PI) DeclineBC += Mathf.PI * 2f;
            if (DeclineBC > DeclineBA + Mathf.PI) DeclineBC -= Mathf.PI * 2f;
            float Angle = DeclineBC - DeclineBA;
            float AngleError = (Angle - Constant) * Parameter;
            float MaxError = 0.02f;
            if (Angle >= 0)
            {
                if (AngleError > MaxError) AngleError = MaxError;
                if (AngleError < -MaxError) AngleError = -MaxError;
            }
            else
            {
                AngleError = (Angle + Constant) * Parameter;
                if (AngleError > MaxError) AngleError = MaxError;
                if (AngleError < -MaxError) AngleError = -MaxError;
            }
            float NewAx = (Ax - MidABx) * Mathf.Cos(AngleError) - (Ay - MidABy) * Mathf.Sin(AngleError) + MidABx;
            float NewAy = (Ax - MidABx) * Mathf.Sin(AngleError) + (Ay - MidABy) * Mathf.Cos(AngleError) + MidABy;
            float NewBx = (Bx - MidABx) * Mathf.Cos(AngleError) - (By - MidABy) * Mathf.Sin(AngleError) + MidABx;
            float NewBy = (Bx - MidABx) * Mathf.Sin(AngleError) + (By - MidABy) * Mathf.Cos(AngleError) + MidABy;
            Vector3 newPAVec = new Vector3(NewAx, NewAy, 0f);
            Vector3 newPBVec = new Vector3(NewBx, NewBy, 0f);
            if (!M2PA.Fixed && !FixAngle)
            {
                M2PA.Vec = newPAVec;
                err += Util.Magnitude(Ax - MidABx, Ay - MidABy) * Mathf.Abs(AngleError);
            }
            if (!M2PB.Fixed && !FixAngle)
            {
                M2PB.Vec = newPBVec;
                err += Util.Magnitude(Bx - MidABx, By - MidABy) * Mathf.Abs(AngleError);
            }
        }
        {
            float Ax = M2PA.Vec.x, Ay = M2PA.Vec.y;
            float Bx = M2PB.Vec.x, By = M2PB.Vec.y;
            float Cx = M2PC.Vec.x, Cy = M2PC.Vec.y;
            float MidCBx = (Cx + Bx) * 0.5f, MidCBy = (Cy + By) * 0.5f;
            float DeclineBA = Mathf.Atan2(Ay - By, Ax - Bx);
            float DeclineBC = Mathf.Atan2(Cy - By, Cx - Bx);
            if (DeclineBC < DeclineBA - Mathf.PI) DeclineBC += Mathf.PI * 2f;
            if (DeclineBC > DeclineBA + Mathf.PI) DeclineBC -= Mathf.PI * 2f;
            float Angle = DeclineBC - DeclineBA;
            float AngleError = (Angle - Constant) * Parameter;
            float MaxError = 0.02f;
            if (Angle >= 0)
            {
                if (AngleError > MaxError) AngleError = MaxError;
                if (AngleError < -MaxError) AngleError = -MaxError;
            }
            else
            {
                AngleError = (Angle + Constant) * 0.1f;
                if (AngleError > MaxError) AngleError = MaxError;
                if (AngleError < -MaxError) AngleError = -MaxError;
            }
            float NewBx = (Bx - MidCBx) * Mathf.Cos(-AngleError) - (By - MidCBy) * Mathf.Sin(-AngleError) + MidCBx;
            float NewBy = (Bx - MidCBx) * Mathf.Sin(-AngleError) + (By - MidCBy) * Mathf.Cos(-AngleError) + MidCBy;
            float NewCx = (Cx - MidCBx) * Mathf.Cos(-AngleError) - (Cy - MidCBy) * Mathf.Sin(-AngleError) + MidCBx;
            float NewCy = (Cx - MidCBx) * Mathf.Sin(-AngleError) + (Cy - MidCBy) * Mathf.Cos(-AngleError) + MidCBy;
            Vector3 newPBVec = new Vector3(NewBx, NewBy, 0f);
            Vector3 newPCVec = new Vector3(NewCx, NewCy, 0f);
            if (!M2PB.Fixed && !FixAngle)
            {
                M2PB.Vec = newPBVec;
                err += Util.Magnitude(Bx - MidCBx, By - MidCBy) * Mathf.Abs(AngleError);
            }
            if (!M2PC.Fixed && !FixAngle)
            {
                M2PC.Vec = newPCVec;
                err += Util.Magnitude(Cx - MidCBx, Cy - MidCBy) * Mathf.Abs(AngleError);
            }
        }
        return err;
    }

    private float Module_LOCUS()
    {
        gameObject.SetActive(Active);
        if (Object1 == null)
        {
            GameObject[] OBJs = FindObjectsOfType<GameObject>();
            for (int i = 0; i < OBJs.Length; i++)
            {
                Point PT = OBJs[i].GetComponent<Point>();
                if (PT != null && PT.Id == Object1Id)
                {
                    Object1 = OBJs[i];
                }
            }
            if (Object1 == null) Active = false;
        }
        Point pt = Object1.GetComponent<Point>();
        Vector3 ptVec = pt.Vec;
        float difference = (PreVec - ptVec).magnitude;
        //Debug.Log("" + difference + ":" + AppMgr.ConvergencyCount);
        if (difference > 0.05f && AppMgr.ConvergencyCount < 2)
        {
            PreVec = ptVec;
            // LocusDotを追加する。
            GameObject prefab = Resources.Load<GameObject>("Prefabs/LocusDot");
            GameObject obj = Instantiate<GameObject>(prefab, ptVec, Quaternion.identity);
            obj.GetComponent<LocusDot>().parent = this;
        }
        return 0f;
    }


    private float MakeShortSegmentLonger(Point pt1, Point pt2, float Constant)
    {
        Vector3 v12 = pt2.Vec - pt1.Vec;
        float mag12 = v12.magnitude;
        float para = 0.1f;
        if (mag12 < Constant)
        {
            v12.Normalize();
            float difference = (mag12 - Constant) * para;
            if (!pt1.Fixed)
            {
                pt1.Vec += difference * v12;
            }
            if (!pt2.Fixed)
            {
                pt2.Vec -= difference * v12;
            }
            err += difference * 2;
        }
        return 0f;
    }
    private float MakeLongSegmentShorter(Point pt1, Point pt2, float Constant)
    {
        Vector3 v12 = pt2.Vec - pt1.Vec;
        float mag12 = v12.magnitude;
        float para = 0.1f;
        if (mag12 > Constant)
        {
            v12.Normalize();
            float difference = (mag12 - Constant) * para;
            if (!pt1.Fixed)
            {
                pt1.Vec += difference * v12;
            }
            if (!pt2.Fixed)
            {
                pt2.Vec -= difference * v12;
            }
            err += difference * 2;
        }
        return 0f;
    }
    private float MakethreeSegmentsIsometric(Point pt1, Point pt2, Point pt3)
    {
        Vector3 v12 = pt2.Vec - pt1.Vec;
        float mag12 = v12.magnitude;
        Vector3 v23 = pt3.Vec - pt2.Vec;
        float mag23 = v23.magnitude;
        Vector3 v31 = pt1.Vec - pt3.Vec;
        float mag31 = v31.magnitude;
        float mean = (mag12 + mag23 + mag31) / 3f;
        float err = 0f;
        float para = 0.1f;
        {
            v12.Normalize();
            float difference = (mag12 - mean) * para;
            if (!pt1.Fixed)
            {
                pt1.Vec += difference * v12;
            }
            if (!pt2.Fixed)
            {
                pt2.Vec -= difference * v12;
            }
            err += difference * 2;
        }
        v12 = pt2.Vec - pt1.Vec;
        mag12 = v12.magnitude;
        v23 = pt3.Vec - pt2.Vec;
        mag23 = v23.magnitude;
        v31 = pt1.Vec - pt3.Vec;
        mag31 = v31.magnitude;
        mean = (mag12 + mag23 + mag31) / 3f;
        {
            v23.Normalize();
            float difference = (mag23 - mean) * para;
            if (!pt2.Fixed)
            {
                pt2.Vec += difference * v23;
            }
            if (!pt3.Fixed)
            {
                pt3.Vec -= difference * v23;
            }
            err += difference * 2;
        }
        v12 = pt2.Vec - pt1.Vec;
        mag12 = v12.magnitude;
        v23 = pt3.Vec - pt2.Vec;
        mag23 = v23.magnitude;
        v31 = pt1.Vec - pt3.Vec;
        mag31 = v31.magnitude;
        mean = (mag12 + mag23 + mag31) / 3f;
        {
            v31.Normalize();
            float difference = (mag31 - mean) * para;
            if (!pt3.Fixed)
            {
                pt3.Vec += difference * v31;
            }
            if (!pt1.Fixed)
            {
                pt1.Vec -= difference * v31;
            }
            err += difference * 2;
        }
        return err;
    }
    private float MakeNarrowAngleWider(Point pt1, Point pt2, Point pt3, float Limiter)
    {
        float Ax = pt1.Vec.x, Ay = pt1.Vec.y;
        float Bx = pt2.Vec.x, By = pt2.Vec.y;
        float Cx = pt3.Vec.x, Cy = pt3.Vec.y;
        float DeclineBA = Mathf.Atan2(Ay - By, Ax - Bx);
        float DeclineBC = Mathf.Atan2(Cy - By, Cx - Bx);
        float Angle = DeclineBC - DeclineBA;
        float PI = Mathf.PI;
        if (Angle <= -PI)
            Angle += 2 * PI;
        if (Angle >= PI)
            Angle -= 2 * PI;
        Angle = Mathf.Abs(Angle);
        if (Angle < Limiter)
        {
            float BA = Util.Magnitude(Ax - Bx, Ay - By);
            float BC = Util.Magnitude(Cx - Bx, Cy - By);
            float AC = Util.Magnitude(Ax - Cx, Ay - Cy);
            float Mx = (Ax * BC + Cx * BA) / (BA + BC);
            float My = (Ay * BC + Cy * BA) / (BA + BC);
            float Dx = Bx - Mx, Dy = By - My;
            float ND = Util.Magnitude(Dx, Dy);
            if (ND < 0.00001f) return 0f;
            Dx /= ND;
            Dy /= ND;
            float Error = (Angle - Limiter) * AC * 0.1f * Parameter;
            Dx *= Error;
            Dy *= Error;
            if (pt2.Fixed == false)
            {
                pt2.Vec += new Vector3(Dx, Dy, 0f);
                return Mathf.Abs(Error);
            }
        }
        return 0f;
    }
    private float MakeWideAngleNarrower(Point pt1, Point pt2, Point pt3, float Limiter)
    {
        float Ax = pt1.Vec.x, Ay = pt1.Vec.y;
        float Bx = pt2.Vec.x, By = pt2.Vec.y;
        float Cx = pt3.Vec.x, Cy = pt3.Vec.y;
        float DeclineBA = Mathf.Atan2(Ay - By, Ax - Bx);
        float DeclineBC = Mathf.Atan2(Cy - By, Cx - Bx);
        float Angle = DeclineBC - DeclineBA;
        float PI = Mathf.PI;
        if (Angle <= -PI)
            Angle += 2 * PI;
        if (Angle >= PI)
            Angle -= 2 * PI;
        Angle = Mathf.Abs(Angle);
        if (Angle > Limiter)
        {
            float BA = Util.Magnitude(Ax - Bx, Ay - By);
            float BC = Util.Magnitude(Cx - Bx, Cy - By);
            float AC = Util.Magnitude(Ax - Cx, Ay - Cy);
            float Mx = (Ax * BC + Cx * BA) / (BA + BC);
            float My = (Ay * BC + Cy * BA) / (BA + BC);
            float Dx = Bx - Mx, Dy = By - My;
            float ND = Util.Magnitude(Dx, Dy);
            if (ND < 0.00001f) return 0f;
            Dx /= ND;
            Dy /= ND;
            float Error = (Angle - Limiter) * AC * 0.1f * Parameter;
            Dx *= Error;
            Dy *= Error;
            if (pt2.Fixed == false)
            {
                pt2.Vec += new Vector3(Dx, Dy, 0f);
                return Mathf.Abs(Error);
            }
        }
        return 0f;
    }
    private float Module_Triangle()
    {
        gameObject.SetActive(Active);
        if (Object1 == null || Object2 == null || Object3 == null)
        {
            GameObject[] OBJs = FindObjectsOfType<GameObject>();
            for (int i = 0; i < OBJs.Length; i++)
            {
                Point PT = OBJs[i].GetComponent<Point>();
                if (PT != null && PT.Id == Object1Id)
                    Object1 = OBJs[i];
                if (PT != null && PT.Id == Object2Id)
                    Object2 = OBJs[i];
                if (PT != null && PT.Id == Object3Id)
                    Object3 = OBJs[i];
            }
            if (Object1 == null || Object2 == null || Object3 == null) 
                Active = false;
        }
        if (Active) {
            Point pt1 = Object1.GetComponent<Point>();
            Point pt2 = Object2.GetComponent<Point>();
            Point pt3 = Object3.GetComponent<Point>();
            float err = 0f;
            if (PolygonOption == 0)
            {
                err += MakethreeSegmentsIsometric(pt1, pt2, pt3);
            }
            else if (PolygonOption == 1)
            {
                err += MakeWideAngleNarrower(pt2, pt1, pt3, 75f * Mathf.PI / 180f);
                err += MakeWideAngleNarrower(pt3, pt2, pt1, 75f * Mathf.PI / 180f);
                err += MakeWideAngleNarrower(pt1, pt3, pt2, 75f * Mathf.PI / 180f);
            }
            else if (PolygonOption == 2)
            {
                err += MakeNarrowAngleWider(pt2, pt1, pt3, 110f * Mathf.PI / 180f);

            }
            // if angle BAC < 25 degree then the module makes this angle wider.
            err += MakeNarrowAngleWider(pt2, pt1, pt3, 25f * Mathf.PI / 180f);
            err += MakeNarrowAngleWider(pt3, pt2, pt1, 25f * Mathf.PI / 180f);
            err += MakeNarrowAngleWider(pt1, pt3, pt2, 25f * Mathf.PI / 180f);
            //
            err += MakeShortSegmentLonger(pt1, pt2, 0.5f);
            err += MakeShortSegmentLonger(pt2, pt3, 0.5f);
            err += MakeShortSegmentLonger(pt3, pt1, 0.5f);
            //
            err += MakeLongSegmentShorter(pt1, pt2, 10f);
            err += MakeLongSegmentShorter(pt2, pt3, 10f);
            err += MakeLongSegmentShorter(pt3, pt1, 10f);
            return err;
        }
        return 0f;
    }

    private float MaketwoSegmentsIsometric(Point pt1, Point pt2, Point pt3, Point pt4)
    {
        Vector3 v12 = pt2.Vec - pt1.Vec;
        float mag12 = v12.magnitude;
        Vector3 v34 = pt4.Vec - pt3.Vec;
        float mag34 = v34.magnitude;
        float mean = (mag12 + mag34) / 2f;
        float err = 0f;
        float para = 0.1f;
        {
            v12.Normalize();
            float difference = (mag12 - mean) * para;
            if (!pt1.Fixed)
            {
                pt1.Vec += difference * v12;
            }
            if (!pt2.Fixed)
            {
                pt2.Vec -= difference * v12;
            }
            err += difference * 2;
        }
        v12 = pt2.Vec - pt1.Vec;
        mag12 = v12.magnitude;
        v34 = pt4.Vec - pt3.Vec;
        mag34 = v34.magnitude;
        mean = (mag12 + mag34) / 2f;
        {
            v34.Normalize();
            float difference = (mag34 - mean) * para;
            if (!pt3.Fixed)
            {
                pt3.Vec += difference * v34;
            }
            if (!pt4.Fixed)
            {
                pt4.Vec -= difference * v34;
            }
            err += difference * 2;
        }
        return err;
    }

    private float Module_Quadrilateral()
    {
        gameObject.SetActive(Active);
        if (Object1 == null || Object2 == null || Object3 == null || Object4 == null)
        {
            GameObject[] OBJs = FindObjectsOfType<GameObject>();
            for (int i = 0; i < OBJs.Length; i++)
            {
                Point PT = OBJs[i].GetComponent<Point>();
                if (PT != null && PT.Id == Object1Id)
                    Object1 = OBJs[i];
                if (PT != null && PT.Id == Object2Id)
                    Object2 = OBJs[i];
                if (PT != null && PT.Id == Object3Id)
                    Object3 = OBJs[i];
                if (PT != null && PT.Id == Object4Id)
                    Object4 = OBJs[i];
            }
            if (Object1 == null || Object2 == null || Object3 == null || Object4 == null)
                Active = false;
        }
        if (Active)
        {
            Point pt1 = Object1.GetComponent<Point>();
            Point pt2 = Object2.GetComponent<Point>();
            Point pt3 = Object3.GetComponent<Point>();
            Point pt4 = Object4.GetComponent<Point>();
            float err = 0f;
            if (PolygonOption == 0)
            {
                err += MaketwoSegmentsIsometric(pt1, pt2, pt4, pt3);
                err += MaketwoSegmentsIsometric(pt1, pt4, pt2, pt3);
                err += MaketwoSegmentsIsometric(pt1, pt2, pt1, pt4);
                err += MaketwoSegmentsIsometric(pt4, pt3, pt2, pt3);
                err += MaketwoSegmentsIsometric(pt1, pt3, pt2, pt4);
            }
            else if (PolygonOption == 1)
            {
                err += MaketwoSegmentsIsometric(pt1, pt2, pt4, pt3);
                err += MaketwoSegmentsIsometric(pt1, pt4, pt2, pt3);
                err += MaketwoSegmentsIsometric(pt1, pt3, pt2, pt4);
            }
            else if (PolygonOption == 2)
            {
                err += MaketwoSegmentsIsometric(pt1, pt2, pt4, pt3);
                err += MaketwoSegmentsIsometric(pt1, pt4, pt2, pt3);
                err += MaketwoSegmentsIsometric(pt1, pt2, pt1, pt4);
                err += MaketwoSegmentsIsometric(pt4, pt3, pt2, pt3);
            }
            else if (PolygonOption == 3)
            {
                err += MaketwoSegmentsIsometric(pt1, pt2, pt4, pt3);
                err += MaketwoSegmentsIsometric(pt1, pt4, pt2, pt3);
            }
            return err;
        }
        return 0f;
    }

    public float ExecuteModule()
    {
        if (!Active) return 0f;
        switch (Type) {
            case MENU.POINT_ON_POINT: 
                return ModulePOINT_ON_POINT();
            case MENU.POINT_ON_LINE:
                return ModulePOINT_ON_LINE();
            case MENU.POINT_ON_CIRCLE:
                return ModulePOINT_ON_CIRCLE();
            case MENU.LINES_ISOMETRY:
                return ModuleLINES_ISOMETRY();
            case MENU.LINES_PERPENDICULAR:
                return ModuleLINES_PERPENDICULAR();
            case MENU.LINES_PARALLEL:
                return ModuleLINES_PARALLEL();
            case MENU.LINE_HORIZONTAL:
                return ModuleLINE_HORIZONTAL();
            case MENU.CIRCLE_TANGENT_LINE:
                return ModuleCIRCLE_TANGENT_LINE();
            case MENU.CIRCLE_TANGENT_CIRCLE:
                return ModuleCIRCLE_TANGENT_CIRCLE();
            case MENU.ADD_MIDPOINT:
                return ModuleADD_MIDPOINT();
            case MENU.ANGLE:
                return ModuleANGLE();
            case MENU.BISECTOR:
                return ModuleBISECTOR();
            case MENU.ADD_LOCUS:
                return Module_LOCUS();
            case MENU.TRIANGLE:
                return Module_Triangle();
            case MENU.QUADRILATERAL:
                return Module_Quadrilateral();
        }
        return 0f;
    }

    void Update()
    {
        //if (AppMgr.ModuleOn) { // いつでもモジュールを切れるようにしておく。
        //    for (int a = 0; a < 100; a++)
        //    {
        //        ExecuteModule();
        //    }
        //}
        if (GameLog != null)
        {
            GameLog.GetComponent<Log>().Ratio1 = Ratio1;
            GameLog.GetComponent<Log>().Ratio2 = Ratio2;
            GameLog.GetComponent<Log>().Constant = Constant;
        }
    }
}
