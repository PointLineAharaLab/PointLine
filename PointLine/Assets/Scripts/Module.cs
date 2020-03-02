using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module : MonoBehaviour {

    public int Type=0;
    public int Object1Id = -1;
    public int Object2Id = -1;
    public int Object3Id = -1;
    public GameObject Object1, Object2, Object3;
    public int Id=-1;
    public float Ratio1 = 1f;
    public float Ratio2 = 1f;
    public float Constant = Mathf.PI/2f;

    public GameObject parent = null;
    public GameObject GameLog = null;
    public string ModuleName = "";
    public bool Active = true;

    // Use this for initialization
    void Start () {
        Active = true;
    }

    private void ModulePOINT_ON_POINT()
    {        //二つの頂点を1つにする
        if(Object1 != null && Object2 != null)
        {
            Point p1 = Object1.GetComponent<Point>();
            Point p2 = Object2.GetComponent<Point>();
            if (p1 == null || p2 == null)
            {
                Active = false;
                return;
            }
            Vector3 v1 = p1.Vec;
            Vector3 v2 = p2.Vec;
            Vector3 v1New = 0.7f * v1 + 0.3f * v2;
            Vector3 v2New = 0.3f * v1 + 0.7f * v2;
            // debug
            float err = (p1.Vec - v1New).magnitude + (p2.Vec - v2New).magnitude;
            if (err > AppMgr.ConvergencyError) AppMgr.ConvergencyCount++;
            // debug
            if (!p1.Fixed)
            {
                p1.Vec = v1New;
            }
            if (!p2.Fixed)
            {
                p2.Vec = v2New;
            }
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
        }
    }

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

    private void ModulePOINT_ON_LINE()
    {
        // 点を直線の上に載せる。
        if (parent != null) parent.SetActive(Active);
        if(Object1 != null && Object2 != null)
        {
            Point p1 = Object1.GetComponent<Point>();
            Line l2 = Object2.GetComponent<Line>();
            if(p1 == null || l2 == null)
            {
                Active = false;
                return;
            }
            Point p21 = l2.Point1.GetComponent<Point>();
            Point p22 = l2.Point2.GetComponent<Point>();
            if(p21 == null || p22 == null)
            {
                Active = false;
                return;
            }
            float Distance;
            Vector3 DVec, PVec;
            DVec = Rotate90(p22.Vec - p21.Vec);
            DVec.Normalize();
            PVec = p1.Vec - p21.Vec;
            Distance = Vector3.Dot(DVec, PVec);
            // debug
            float err = Mathf.Abs(Distance) * 0.25f;
            if (err > AppMgr.ConvergencyError) AppMgr.ConvergencyCount++;
            // debug
            if (!p1.Fixed)
            {// 点を直線に近づける
                p1.Vec -= (Distance * 0.25f) * DVec;
            }
            if (!p21.Fixed)
            { //直線を点に近づける
                p21.Vec += (Distance * 0.25f) * DVec;
            }
            if (!p22.Fixed)
            { //直線を点に近づける
                p22.Vec += (Distance * 0.25f) * DVec;
            }
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
        }
    }

    private void ModulePOINT_ON_CIRCLE()
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
                return;
            }
            float dist = Vector3.Distance(p21.Vec, p1.Vec);
            float rad = c2.Radius;
            float delta = (dist - rad) * 0.25f;
            float radNew = rad + delta;
            c2.Radius = radNew;
            Vector3 v2 = p1.Vec - p21.Vec;
            v2.Normalize();
            // debug
            float err = Mathf.Abs(delta);
            if (err > AppMgr.ConvergencyError) AppMgr.ConvergencyCount++;
            // debug
            if (!p1.Fixed)
            {
                p1.Vec = p1.Vec - delta * v2;
            }
            if (!p21.Fixed)
            { 
                p21.Vec = p21.Vec + delta * v2;
            }
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
        }
    }


    private void ModuleLINES_ISOMETRY()
    {
        gameObject.SetActive(Active);
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
            if(p11 == null|| p12 == null|| p21 == null||p22 == null)
            {
                Active = false;
                return;
            }
            Vector3 VecA, VecB;
            float NormA, NormB, Delta;

            VecA = p12.Vec - p11.Vec;
            VecB = p22.Vec - p21.Vec;
            NormA = VecA.magnitude;
            NormB = VecB.magnitude;
            VecA.Normalize();
            VecB.Normalize();
            Delta = (NormB - NormA) * 0.125f;
            // debug
            float err = Mathf.Abs(Delta);
            if (err > AppMgr.ConvergencyError) AppMgr.ConvergencyCount++;
            // debug
            //  線分の長さを等しくする
            if (!p11.Fixed)
                p11.Vec -= Delta * VecA;
            if (!p21.Fixed)
                p21.Vec += Delta * VecB;
            if (!p12.Fixed)
                p12.Vec += Delta * VecA;
            if (!p22.Fixed)
                p22.Vec -= Delta * VecB;
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

    private void ModuleLINES_PERPENDICULAR()
    {
        gameObject.SetActive(Active);
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
            Delta *= 0.05f;
            float CosDelta = Mathf.Cos(Delta);
            float SinDelta = Mathf.Sin(Delta);
            float x1c = (x11 + x12) * 0.5f;
            float y1c = (y11 + y12) * 0.5f;
            float x2c = (x21 + x22) * 0.5f;
            float y2c = (y21 + y22) * 0.5f;
            // debug
            float err = Mathf.Abs(SinDelta);
            if (err > AppMgr.ConvergencyError) AppMgr.ConvergencyCount++;
            // debug
            Vector3 NewVec = Vector3.zero;
            NewVec.x = (x11 - x1c) * CosDelta - (y11 - y1c) * SinDelta + x1c;
            NewVec.y = +(x11 - x1c) * SinDelta + (y11 - y1c) * CosDelta + y1c;
            if (!p11.Fixed)
                p11.Vec = NewVec;
            NewVec.x = (x12 - x1c) * CosDelta - (y12 - y1c) * SinDelta + x1c;
            NewVec.y = +(x12 - x1c) * SinDelta + (y12 - y1c) * CosDelta + y1c;
            if (!p12.Fixed)
                p12.Vec = NewVec;
            NewVec.x = (x21 - x2c) * CosDelta + (y21 - y2c) * SinDelta + x2c;
            NewVec.y = -(x21 - x2c) * SinDelta + (y21 - y2c) * CosDelta + y2c;
            if (!p21.Fixed)
                p21.Vec = NewVec;
            NewVec.x = (x22 - x2c) * CosDelta + (y22 - y2c) * SinDelta + x2c;
            NewVec.y = -(x22 - x2c) * SinDelta + (y22 - y2c) * CosDelta + y2c;
            if (!p22.Fixed)
                p22.Vec = NewVec;
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

    private void ModuleLINES_PARALLEL()
    {//直線を平行にする
        gameObject.SetActive(Active);
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
            Delta *= 0.125f;
            float CosDelta = Mathf.Cos(Delta);
            float SinDelta = Mathf.Sin(Delta);
            float x1c = (x11 + x12) * 0.5f;
            float y1c = (y11 + y12) * 0.5f;
            float x2c = (x21 + x22) * 0.5f;
            float y2c = (y21 + y22) * 0.5f;
            // debug
            float err = Mathf.Abs(SinDelta);
            if (err > AppMgr.ConvergencyError) AppMgr.ConvergencyCount++;
            // debug
            Vector3 NewVec = Vector3.zero;
            NewVec.x = (x11 - x1c) * CosDelta - (y11 - y1c) * SinDelta + x1c;
            NewVec.y = +(x11 - x1c) * SinDelta + (y11 - y1c) * CosDelta + y1c;
            if (!p11.Fixed)
                p11.Vec = NewVec;
            NewVec.x = (x12 - x1c) * CosDelta - (y12 - y1c) * SinDelta + x1c;
            NewVec.y = +(x12 - x1c) * SinDelta + (y12 - y1c) * CosDelta + y1c;
            if (!p12.Fixed)
                p12.Vec = NewVec;
            NewVec.x = (x21 - x2c) * CosDelta + (y21 - y2c) * SinDelta + x2c;
            NewVec.y = -(x21 - x2c) * SinDelta + (y21 - y2c) * CosDelta + y2c;
            if (!p21.Fixed)
                p21.Vec = NewVec;
            NewVec.x = (x22 - x2c) * CosDelta + (y22 - y2c) * SinDelta + x2c;
            NewVec.y = -(x22 - x2c) * SinDelta + (y22 - y2c) * CosDelta + y2c;
            if (!p22.Fixed)
                p22.Vec = NewVec;
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
        return;
    }

    private void ModuleCIRCLE_TANGENT_LINE()
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
                return;
            }
            Point p11 = CI1.CenterPoint.GetComponent<Point>();
            Point p21 = LN2.Point1.GetComponent<Point>();
            Point p22 = LN2.Point2.GetComponent<Point>();
            if (p11 == null || p21 == null || p22 == null)
            {
                Active = false;
                return;
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
            float Delta = (Distance - Radius) * 0.16f;
            // debug
            float err = Mathf.Abs(Delta);
            if (err > AppMgr.ConvergencyError) AppMgr.ConvergencyCount++;
            // debug
            // 円の中心を直線に近づける
            if (!p11.Fixed)
                p11.Vec -= Delta * DVec;
            //円の半径を適切に変化させる
            CI1.Radius += Delta;
            // 直線を円に近づける
            if (!p21.Fixed)
                p21.Vec += Delta * DVec;
            if (!p22.Fixed)
                p22.Vec += Delta * DVec;
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
        return;
    }

    private void ModuleCIRCLE_TANGENT_CIRCLE()
    {//円を円に（外）接させる
        //Object1Id : 円１
        //Object2Id : 円２
        // 円と中心を探す
        gameObject.SetActive(Active);
        if (Object1 != null && Object2 != null)
        {
            Circle CI1 = Object1.GetComponent<Circle>();
            Circle CI2 = Object2.GetComponent<Circle>();
            if (CI1 == null || CI2 == null)
            {
                Active = false;
                return;
            }
            Point p11 = CI1.CenterPoint.GetComponent<Point>();
            Point p21 = CI2.CenterPoint.GetComponent<Point>();
            if (p11 == null || p21 == null)
            {
                Active = false;
                return;
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
            if (Mathf.Abs(DeltaIn) > Mathf.Abs(DeltaOut))
            {// 外接していると思われる
                Delta = DeltaOut * 0.125f;
                // debug
                float err = Mathf.Abs(Delta);
                if (err > AppMgr.ConvergencyError) AppMgr.ConvergencyCount++;
                // debug
                //円１の中心を動かす
                if (!p11.Fixed)
                    p11.Vec += Delta * DVec12;
                //円２の中心を動かす
                if (!p21.Fixed)
                    p21.Vec += Delta * DVec21;
                //円１の半径を調整する
                CI1.Radius += Delta;
                //円２の半径を調整する
                CI2.Radius += Delta;
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
                Delta = DeltaIn * 0.125f;
                // debug
                float err = Mathf.Abs(Delta);
                if (err > AppMgr.ConvergencyError) AppMgr.ConvergencyCount++;
                // debug
                //円１の中心を動かす
                if (!p11.Fixed)
                    p11.Vec += Delta * DVec12;
                //円２の中心を動かす
                if (!p21.Fixed)
                    p21.Vec += Delta * DVec21;
                //円１の半径を調整する
                if (C1out)
                {
                    CI1.Radius += Delta;
                }
                else
                {
                    CI1.Radius -= Delta;
                }
                //円２の半径を調整する
                if (C1out)
                {
                    CI2.Radius -= Delta;
                }
                else
                {
                    CI2.Radius += Delta;
                }

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
        return;
    }

    private void ModuleADD_MIDPOINT()
    {
        int i1 = -1, i2 = -1, i3 = -1;
        if (AppMgr.pts == null) return;
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
            if (Ratio1 == Ratio2 || Ratio1 == -Ratio2) { 
                Vector3 v1 = AppMgr.pts[i1].Vec;
                Vector3 v2 = AppMgr.pts[i2].Vec;
                Vector3 v3 = AppMgr.pts[i3].Vec;
                Vector3 NewV1 = - v2 + 2.0f * v3;
                Vector3 NewV2 = - v1 + 2.0f * v3;
                Vector3 NewV3 = 0.5f * v1 + 0.5f * v2;
                float Delta = 0.25f;
                float Epsilon = 1.0f - Delta;
                NewV1 = Delta * NewV1 + Epsilon * v1;
                NewV2 = Delta * NewV2 + Epsilon * v2;
                NewV3 = Delta * NewV3 + Epsilon * v3;
                // debug
                float err = (v1-NewV1).magnitude + (v2 - NewV2).magnitude + (v3 - NewV3).magnitude;
                if (err > AppMgr.ConvergencyError) AppMgr.ConvergencyCount++;
                // debug
                if (!AppMgr.pts[i1].Fixed)
                    AppMgr.pts[i1].Vec = NewV1;
                if (!AppMgr.pts[i2].Fixed)
                    AppMgr.pts[i2].Vec = NewV2;
                if (!AppMgr.pts[i3].Fixed)
                    AppMgr.pts[i3].Vec = NewV3;
            }
            else
            {
                Vector3 v1 = AppMgr.pts[i1].Vec;
                Vector3 v2 = AppMgr.pts[i2].Vec;
                Vector3 v3 = AppMgr.pts[i3].Vec;
                Vector3 NewV1 = (Ratio2==0)? v1 : (-Ratio1 * v2 + (Ratio2 + Ratio1) * v3) / Ratio2;
                Vector3 NewV2 = (Ratio1==0)? v2 : (-Ratio2 * v1 + (Ratio2 + Ratio1) * v3) / Ratio1;
                Vector3 NewV3 = (Ratio2 * v1 + Ratio1 * v2) / (Ratio2 + Ratio1);
                float Delta = 0.25f;
                float Epsilon = 1.0f - Delta;
                NewV1 = Delta * NewV1 + Epsilon * v1;
                NewV2 = Delta * NewV2 + Epsilon * v2;
                NewV3 = Delta * NewV3 + Epsilon * v3;
                // debug
                float err = (v1 - NewV1).magnitude + (v2 - NewV2).magnitude + (v3 - NewV3).magnitude;
                if (err > AppMgr.ConvergencyError) AppMgr.ConvergencyCount++;
                // debug
                if (!AppMgr.pts[i1].Fixed)
                    AppMgr.pts[i1].Vec = NewV1;
                if (!AppMgr.pts[i2].Fixed)
                    AppMgr.pts[i2].Vec = NewV2;
                if (!AppMgr.pts[i3].Fixed)
                    AppMgr.pts[i3].Vec = NewV3;
            }
        }

    }

    private void ModuleANGLE()
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
            return;
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
            float AngleError = (Angle - Constant) * 0.1f;
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
            // debug
            float err = Mathf.Abs(AngleError);
            if (err > AppMgr.ConvergencyError) AppMgr.ConvergencyCount++;
            // debug
            float NewAx = (Ax - MidABx) * Mathf.Cos(AngleError) - (Ay - MidABy) * Mathf.Sin(AngleError) + MidABx;
            float NewAy = (Ax - MidABx) * Mathf.Sin(AngleError) + (Ay - MidABy) * Mathf.Cos(AngleError) + MidABy;
            float NewBx = (Bx - MidABx) * Mathf.Cos(AngleError) - (By - MidABy) * Mathf.Sin(AngleError) + MidABx;
            float NewBy = (Bx - MidABx) * Mathf.Sin(AngleError) + (By - MidABy) * Mathf.Cos(AngleError) + MidABy;
            Vector3 newPAVec = new Vector3(NewAx, NewAy, 0f);
            Vector3 newPBVec = new Vector3(NewBx, NewBy, 0f);
            if (!PA.Fixed)
                PA.Vec = newPAVec;
            if (!PB.Fixed)
                PB.Vec = newPBVec;
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
            float AngleError = (Angle - Constant) * 0.1f;
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
            // debug
            float err = Mathf.Abs(AngleError);
            if (err > AppMgr.ConvergencyError) AppMgr.ConvergencyCount++;
            // debug
            float NewCx = (Cx - MidCBx) * Mathf.Cos(-AngleError) - (Cy - MidCBy) * Mathf.Sin(-AngleError) + MidCBx;
            float NewCy = (Cx - MidCBx) * Mathf.Sin(-AngleError) + (Cy - MidCBy) * Mathf.Cos(-AngleError) + MidCBy;
            float NewBx = (Bx - MidCBx) * Mathf.Cos(-AngleError) - (By - MidCBy) * Mathf.Sin(-AngleError) + MidCBx;
            float NewBy = (Bx - MidCBx) * Mathf.Sin(-AngleError) + (By - MidCBy) * Mathf.Cos(-AngleError) + MidCBy;
            Vector3 newPBVec = new Vector3(NewBx, NewBy, 0f);
            Vector3 newPCVec = new Vector3(NewCx, NewCy, 0f);
            if (!PB.Fixed)
                PB.Vec = newPBVec;
            if (!PC.Fixed)
                PC.Vec = newPCVec;
        }
    }

    public void ExecuteModule()
    {
        if (!Active) return;
        switch (Type) {
            case MENU.POINT_ON_POINT: 
                ModulePOINT_ON_POINT();
                break;
            case MENU.POINT_ON_LINE:
                ModulePOINT_ON_LINE();
                break;
            case MENU.POINT_ON_CIRCLE:
                ModulePOINT_ON_CIRCLE();
                break;
            case MENU.LINES_ISOMETRY:
                ModuleLINES_ISOMETRY();
                break;
            case MENU.LINES_PERPENDICULAR:
                ModuleLINES_PERPENDICULAR();
                break;
            case MENU.LINES_PARALLEL:
                ModuleLINES_PARALLEL();
                break;
            case MENU.CIRCLE_TANGENT_LINE:
                ModuleCIRCLE_TANGENT_LINE();
                break;
            case MENU.CIRCLE_TANGENT_CIRCLE:
                ModuleCIRCLE_TANGENT_CIRCLE();
                break;
            case MENU.ADD_MIDPOINT:
                ModuleADD_MIDPOINT();
                break;
            case MENU.ANGLE:
                ModuleANGLE();
                break;
        }
    }

    void Update()
    {
        if (AppMgr.ModuleOn) { // いつでもモジュールを切れるようにしておく。
            for (int a = 0; a < 100; a++)
            {
                ExecuteModule();
            }
        }
        if (GameLog != null)
        {
            GameLog.GetComponent<Log>().Ratio1 = Ratio1;
            GameLog.GetComponent<Log>().Ratio2 = Ratio2;
            GameLog.GetComponent<Log>().Constant = Constant;
        }
    }
}
