﻿
using Newtonsoft.Json.Linq;
using SimpleFileBrowser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.UIElements;


//using cv2;
//using OpenCvSharp.Demo;
//using OpenCvSharp;
//using OpenCvSharp.Aruco;



/// <summary>
/// アプリケーションマネージャ
/// </summary>
public class AppMgr : MonoBehaviour {
    /// <summary>
    /// Language, 1:Japanese, 0:English
    /// </summary>
    public static int Japanese = 1;
    /// 

    public static int Mode = 0;
    public static int ModeStep = 0;

    public static bool DrawOn = true;
    public static bool MenuOn = false;
    public static bool SelectorOn = false;
    public static bool KeyOn = true;
    public static bool FileDialogOn = false;
    public static bool AccessWebOn = false;
    public static bool AccessWebEnd = false;
    public static bool WorkListOn = false;

    public static Point[] pts = null;
    public static Line[] lns = null;
    public static Circle[] cis = null;
    public static Module[] mds = null;
    public static AngleMark[] ams = null;
    //public static WorkListColumn[] wlcs = null;
    public static string TmpFilename = null;
    public static int WorkListStart = 0;

    public static Vector3 LeftBottom, RightUp;
    public static byte[] ImageBytes=null;
    public GameObject LogFolder;

    public static int ConvergencyCount=0;
    public GameObject ConvergencyAlertText;
    public static GameObject ConvergencyAlert;
    public static float ConvergencyError = 0.0001f;
    public static float ConvergencyThreshold = 0.000001f;
    public static bool ModuleOn = true;
    
    public static int ClickRequire = 0;// 1: point, 2: line, 3:circle, 4:angle
    public static int CLICKREQ_NULL = 0;
    public static int CLICKREQ_POINT = 1;
    public static int CLICKREQ_LINE = 2;
    public static int CLICKREQ_CIRCLE = 3;
    public static int CLICKREQ_ANGLE = 4;
    public static int CLICKREQ_POINT_NULL = 5;
    public static int CLICKREQ_LINE_CIRCLE = 6;

    // Use this for initialization
    void Start () {
        pts = null;
        lns = null;
        cis = null;
        mds = null;
        if(TmpFilename != null)
            Util.OpenLog(TmpFilename);
        LeftBottom = Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, 0f));
        RightUp = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
        // set LogFolder.Tranform
        AppMgr.ConvergencyAlert = ConvergencyAlertText;

    }


    public static int GetMode()
    {
        return Mode;
    }

	// Update is called once per frame
	void Update () {
        //ExecuteAllModules();
        Rescaling();
    }


    private static Rect _guiRect = new Rect();
    static Rect GetGUIRect()
    {
        return _guiRect;
    }
    private static GUIStyle _guiStyle = null;
    static GUIStyle GetGUIStyle()
    {
        return _guiStyle ?? (_guiStyle = new GUIStyle());
    }
    /// フォントサイズを設定.
    public static void SetFontSize(int size)
    {
        GetGUIStyle().fontSize = size;
    }
    /// フォントカラーを設定.
    public static void SetFontColor(Color color)
    {
        GetGUIStyle().normal.textColor = color;
    }

    /// フォント位置設定
    public static void SetFontAlignment(TextAnchor align)
    {
        GetGUIStyle().alignment = align;
    }

    public static bool ClickOnButton(Vector3 v)//おそらく不要
    {
        //float WorldHeight = 5f;
        float ButtonSizeX = 210f; //(WorldHeight / Screen.height * Screen.width);
        float ButtonSizeY = 150f; //(WorldHeight / Screen.height * Screen.width);

        if (0 <= v.x && v.x <= ButtonSizeX && Screen.height- ButtonSizeY <= v.y && v.y <= Screen.height)
        {
           //Debug.Log("ClickOnButton  = true");
            return true;
        }
        return false;
    }


    public static void Rescaling()
    {
        if (pts == null) return;
        if (Util.FixDisplay) return;
        float MaxX = -9999f, MaxY = -9999f;
        float MinX = 9999f, MinY = 9999f;

        for (int i = 0; i < pts.Length; ++i)
        {
            Vector3 v = pts[i].Vec;
            if (MaxX < v.x || i == 0) MaxX = v.x;
            if (MinX > v.x || i == 0) MinX = v.x;
            if (MaxY < v.y || i == 0) MaxY = v.y;
            if (MinY > v.y || i == 0) MinY = v.y;
        }

        float rate = 1f;

        if (LeftBottom.x > MinX && rate > LeftBottom.x / MinX)
            rate = LeftBottom.x / MinX;
        if (RightUp.x < MaxX && rate > RightUp.x / MaxX)
            rate = RightUp.x / MaxX;

        if (LeftBottom.y > MinY && rate > LeftBottom.y / MinY)
            rate = LeftBottom.y / MinY;
        if (RightUp.y < MaxY && rate > RightUp.y / MaxY)
            rate = RightUp.y / MaxY;


        if (rate < 1)
        {
            for (int i = 0; i < pts.Length; ++i)
            {
                Vector3 v = pts[i].Vec;
                pts[i].Vec = v * rate;
            }
        }
    }

    public static void ExecuteAllModules()
    {
        Module[] md = FindObjectsOfType<Module>();
        Line[] ln = FindObjectsOfType<Line>();
        Circle[] cn = FindObjectsOfType<Circle>();
        int mdLength = md.Length;
        int lnLength = ln.Length;
        int cnLength = cn.Length;
        float err = 0f;
        float previousErr = 0f;
        int ConvergencyCount = 0;
        //int PerturbationCount = 0;
        pts = FindObjectsOfType<Point>();
        for (int p = 0; p > pts.Length; p++) {
            if (pts[p].Fixed == false) { 
                //頂点を微動させる。
                Point pt = pts[p];
                float x = UnityEngine.Random.Range(-0.001f, 0.001f);
                float y = UnityEngine.Random.Range(-0.001f, 0.001f);
                Vector3 position = pt.transform.position;
                position.x += x;
                position.y += y;
                pt.Vec = position;
                pt.transform.position = position;
            }
        }
        AppMgr.ConvergencyAlert.GetComponent<TextMesh>().text = "";// 
        for (int repeat = 0; repeat < 200; repeat++)
        {
            previousErr = err;
            err = 0f;
            for (int i = 0; i < mdLength; i++)
            {
                if (md[i].Type != MENU.ADD_LOCUS)
                {
                    err += md[i].ExecuteModule();
                }
            }
            for (int i = 0; i < lnLength; i++)
            {
                err += ln[i].ExecuteModule();
            }
            for (int i = 0; i < cnLength; i++)
            {
                err += cn[i].ExecuteModule();
            }
            if (err >= previousErr)
            {
                ConvergencyCount += 1;
                previousErr = err;
                if (ConvergencyCount > 20)
                {
                    //Debug.Log("Conflict" + ConvergencyCount + ":" + PerturbationCount);
                    if (Japanese == 1)
                        AppMgr.ConvergencyAlert.GetComponent<TextMesh>().text = "図形の不合理";
                    else
                        AppMgr.ConvergencyAlert.GetComponent<TextMesh>().text = "Conflict";
                    ModuleOn = false;
                }
            }
            if (err < 0.00001f)
            {
                break;
            }
        }
        //Debug.Log(AppMgr.ConvergencyCount);
        for (int i = 0; i < md.Length; i++)
        {
            if (md[i].Type == MENU.ADD_LOCUS)
            {
                md[i].ExecuteModule();
            }
        }

    }

}



/// <summary> 様々なユーティリティ
/// </summary>
public class Util
{
    public static List<Log> logs = null;
    public static int LogLength = 0;
    static int LastLog = 0;

    /// プレハブ
    private static GameObject Prefab = null;

    /// ログフォルダー
    private static GameObject LogFolder = null;
    public static float LogLeft = 0;
    public static float StartTop = 4f;
    public static bool ShowLog = true;

    public static Color[] IsometryColor;
    public static Color[] IsometrySelectedColor;

    /// <summary>
    /// 画面固定のためのフラグ
    /// </summary>
    public static bool FixDisplay = false;


    public static float Magnitude(float x, float y)
    {
        return Mathf.Sqrt(x * x + y * y);
    }

    public static float Round3(float a)
    {
        return Mathf.Round(a * 1000f) / 1000f;
    }
    #region GetLogFolder
    private static void GetLogFolder()
    {
        if (LogFolder == null)
        {
            GameObject[] OBJs = MonoBehaviour.FindObjectsOfType<GameObject>();
            for (int i = 0; i < OBJs.Length; i++)
            {
                if (OBJs[i].name == "LogFolder")
                {
                    LogFolder = OBJs[i];
                }
            }
        }
    }
    #endregion

    #region AddPoint
    public static char[] GetNewPointName()
    {
        char[] name = { 'A' };
        for (int k = 0; k < 26; k++)
        {
            name[0] = (char)((int)('A') + k);
            //Debug.Log("k = "+k+", name = " + name[0]);
            bool OK = true;
            AppMgr.pts = MonoBehaviour.FindObjectsOfType<Point>();
            int size = AppMgr.pts.Length;
            for (int i = 0; i < size; i++)
            {
                Point pt = AppMgr.pts[i];
                //Debug.Log("PointName = " + pt.PointName);
                if (pt.PointName.Length == 1 && pt.PointName[0] == name[0])
                {
                    OK = false;
                    break;
                }
            }
            if (OK)
            {
                return name;
            }
        }
        name = new char[2];
        name[0] = 'A';
        name[1] = 'A';
        for (int k = 0; k < 26; k++)
        {
            name[1] = (char)((int)('A') + k);
            Debug.Log("k = " + k + ", name = " + name[0]);
            bool OK = true;
            AppMgr.pts = MonoBehaviour.FindObjectsOfType<Point>();
            int size = AppMgr.pts.Length;
            for (int i = 0; i < size; i++)
            {
                Point pt = AppMgr.pts[i];
                Debug.Log("PointName = " + pt.PointName);
                if (pt.PointName.Length == 2 && pt.PointName[0] == name[0] && pt.PointName[1] == name[1])
                {
                    OK = false;
                    break;
                }
            }
            if (OK)
            {
                return name;
            }
        }
        name[0] = 'B';
        name[1] = 'A';
        return name;
    }

    /// 頂点の生成
    public static Point AddPoint(Vector3 V, int pointId)
    {
        // プレハブを取得
        Prefab = Resources.Load<GameObject>("Prefabs/Point");
        // プレハブからインスタンスを生成
        GameObject obj = Point.Instantiate<GameObject>(Prefab, V, Quaternion.identity);
        Point pt = obj.GetComponent<Point>();
        if (pt != null)
        {
            pt.Id = pointId;
            pt.Vec = V;
            pt.Active = true;
            pt.parent = obj;
            pt.PointName = new string(GetNewPointName());
            AppMgr.pts = MonoBehaviour.FindObjectsOfType<Point>();
            pt.PTobject.GetComponent<TextMesh>().text = pt.PointName;
            pt.PTobject.transform.localPosition = Vector3.zero;
            //もし余分なログがある場合には   余分なログは消す．
            if (LastLog != LogLength)
            {
                DeleteNonactiveLog();
            }

            //ログを作成
            Prefab = Resources.Load<GameObject>("Prefabs/GameLog");
            if (Prefab == null) Debug.Log("no prefab for gamelog");
            GetLogFolder();
            GameObject LogObj = MonoBehaviour.Instantiate<GameObject>(Prefab, Vector3.zero, Quaternion.identity, LogFolder.transform);
            Log lg = LogObj.GetComponent<Log>();
            lg.MakePointLog(pt);
            pt.GameLog = LogObj;

            //ログをlogsに追加
            AddLog(lg);
        }

        return pt;
    }
    #endregion

    #region Midpoint
    public static Point AddMidpoint(int first, int second, int third, int moduleId)
    {
        Prefab = Resources.Load<GameObject>("Prefabs/Point");

        Point[] pp = MonoBehaviour.FindObjectsOfType<Point>();
        Vector3 v1 = Vector3.zero, v2 = Vector3.zero;
        if (pp != null)
        {
            for (int i = 0; i < pp.Length; ++i)
            {
                if (pp[i].Id == first)
                {
                    v1 = pp[i].Vec;
                }
                if (pp[i].Id == second)
                {
                    v2 = pp[i].Vec;
                }
            }
        }

        GameObject obj = Point.Instantiate(Prefab, 0.5f * v1 + 0.5f * v2, Quaternion.identity) as GameObject;
        Point pt = obj.GetComponent<Point>();
        if (pt != null)
        {
            pt.Vec = obj.transform.position;

            //中点の位置をプリセットする。
            pt.Id = third;
            pt.Active = true;
            pt.parent = obj;
            char[] name = { 'A' };
            name[0] = (char)('A' + pt.Id);
            pt.PointName = new string(name);
            AppMgr.pts = MonoBehaviour.FindObjectsOfType<Point>();
            pt.PTobject.GetComponent<TextMesh>().text = pt.PointName;
            pt.PTobject.transform.localPosition = Vector3.zero;
            //もし余分なログがある場合には   余分なログは消す．
            if (LastLog != LogLength)
            {
                DeleteNonactiveLog();
            }

            //中点のログを作成
            Prefab = Resources.Load<GameObject>("Prefabs/GameLog");
            GetLogFolder();
            GameObject LogObj = MonoBehaviour.Instantiate<GameObject>(Prefab, Vector3.zero, Quaternion.identity, LogFolder.transform);
            Log lg = LogObj.GetComponent<Log>();
            lg.MakePointLog(pt);
            //ログをlogsに追加
            AddLog(lg);
            pt.GameLog = LogObj;

            // モジュールの追加
            AddModule(MENU.ADD_MIDPOINT, first, second, third, moduleId);


        }
        return pt;
    }
    #endregion

    #region AddLine
    ///線分の生成
    public static Line AddLine(int first, int second, int lineId)
    {
        // プレハブを取得
        Prefab = Resources.Load<GameObject>("Prefabs/Line");
        // プレハブからインスタンスを生成
        GameObject g = Line.Instantiate(Prefab, Vector3.zero, Quaternion.identity) as GameObject;
        Line obj = g.GetComponent<Line>();
        if (obj != null)
        {
            obj.Point1Id = first;
            obj.Point2Id = second;
            obj.Id = lineId;
            obj.Active = true;
            obj.parent = g;
            obj.LineName = "L" + (obj.Id - 999);
            AppMgr.lns = MonoBehaviour.FindObjectsOfType<Line>();

            //もし余分なログがある場合には   余分なログは消す．
            if (LastLog != LogLength)
            {
                DeleteNonactiveLog();
            }
            //ログを作成
            Prefab = Resources.Load<GameObject>("Prefabs/GameLog");
            GetLogFolder();
            GameObject LogObj = MonoBehaviour.Instantiate<GameObject>(Prefab, Vector3.zero, Quaternion.identity, LogFolder.transform);
            Log lg = LogObj.GetComponent<Log>();
            lg.MakeLineLog(obj);
            //ログをlogsに追加
            AddLog(lg);
            obj.GameLog = LogObj;
        }
        return obj;
    }
    #endregion

    #region AddCircle
    /// 円の生成
    public static Circle AddCircle(int first, float rd, int circleId)
    {
        // プレハブを取得
        Prefab = Resources.Load<GameObject>("Prefabs/Circle");
        // プレハブからインスタンスを生成
        GameObject g = Point.Instantiate(Prefab, Vector3.zero, Quaternion.identity) as GameObject;
        Circle obj = g.GetComponent<Circle>();
        if (obj != null)
        {
            obj.CenterPointId = first;
            obj.Radius = rd;
            obj.Id = circleId;
            obj.Active = true;
            obj.parent = g;
            obj.CircleName = "C" + (obj.Id - 1999);
            AppMgr.cis = MonoBehaviour.FindObjectsOfType<Circle>();

            //もし余分なログがある場合には   余分なログは消す．
            if (LastLog != LogLength)
            {
                DeleteNonactiveLog();
            }
            //円のログを作成
            Prefab = Resources.Load<GameObject>("Prefabs/GameLog");
            GetLogFolder();
            GameObject LogObj = MonoBehaviour.Instantiate<GameObject>(Prefab, Vector3.zero, Quaternion.identity, LogFolder.transform);
            Log lg = LogObj.GetComponent<Log>();
            lg.MakeCircleLog(obj);
            //ログをlogsに追加
            AddLog(lg);
            obj.GameLog = LogObj;
        }
        return obj;
    }
    #endregion

    #region AddModule
    public static string GetModuleNameByType(int type)
    {
        if (AppMgr.Japanese == 1) { 
            switch (type)
            {
                case MENU.POINT_ON_POINT:
                    return "点 - 点";
                case MENU.POINT_ON_LINE:
                    return "点 - 直線";
                case MENU.POINT_ON_CIRCLE:
                    return "点 - 円";
                case MENU.CROSSING_LL:
                    return "交点";
                case MENU.LINES_ISOMETRY:
                    return "等長";//"辺長比"
                case MENU.LINES_PERPENDICULAR:
                    return "垂直";//"一定角"
                case MENU.LINES_PARALLEL:
                    return "平行";
                case MENU.LINE_HORIZONTAL:
                    return "水平";
                case MENU.CIRCLE_TANGENT_LINE:
                    return "円 - 直線";
                case MENU.CIRCLE_TANGENT_CIRCLE:
                    return "円 - 円";
                case MENU.TRIANGLE:
                    return "三角形";
                case MENU.QUADRILATERAL:
                    return "四角形";
                case MENU.ADD_MIDPOINT:
                    return "中点";//"内分点"
                case MENU.ANGLE:
                    return "角度";
                case MENU.BISECTOR:
                    return "角度 - 角度";
                case MENU.ADD_LOCUS:
                    return "軌跡";
            }
        }
        else
        {
            switch (type)
            {
                case MENU.POINT_ON_POINT:
                    return "point - point";
                case MENU.POINT_ON_LINE:
                    return "point - line";
                case MENU.POINT_ON_CIRCLE:
                    return "point - circle";
                case MENU.CROSSING_LL:
                    return "crossing";
                case MENU.LINES_ISOMETRY:
                    return "isometry";//"辺長比"
                case MENU.LINES_PERPENDICULAR:
                    return "perpendicular";//"一定角"
                case MENU.LINES_PARALLEL:
                    return "parallel";
                case MENU.LINE_HORIZONTAL:
                    return "horizontal";
                case MENU.CIRCLE_TANGENT_LINE:
                    return "circle - line";
                case MENU.CIRCLE_TANGENT_CIRCLE:
                    return "circle - circle";
                case MENU.TRIANGLE:
                    return "triangle";
                case MENU.QUADRILATERAL:
                    return "quadrilateral";
                case MENU.ADD_MIDPOINT:
                    return "midpoint";//"内分点"
                case MENU.ANGLE:
                    return "angle";
                case MENU.BISECTOR:
                    return "angle - angle";
                case MENU.ADD_LOCUS:
                    return "locus";
            }
        }
        return "";
    }
    /// パーティクルの生成
    public static Module AddModule(int type, int first, int second, int third, int moduleId, bool fixRatio = true)
    {
        // プレハブを取得
        Prefab = Resources.Load<GameObject>("Prefabs/Module");
        //// プレハブからインスタンスを生成
        /// インスタンスを生成してスクリプトを返す.
        GameObject g = Module.Instantiate(Prefab, Vector3.zero, Quaternion.identity) as GameObject;
        Module obj = g.GetComponent<Module>();
        if (obj != null)
        {
            obj.Type = type;
            obj.Object1Id = first;
            obj.Object2Id = second;
            obj.Object3Id = third;
            obj.Id = moduleId;
            obj.Active = true;
            obj.parent = g;
            obj.ModuleName = GetModuleNameByType(type);
            obj.FixRatio = fixRatio;
            Debug.Log("check here");
            AppMgr.mds = MonoBehaviour.FindObjectsOfType<Module>();

            if (obj.Type == MENU.ADD_LOCUS)
            {
                Point[] PTS = MonoBehaviour.FindObjectsOfType<Point>();
                for (int i = 0; i < PTS.Length; i++)
                {
                    if (PTS[i].Id == obj.Object1Id)
                    {
                        obj.PreVec = PTS[i].Vec;
                        break;
                    }
                }
            }

            //もし余分なログがある場合には   余分なログは消す．
            if (LastLog != LogLength)
            {
                DeleteNonactiveLog();
            }
            //モジュールのログを作成
            Prefab = Resources.Load<GameObject>("Prefabs/GameLog");
            GetLogFolder();
            GameObject LogObj = MonoBehaviour.Instantiate<GameObject>(Prefab, Vector3.zero, Quaternion.identity, LogFolder.transform);
            Log lg = LogObj.GetComponent<Log>();
            lg.MakeModuleLog(obj);
            //ログをlogsに追加
            AddLog(lg);
            obj.GameLog = LogObj;
            AppMgr.ExecuteAllModules();// 念のためにモジュールを一度回してみる。
        }
        return obj;
    }
    #endregion

    #region AddAngleMark
    public static AngleMark AddAngleMark(int first, int second, GameObject parentModule = null)// omit 'Id'
    {//
        Prefab = Resources.Load<GameObject>("Prefabs/AngleMark");
        GameObject g = AngleMark.Instantiate(Prefab, Vector3.zero, Quaternion.identity) as GameObject;
        AngleMark obj = g.GetComponent<AngleMark>();
        if (obj != null)
        {
            obj.Object1Id = first;
            obj.Object2Id = second;
            obj.parent = parentModule;
            obj.RightAngle = true;
        }
        return obj;
    }
    public static AngleMark AddAngleMark(int first, int second, int third, GameObject parentModule = null)// omit 'Id'
    {//
        Prefab = Resources.Load<GameObject>("Prefabs/AngleMark");
        GameObject g = AngleMark.Instantiate<GameObject>(Prefab, Vector3.zero, Quaternion.identity);
        AngleMark obj = g.GetComponent<AngleMark>();
        if (obj != null)
        {
            obj.Object1Id = first;
            obj.Object2Id = second;
            obj.Object3Id = third;
            obj.parent = parentModule;
            obj.RightAngle = false;
        }
        return obj;
    }
    #endregion

    #region AddThinLine
    public static ThinLine AddThinLine(int first, int second)// first is a point, the second is a line.
    {//
        Prefab = Resources.Load<GameObject>("Prefabs/ThinLine");
        GameObject g = ThinLine.Instantiate<GameObject>(Prefab, Vector3.zero, Quaternion.identity);
        ThinLine TL = g.GetComponent<ThinLine>();
        if (TL != null)
        {
            TL.PointId = first;
            TL.LineId = second;
            TL.parent = g;
        }
        return TL;
    }
    #endregion

    #region FindThinLine
    public static ThinLine FindThinLineByPoint(int ptId)
    {
        GameObject[] objs = MonoBehaviour.FindObjectsOfType<GameObject>();
        if (objs != null)
        {
            for (int k = 0; k < objs.Length; k++)
            {
                ThinLine tl = (ThinLine)objs[k].GetComponent("ThinLine");
                if (tl != null)
                {
                    if (tl.PointId == ptId)
                    {
                        return tl;
                    }
                }
            }
        }
        return null;
    }
    public static ThinLine FindThinLineByPointLine(Point pt, Line ln)
    {
        GameObject[] objs = MonoBehaviour.FindObjectsOfType<GameObject>();
        if (objs != null)
        {
            for (int k = 0; k < objs.Length; k++)
            {
                ThinLine tl = (ThinLine)objs[k].GetComponent("ThinLine");
                if (tl != null)
                {
                    if (tl.PointId == pt.Id && tl.LineId == ln.Id)
                    {
                        return tl;
                    }
                }
            }
        }
        return null;
    }
    #endregion

    #region Logまわり
    public static void InitLog()
    {
        // GameLogの親すべて消去する
        Log[] OBJs = MonoBehaviour.FindObjectsOfType<Log>();
        for (int i = OBJs.Length - 1; i >= 0; i--)
        {
            MonoBehaviour.Destroy(OBJs[i].gameObject);
        }
        logs = new List<Log>(100);
        LastLog = 0;
        LogLength = 0;

    }

    public static void AddLog(Log _log)
    {
        if (logs == null)
        {
            InitLog();
        }
        //DeleteNonactiveLog();
        logs.Add(_log);
        LogLength = LastLog + 1;
        LastLog = LogLength;

        CopyLog("TmpLog.txt", "TmpBeforeLastLog.txt");
        SaveLog("TmpLog.txt");

    }

    public static void MakeALogFixed(int MOP)
    {
        if (logs == null) return;
        for (int i = 0; i < LogLength; ++i)
        {
            if (logs[i].Id == MOP)
            {
                logs[i].Fixed = !logs[i].Fixed;
            }

        }

    }

    public static void RemakeLog()
    {// 現状の頂点の座標をlogsに反映する。
        if (logs == null) return;
        for (int i = 0; i < LogLength; i++)
        {
            if (logs[i].ObjectType == "Point")
            {
                GameObject g = logs[i].parent;
                if (g != null)
                {
                    Point p = g.GetComponent<Point>();
                    if (p != null) // 多分不要だが念のため。
                        logs[i].Vec = p.Vec;
                }
            }
        }
    }

    public static void DeleteLogAtID(int MOP)
    {
        if (logs == null) return;
        for (int i = 0; i < LogLength; i++)
        {
            if (logs[i].Id == MOP)
            {
                LogLength--;
                if (logs[i].Active)
                    LastLog--;
                DeleteLogAt(i);
                return;
            }
        }
    }

    static void DeleteLogAt(int i)
    {
        if (logs == null) return;
        MonoBehaviour.Destroy(logs[i].gameObject);
        logs.RemoveAt(i);
    }


    public static void DeleteNonactiveLog()
    {
        if (logs == null) return;
        for (int i = LogLength - 1; i >= LastLog; i--)
        {
            Debug.Log("i = " + i);
            if (!logs[i].Active)
            {
                //Debug.Log(logs[i].parent);
                logs[i].parent.SetActive(true);
                MonoBehaviour.Destroy(logs[i].parent);
                MonoBehaviour.Destroy(logs[i].gameObject);
                logs.RemoveAt(i);
            }
        }
        LogLength = LastLog;
    }


    public static bool SaveText(string path, string text)
    {
        //ストリームライターwriterに書き込む
        try
        {
            using (StreamWriter writer = new StreamWriter(Application.dataPath + path, false))
            {
                writer.Write(text);
                writer.Flush();
                writer.Close();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return false;
        }
        return true;
    }

    public static bool SaveLog(string path)
    {
        //ストリームライターwriterに書き込む
        if (logs == null) return false;
        try
        {
            using (StreamWriter writer = new StreamWriter(Application.dataPath + "/Resources/" + path, false))
            {
                //Debug.Log("start savelog : logLength = " + LogLength);
                for (int i = 0; i < LogLength; i++)
                {
                    writer.WriteLine(logs[i].ToString());
                }
                writer.Flush();
                writer.Close();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return false;
        }
        return true;
    }
    #endregion

    #region ファイル保存

    // Saves a file with the textToSave using a path
    private static void SaveFileUsingPath(string path)
    {
        if (logs == null) return;
        // Make sure path is not null or empty
        if (!String.IsNullOrEmpty(path)/* && !String.IsNullOrEmpty(_textToSave)*/)
        {
            Debug.Log("SaveFileUsingPath - " + path);
            string ext = path.Substring(path.Length - 4);
            if (ext.Contains("png") || ext.Contains("PNG"))
            {
                BackGroundScreen BGS = MonoBehaviour.FindObjectOfType<BackGroundScreen>();
                BGS.CaptureFromCamera(path);
            }
            else if (ext.Contains("txt") || ext.Contains("TXT"))
            {
                try
                {
                    using (StreamWriter writer = new StreamWriter(path, false))
                    {
                        //Debug.Log("start savelog : logLength = " + LogLength);
                        for (int i = 0; i < LogLength; i++)
                        {
                            if (logs[i].Active)
                            {
                                writer.WriteLine(logs[i].ToString());
                            }
                        }
                        writer.Flush();
                        writer.Close();
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }
            }
            else if (ext.Contains("tex"))
            {
                SaveTeXFileUsingPath(path);
            }
            else if (ext.Contains("xml"))
            {
                SaveXmlFileUsingPath(path);
            }
        }
        else
        {
            Debug.Log("Invalid path or empty file given");
        }
        AppMgr.DrawOn = true;
        AppMgr.KeyOn = true;
        AppMgr.FileDialogOn = false;
    }

    //private static  bool PortraitMode = false;


    public static bool SaveLogSelectFile()
    {
        //Prefab = Resources.Load<GameObject>("Prefabs/FileBrowser");
        //GameObject g = FileBrowser.Instantiate<GameObject>(Prefab, Vector3.zero, Quaternion.identity);
        //FileBrowser fb = g.GetComponent<FileBrowser>();
        //string[] ext = new string[] { "txt", "png" };
        //fb.SetupFileBrowser(ViewMode.Landscape);// : ViewMode.Landscape);Portrait
        //fb.SaveFilePanel("pointLineFigure", ext);
        //fb.OnFileSelect += SaveFileUsingPath;
        //string[] exts = {"txt", "png"};
        //string path = Crosstales.FB.FileBrowser.SaveFile("Save a PointLine file", "", "SamplePointLine",exts);
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Text Files", ".txt"), new FileBrowser.Filter("Image Files", ".png"), new FileBrowser.Filter("TeX Files", ".tex"), new FileBrowser.Filter("Xml Files", ".xml"));
        FileBrowser.SetDefaultFilter(".txt");
        FileBrowser.ShowSaveDialog(onSaveSuccess, onCancel, FileBrowser.PickMode.FilesAndFolders, false, "",
            "SamplePointLine.txt", "Save PointLine File");

        return false;
    }
    static void onSaveSuccess(string[] paths)
    {
        Debug.Log("" + paths[0]);
        SaveFileUsingPath(paths[0]);
    }
    private static void SaveXmlFileUsingPath(string path)
    {
        XElement xml = new("PointLine");
        for (int l = 0; l < LogLength; l++)
        {
            Log log = logs[l];
            int id = log.Id;
            //Debug.Log(""+ log.ObjectType);
            if (log.Active == false)
            {
                ;
            }
            else if (log.ObjectType == "Point")
            {
                Point pt = null;
                for (int k = 0; k < AppMgr.pts.Length; k++)
                {
                    if (AppMgr.pts[k].Id == id)
                    {
                        pt = AppMgr.pts[k];
                        break;
                    }
                }
                if (pt != null)
                {
                    XElement datas = new XElement("Object",
                        new XElement("ObjectType", "Point"),
                        new XElement("x", pt.Vec.x),
                        new XElement("y", pt.Vec.y),
                        new XElement("z", pt.Vec.z),
                        new XElement("id", pt.Id),
                        new XElement("fixed", pt.Fixed),
                        new XElement("name", pt.PointName),
                        new XElement("displayName", pt.ShowPointName),
                        new XElement("active", pt.Active)
                        );
                    xml.Add(datas);
                }
            }
            else if (log.ObjectType == "Line")
            {
                Line ln = null;
                for (int k = 0; k < AppMgr.lns.Length; k++)
                {
                    if (AppMgr.lns[k].Id == id)
                    {
                        ln = AppMgr.lns[k];
                        break;
                    }
                }
                if (ln != null)
                {
                    XElement datas = new XElement("Object",
                        new XElement("ObjectType", "Line"),
                        new XElement("Point1Id", ln.Point1Id),
                        new XElement("Point2Id", ln.Point2Id),
                        new XElement("ShowLength", ln.ShowLength),
                        new XElement("FixLength", ln.FixLength),
                        new XElement("para", ln.para),
                        new XElement("Isometry", ln.Isometry),
                        new XElement("Bracket", ln.Bracket),
                        new XElement("BracketText", ln.BracketText),
                        new XElement("edgeLength", ln.edgeLength),
                        new XElement("id", ln.Id),
                        new XElement("name", ln.LineName),
                        new XElement("active", ln.Active)
                        );
                    xml.Add(datas);
                }
            }
            else if (log.ObjectType == "Circle")
            {
                Circle ci = null;
                for (int k = 0; k < AppMgr.cis.Length; k++)
                {
                    if (AppMgr.cis[k].Id == id)
                    {
                        ci = AppMgr.cis[k];
                        break;
                    }
                }
                if (ci != null)
                {
                    XElement datas = new XElement("Object",
                        new XElement("ObjectType", "Circle"),
                        new XElement("CenterPointId", ci.CenterPointId),
                        new XElement("Radius", ci.Radius),
                        new XElement("FixedRadius", ci.FixedRadius),
                        new XElement("para", ci.para),
                        new XElement("FixRadius", ci.FixRadius),
                        new XElement("id", ci.Id),
                        new XElement("name", ci.CircleName),
                        new XElement("active", ci.Active)
                        );
                    xml.Add(datas);
                }
            }
            else if (log.ObjectType == "Module")
            {
                Module md = null;
                for (int k = 0; k < AppMgr.mds.Length; k++)
                {
                    if (AppMgr.mds[k].Id == id)
                    {
                        md = AppMgr.mds[k];
                        break;
                    }
                }
                if (md != null)
                {
                    XElement datas = new XElement("Object",
                        new XElement("ObjectType", "Module"),
                        new XElement("Type", md.Type),
                        new XElement("Object1Id", md.Object1Id),
                        new XElement("Object2Id", md.Object2Id),
                        new XElement("Object3Id", md.Object3Id),
                        new XElement("Object4Id", md.Object4Id),
                        new XElement("Ratio1", md.Ratio1),
                        new XElement("Ratio2", md.Ratio2),
                        new XElement("Constant", md.Constant),
                        new XElement("ShowConstant", md.ShowConstant),
                        new XElement("FixAngle", md.FixAngle),
                        new XElement("FixRatio", md.FixRatio),
                        new XElement("Parameter", md.Parameter),
                        new XElement("ParaWeight", md.ParaWeight),
                        new XElement("PolygonOption", md.PolygonOption),
                        new XElement("id", md.Id),
                        new XElement("name", md.ModuleName),
                        new XElement("active", md.Active)
                        );
                    xml.Add(datas);
                }
            }
        }
        xml.Save(@path);
        Debug.Log(xml);
    }


    #endregion

    #region ファイルを開く

    private static void LoadFileUsingPath(string path)
    {
        string ext = path.Substring(path.Length - 4);
        if (ext == ".xml")
        {
            LoadXmlFileUsingPath(path);
        }
        else if (ext == ".PNG" || ext == ".png" || ext == ".JPG" || ext == ".jpg")
        {
            LoadImageFileUsingPath(path);
        }
        else
        {
            ClickOnPanel.DeleteAll();
            if (path != null && path.Length != 0)
            {
                try
                {
                    using (StreamReader reader = new StreamReader(path, false))
                    {
                        InitLog();
                        string str;
                        int PId = -1, LId = -1, CId = -1, MId = -1;
                        do
                        {
                            str = reader.ReadLine();
                            if (str == null) break;//多分要らない。
                            else
                            {
                                Log lg = GetLogFromString(str);
                                //AddLog(lg);
                                if (lg.ObjectType == "Point")
                                {
                                    if (PId < lg.Id) PId = lg.Id;
                                }
                                else if (lg.ObjectType == "Line")
                                {
                                    if (LId < lg.Id) LId = lg.Id;
                                }
                                else if (lg.ObjectType == "Circle")
                                {
                                    if (CId < lg.Id) CId = lg.Id;
                                }
                                else if (lg.ObjectType == "Module")
                                {
                                    if (MId < lg.Id) MId = lg.Id;
                                }
                            }
                        }
                        while (str != null);
                        reader.Close();
                        ClickOnPanel.SetId(PId + 1, LId + 1, CId + 1, MId + 1);
                        AppMgr.pts = MonoBehaviour.FindObjectsOfType<Point>();
                        AppMgr.lns = MonoBehaviour.FindObjectsOfType<Line>();
                        AppMgr.cis = MonoBehaviour.FindObjectsOfType<Circle>();
                        AppMgr.mds = MonoBehaviour.FindObjectsOfType<Module>();
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }
            else
            {
                Debug.Log("Invalid path given");
            }
            AppMgr.DrawOn = true;
            AppMgr.KeyOn = true;
            AppMgr.FileDialogOn = false;
        }
    }

    public static bool OpenLogSelectFile()
    {
        //Prefab = Resources.Load<GameObject>("Prefabs/FileBrowser");
        //GameObject g = FileBrowser.Instantiate<GameObject>(Prefab, Vector3.zero, Quaternion.identity);
        //FileBrowser fb = g.GetComponent<FileBrowser>();
        //string[] ext = new string[] { "txt", "plf" };
        //fb.SetupFileBrowser(PortraitMode ? ViewMode.Portrait : ViewMode.Landscape);
        //fb.OpenFilePanel(ext);
        //fb.OnFileSelect += LoadFileUsingPath;
        //string path = Crosstales.FB.FileBrowser.OpenSingleFile("Open a PointLine file", "", "txt");
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Text Files", ".txt"), 
            new FileBrowser.Filter("Image Files", ".png"), new FileBrowser.Filter("Xml Files", ".xml"));
        FileBrowser.SetDefaultFilter(".txt");
        FileBrowser.ShowLoadDialog(onLoadSuccess, onCancel, FileBrowser.PickMode.FilesAndFolders, false, "", "SamplePointLine.txt", "Open PointLine File");
        return false;
    }


    static void onLoadSuccess(string[] paths)
    {
        Debug.Log("" + paths[0]);
        LoadFileUsingPath(paths[0]);
    }
    static void onCancel()
    {
        AppMgr.DrawOn = true;
        AppMgr.KeyOn = true;
        AppMgr.FileDialogOn = false;
    }


    public static Log GetLogFromString(string str)
    {// 文字列からlogを起こすのだが、実物も伴う。
        string[] item = str.Split(',');
        if (item[0] == "Point")
        {
            float vx = float.Parse(item[1]);
            float vy = float.Parse(item[2]);
            float vz = float.Parse(item[3]);
            Vector3 vec = new Vector3(vx, vy, vz);
            int id = int.Parse(item[4]);
            bool fxd = bool.Parse(item[5]);
            bool act = bool.Parse(item[6]);
            Point pt = Util.AddPoint(vec, id);//　この段階でログファイルも作り終わっている。
            pt.Fixed = fxd;
            string pname = "";
            if (item.Length > 7)
            {
                pt.PTobject.GetComponent<TextMesh>().text = item[7];
            }
            else { 
                char[] name = { 'A' };
                name[0] = (char)('A' + id);
                pname = new string(name);
                pt.PTobject.GetComponent<TextMesh>().text = pname;
            }
            if (item.Length > 8)
            {
                pt.ShowPointName = bool.Parse(item[8]);
            }
            //Debug.Log(pt.PTobject.GetComponent<Text>().text);
            Log lg = pt.GameLog.GetComponent<Log>();
            lg.MakePointLog(id, vec, pt.parent, fxd, act, pname);
            return lg;
        }
        else if (item[0] == "Line")
        {
            int o1 = int.Parse(item[1]);
            int o2 = int.Parse(item[2]);
            int id = int.Parse(item[3]);
            bool act = bool.Parse(item[4]);
            Line ln = AddLine(o1, o2, id);
            if (item.Length > 5)
            {
                ln.ShowLength = bool.Parse(item[5]);
                ln.FixLength = bool.Parse(item[6]);
                ln.para = float.Parse(item[7]);
                ln.Isometry = int.Parse(item[8]);
                ln.Bracket = bool.Parse(item[9]);
                ln.BracketText = item[10];
                ln.edgeLength = float.Parse(item[11]);
                ln.LineName = item[12];
            }
            GameObject[] OBJs = MonoBehaviour.FindObjectsOfType<GameObject>();
            for (int i = 0; i < OBJs.Length; i++)
            {
                Point PT = OBJs[i].GetComponent<Point>();
                if (PT != null)
                {
                    if (PT.Id == o1)
                    {
                        ln.Point1 = OBJs[i];
                    }
                    if (PT.Id == o2)
                    {
                        ln.Point2 = OBJs[i];
                    }
                }
            }
            Log lg = ln.GameLog.GetComponent<Log>();
            lg.MakeLineLog(id, o1, o2, ln.parent, act, ln.LineName);
            return lg;
        }
        else if (item[0] == "Circle")
        {
            int o1 = int.Parse(item[1]);
            float rad = float.Parse(item[2]);
            int id = int.Parse(item[3]);
            bool act = bool.Parse(item[4]);
            Circle ci = Util.AddCircle(o1, rad, id);
            if (item.Length > 5)
            {
                ci.FixedRadius = float.Parse(item[5]);
                ci.para = float.Parse(item[6]);
                ci.FixRadius = bool.Parse(item[7]);
                ci.CircleName = item[8];
            }
            GameObject[] OBJs = MonoBehaviour.FindObjectsOfType<GameObject>();
            for (int i = 0; i < OBJs.Length; i++)
            {
                Point PT = OBJs[i].GetComponent<Point>();
                if (PT != null)
                {
                    if (o1 == PT.Id)
                    {
                        ci.CenterPoint = OBJs[i];
                        break;
                    }
                }
            }
            Log lg = ci.GameLog.GetComponent<Log>();
            lg.MakeCircleLog(id, o1, rad, ci.parent, act, ci.CircleName);
            return lg;
        }
        else if (item[0] == "Module")
        {
            int mt = int.Parse(item[1]);
            int o1 = int.Parse(item[2]);
            int o2 = int.Parse(item[3]);
            int o3 = int.Parse(item[4]);
            int o4 = -1;
            int id = int.Parse(item[5]);
            bool act = bool.Parse(item[6]);
            float ra1 = 1f, ra2 = 1f, cst = Mathf.PI / 2;
            if (item.Length > 7)
            {
                ra1 = float.Parse(item[7]);
                ra2 = float.Parse(item[8]);
                cst = float.Parse(item[9]);
            }
            bool showC = true, fixA = false, fixR = false;
            if (item.Length > 10)
            {
                showC = bool.Parse(item[10]);
                fixA = bool.Parse(item[11]);
                fixR = bool.Parse(item[12]);
            }
            //オブジェクト作成
            Module MD = Util.AddModule(mt, o1, o2, o3, id);
            //パラメータ調整
            MD.Ratio1 = ra1;
            MD.Ratio2 = ra2;
            MD.Constant = cst;
            MD.ShowConstant = showC;
            MD.FixAngle = fixA;
            MD.FixRatio = fixR;
            if (item.Length > 13)
            {
                MD.Parameter = float.Parse(item[13]);
                MD.ParaWeight = float.Parse(item[14]);
                MD.ModuleName = item[15];
            }
            if (item.Length > 16) { 
                MD.Object4Id = o4 = int.Parse(item[16]);
                MD.PolygonOption = int.Parse(item[17]);
            }
            //Debug.Log(MD.ToString());
            //ログ作成
            Log lg = MD.GameLog.GetComponent<Log>();
            lg.MakeModuleLog(id, mt, o1, o2, o3, o4, MD.parent, act, MD.ModuleName);
            if (mt == MENU.LINES_PERPENDICULAR)
            {// 直交モジュールの時には直角マークを付ける。
                AddAngleMark(o1, o2, MD.gameObject);
            }
            else if (mt == MENU.ANGLE)
            {// 角度モジュールの時には角度マークを付ける。
                AddAngleMark(o1, o2, o3, MD.gameObject);
            }
            if (mt == MENU.POINT_ON_LINE)
            {// 点を直線上に、のときには補助線を付ける。
                AddThinLine(o1, o2);
            }
            return lg;
        }
        return new Log();
    }

    private static void LoadXmlFileUsingPath(string path)
    {
        ClickOnPanel.DeleteAll();
        if (path != null && path.Length != 0)
        {
            //try
            {
                XElement xml = XElement.Load(@path);
                IEnumerable<XElement> infos = from item in xml.Elements("Object") select item;
                foreach (XElement info in infos)
                {
                    string objectType = info.Element("ObjectType").Value;
                    if (objectType == "Point")
                    {
                        float vx=0f, vy=0f, vz=0f;
                        if (info.Element("x")==null || info.Element("y") == null)
                        {
                            vx = UnityEngine.Random.value*10f - 5f;
                            vy = UnityEngine.Random.value*10f - 5f;
                        }
                        else
                        {
                            vx = float.Parse(info.Element("x").Value);
                            vy = float.Parse(info.Element("y").Value);
                        }
                        if (info.Element("z") == null)
                            vz = 0f;
                        else 
                            float.Parse(info.Element("z").Value);
                        Vector3 vec = new Vector3(vx, vy, vz);
                        int id = int.Parse(info.Element("id").Value);// id is required
                        Point pt = Util.AddPoint(vec, id);//　この段階でログファイルも作り終わっている。
                        bool fxd = false;
                        if (info.Element("fixed")!=null)
                            fxd = bool.Parse(info.Element("fixed").Value);
                        bool act = true;
                        if (info.Element("active")!=null)
                            act = bool.Parse(info.Element("active").Value);
                        pt.Fixed = fxd;
                        string pname = "";
                        if (info.Element("name") == null)
                        {
                            char[] name = { 'A' };
                            name[0] = (char)('A' + id);
                            pname = new string(name);
                        }
                        else
                        {
                            pname = info.Element("name").Value;
                        }
                        pt.PTobject.GetComponent<TextMesh>().text = pname;
                        //Debug.Log(pt.PTobject.GetComponent<Text>().text);
                        Log lg = pt.GameLog.GetComponent<Log>();
                        lg.MakePointLog(id, vec, pt.parent, fxd, act, pname);
                    }
                    else if (objectType == "Line")
                    {
                        int o1 = int.Parse(info.Element("Point1Id").Value);// required
                        int o2 = int.Parse(info.Element("Point2Id").Value);// required
                        int id = int.Parse(info.Element("id").Value);// required
                        Line ln = AddLine(o1, o2, id);//　この段階でログファイルも作り終わっている。
                        bool act = true;
                        if (info.Element("active")!=null) 
                            act = bool.Parse(info.Element("active").Value);
                        if (info.Element("name") == null)
                            ln.LineName = "L" + id;
                        else 
                            ln.LineName = info.Element("name").Value;
                        if (info.Element("ShowLength")!=null)
                            ln.ShowLength = bool.Parse(info.Element("ShowLength").Value);
                        if (info.Element("FixLength")!=null)
                           ln.FixLength = bool.Parse(info.Element("FixLength").Value);
                        if (info.Element("para") != null)
                            ln.para = float.Parse(info.Element("para").Value);
                        if (info.Element("Isometry")!=null)
                            ln.Isometry = int.Parse(info.Element("Isometry").Value);
                        if (info.Element("Bracket")!=null)
                            ln.Bracket = bool.Parse(info.Element("Bracket").Value);
                        if (info.Element("BracketText")!=null)
                            ln.BracketText = info.Element("BracketText").Value;
                        if (info.Element("edgeLength")!=null)
                            ln.edgeLength = float.Parse(info.Element("edgeLength").Value);
                        GameObject[] OBJs = MonoBehaviour.FindObjectsOfType<GameObject>();
                        for (int i = 0; i < OBJs.Length; i++)
                        {
                            Point PT = OBJs[i].GetComponent<Point>();
                            if (PT != null)
                            {
                                if (PT.Id == o1)
                                {
                                    ln.Point1 = OBJs[i];
                                }
                                if (PT.Id == o2)
                                {
                                    ln.Point2 = OBJs[i];
                                }
                            }
                        }
                        Log lg = ln.GameLog.GetComponent<Log>();
                        lg.MakeLineLog(id, o1, o2, ln.parent, act, ln.LineName);
                    }
                    else if (objectType == "Circle")
                    {
                        int o1 = int.Parse(info.Element("CenterPointId").Value);
                        float rad = float.Parse(info.Element("Radius").Value);
                        int id = int.Parse(info.Element("id").Value);
                        bool act = bool.Parse(info.Element("active").Value);
                        Circle ci = Util.AddCircle(o1, rad, id);
                        ci.FixedRadius= float.Parse(info.Element("FixedRadius").Value); 
                        ci.para = float.Parse(info.Element("para").Value); 
                        ci.FixRadius = bool.Parse(info.Element("FixRadius").Value); 
                        GameObject[] OBJs = MonoBehaviour.FindObjectsOfType<GameObject>();
                        for (int i = 0; i < OBJs.Length; i++)
                        {
                            Point PT = OBJs[i].GetComponent<Point>();
                            if (PT != null)
                            {
                                if (o1 == PT.Id)
                                {
                                    ci.CenterPoint = OBJs[i];
                                    break;
                                }
                            }
                        }
                        Log lg = ci.GameLog.GetComponent<Log>();
                        lg.MakeCircleLog(id, o1, rad, ci.parent, act, ci.CircleName);
                    }
                    else if (objectType == "Module")
                    {
                        int mt = int.Parse(info.Element("Type").Value);
                        int o1 = int.Parse(info.Element("Object1Id").Value);
                        int o2 = int.Parse(info.Element("Object2Id").Value);
                        int o3 = (info.Element("Object3Id")!=null)? int.Parse(info.Element("Object3Id").Value):-1;
                        int o4 = (info.Element("Object4Id")!=null)? int.Parse(info.Element("Object4Id").Value):-1;
                        int id = int.Parse(info.Element("id").Value);
                        //オブジェクト作成
                        Module MD = Util.AddModule(mt, o1, o2, o3, id);
                        //パラメータ調整
                        MD.Active = bool.Parse(info.Element("active").Value);
                        MD.Ratio1 = float.Parse(info.Element("Ratio1").Value);
                        MD.Ratio2 = float.Parse(info.Element("Ratio2").Value);
                        MD.Constant = float.Parse(info.Element("Constant").Value   );
                        MD.Parameter = float.Parse(info.Element("Parameter").Value   );
                        MD.ParaWeight = float.Parse(info.Element("ParaWeight").Value   );
                        MD.ShowConstant = (bool.Parse(info.Element("ShowConstant").Value) == true);
                        MD.FixAngle = (bool.Parse(info.Element("FixAngle").Value) == true);
                        MD.FixRatio = (bool.Parse(info.Element("FixRatio").Value) == true);
                        MD.PolygonOption = int.Parse(info.Element("PolygonOption").Value);
                        //Debug.Log(MD.ToString());
                        //ログ作成
                        Log lg = MD.GameLog.GetComponent<Log>();
                        lg.MakeModuleLog(id, mt, o1, o2, o3, o4, MD.parent, MD.Active, MD.ModuleName);
                        if (mt == MENU.LINES_PERPENDICULAR)
                        {// 直交モジュールの時には直角マークを付ける。
                            AddAngleMark(o1, o2, MD.gameObject);
                        }
                        else if (mt == MENU.ANGLE)
                        {// 角度モジュールの時には角度マークを付ける。
                            AddAngleMark(o1, o2, o3, MD.gameObject);
                        }
                        if (mt == MENU.POINT_ON_LINE)
                        {// 点を直線上に、のときには補助線を付ける。
                            AddThinLine(o1, o2);
                        }
                    }
                }
                int PId = 0, LId = 1000, CId = 2000, MId = 3000;
                AppMgr.pts = MonoBehaviour.FindObjectsOfType<Point>();
                AppMgr.lns = MonoBehaviour.FindObjectsOfType<Line>();
                AppMgr.cis = MonoBehaviour.FindObjectsOfType<Circle>();
                AppMgr.mds = MonoBehaviour.FindObjectsOfType<Module>();
                for (int k = 0; k < AppMgr.pts.Length; k++)
                {
                    if (PId <= AppMgr.pts[k].Id) PId = AppMgr.pts[k].Id+1;
                }
                for (int k = 0; k < AppMgr.lns.Length; k++)
                {
                    if (LId <= AppMgr.lns[k].Id) LId = AppMgr.lns[k].Id + 1;
                }
                for (int k = 0; k < AppMgr.cis.Length; k++)
                {
                    if (CId <= AppMgr.cis[k].Id) CId = AppMgr.cis[k].Id + 1;
                }
                for (int k = 0; k < AppMgr.mds.Length; k++)
                {
                    if (MId <= AppMgr.mds[k].Id) MId = AppMgr.mds[k].Id + 1;
                }
                ClickOnPanel.SetId(PId, LId, CId, MId);
            }
            //catch (Exception e)
            //{
            //    Debug.LogError(e.Message);
            //}
        }
        else
        {
            Debug.Log("Invalid path given");
        }
        AppMgr.DrawOn = true;
        AppMgr.KeyOn = true;
        AppMgr.FileDialogOn = false;
    }

    public static void LoadImageFileUsingPath(string path)
    {
        //画像を読み込む
        AppMgr.ImageBytes = File.ReadAllBytes(path);
        //for (int i=0; i<10; i++)
        //{
        //    Debug.Log(bytes[i]);
        //}
        //Debug.Log(bytes.Length);
        //AppMgr.BackgroundTexture = new Texture2D(2, 2);
        //AppMgr.BackgroundTexture.LoadImage(bytes);
        //Debug.Log(AppMgr.BackgroundTexture);
        //Mat mat = OpenCvSharp.Unity.TextureToMat(texture);

        //Mat gray = new Mat();
        //Cv2.CvtColor(mat, gray, ColorConversionCodes.BGR2GRAY);


        // 画像書き出し
        //Texture2D outTexture = new Texture2D(mat.Width, mat.Height, TextureFormat.ARGB32, false);
        //OpenCvSharp.Unity.MatToTexture(mat, outTexture);
        //AppMgr.BackgroundTexture = outTexture;

        // 表示
        //GameObject image = FindObjectOfType<RawImage>();
        //image.GetComponent<RawImage>().texture = outTexture;        
        //テクスチャへ変換する
        //背景に貼り付ける
        //OpenCVで画像を開く
        //二値化などの前処理を行う
        //ハフ変換で直線を検出する。
        //直線検出から頂点を検出する。
        //ハフ変換で円を検出する。
    }

    #endregion

    #region TeX保存

    private static void SaveTeXFileUsingPath(string path)
    {
        Debug.Log("path=" + path);
        Debug.Log("logs=" + logs);
        Debug.Log("cis=" + AppMgr.cis);
        Debug.Log("pts=" + AppMgr.pts);
        if (logs == null || AppMgr.pts == null) return;
        // Make sure path is not null or empty
        if (!String.IsNullOrEmpty(path))
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path, false))
                {
                    writer.WriteLine("\\documentclass[10pt,dvipdfmx]{article}");
                    writer.WriteLine("\\usepackage{pgf,tikz}");
                    writer.WriteLine("\\usepackage{mathrsfs}");
                    writer.WriteLine("\\pagestyle{empty}");
                    writer.WriteLine("\\begin{document}");
                    writer.WriteLine("\\begin{tikzpicture}[line cap=round,line join=round,x=0.5cm,y=0.5cm]");
                    writer.WriteLine("\\clip(" + AppMgr.LeftBottom.x + "," + AppMgr.LeftBottom.y + ") rectangle (" + AppMgr.RightUp.x + "," + AppMgr.RightUp.y + ");");
                    writer.Flush();
                    for (int i = 0; i < LogLength; i++)
                    {
                        Log l = logs[i];
                        //Debug.Log(""+ l.ObjectType);
                        if (l.Active == false)
                        {
                            ;
                        }
                        else if (l.ObjectType == "Circle")
                        {
                            if (AppMgr.cis != null)
                            {
                                // l.Radiusの最新情報を取得する
                                for (int k = 0; k < AppMgr.cis.Length; k++)
                                {
                                    if (AppMgr.cis[k].Id == l.Id)
                                    {
                                        l.Radius = AppMgr.cis[k].Radius;
                                        break;
                                    }
                                }

                                for (int j = 0; j < AppMgr.pts.Length; j++)
                                {
                                    if (AppMgr.pts[j].Id == l.Object1Id)
                                    {
                                        writer.Write("\\draw(" + AppMgr.pts[j].Vec.x + "," + AppMgr.pts[j].Vec.y + ")");
                                        writer.WriteLine(" circle (" + l.Radius + ");");
                                        writer.Flush();
                                        break;
                                    }
                                }
                            }
                        }
                        else if (l.ObjectType == "Line")
                        {
                            int j1 = -1, j2 = -1;
                            for (int j = 0; j < AppMgr.pts.Length; j++)
                            {
                                if (AppMgr.pts[j].Id == l.Object1Id)
                                    j1 = j;
                                if (AppMgr.pts[j].Id == l.Object2Id)
                                    j2 = j;
                            }
                            writer.WriteLine("\\draw (" + AppMgr.pts[j1].Vec.x + "," + AppMgr.pts[j1].Vec.y + ")-- (" + AppMgr.pts[j2].Vec.x + "," + AppMgr.pts[j2].Vec.y + ");");
                            writer.Flush();
                        }
                        else if (l.ObjectType == "Point")
                        {
                            int id = l.Id;
                            //FindPointFromId();
                            Point pt = null;
                            for (int k = 0; k < AppMgr.pts.Length; k++)
                            {
                                if (AppMgr.pts[k].Id == id)
                                {
                                    pt = AppMgr.pts[k];
                                }
                            }
                            writer.WriteLine("\\draw[fill=black](" + pt.Vec.x + "," + pt.Vec.y + ") circle  (1.5pt);");
                            if (pt.PTobject != null)                            // 文字を添えるかどうか
                            {
                                if (pt.ShowPointName)
                                {// 文字を表示するかどうかのフラグ。
                                    Vector3 textPos = pt.PTobject.transform.position;
                                    writer.WriteLine("\\draw[fill=black](" + textPos.x + "," + textPos.y + ") node  {" + pt.PointName + "};");
                                }
                            }
                            writer.Flush();
                        }
                    }
                    writer.WriteLine("\\end{tikzpicture}");
                    writer.WriteLine("\\end{document}");
                    writer.Flush();
                    writer.Close();
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
        Debug.Log("save TeX file : over");
        AppMgr.DrawOn = true;
        AppMgr.KeyOn = true;
        AppMgr.FileDialogOn = false;
    }

    public static bool SaveTeXFileSelectFile()
    {
        //Prefab = Resources.Load<GameObject>("Prefabs/FileBrowser");
        //GameObject g = FileBrowser.Instantiate<GameObject>(Prefab, Vector3.zero, Quaternion.identity);
        //FileBrowser fb = g.GetComponent<FileBrowser>();
        //string[] ext = new string[] { "tex" };
        //fb.SetupFileBrowser(PortraitMode ? ViewMode.Portrait : ViewMode.Landscape);
        //fb.SaveFilePanel("PointLine", ext);
        //fb.OnFileSelect += SaveTeXFileUsingPath;
        //string path = Crosstales.FB.FileBrowser.SaveFile("Save a PointLine TeX file", "", "SamplePointLine.tex","tex");
        FileBrowser.SetFilters(true, new FileBrowser.Filter("TeX Files", ".tex"));
        FileBrowser.SetDefaultFilter(".tex");
        FileBrowser.ShowSaveDialog(onSaveTeXSuccess, onCancel, FileBrowser.PickMode.FilesAndFolders, false, "",
            "SamplePointLine.tex", "Save TeX File");
        return false;
    }
    static void onSaveTeXSuccess(string[] paths)
    {
        Debug.Log("" + paths[0]);
        SaveTeXFileUsingPath(paths[0]);
    }
    #endregion

    #region Log操作
    public static bool CopyLog(string srcPath, string tgtPath)
    {
        //ストリームリーダーreaderから読み込む
        try
        {
            StreamReader reader = new StreamReader(Application.dataPath + "/Resources/" + srcPath, false);
            StreamWriter writer = new StreamWriter(Application.dataPath + "/Resources/" + tgtPath, false);
            string str;
            do
            {
                str = reader.ReadLine();
                if (str == null) break;//多分要らない。
                else
                {
                    writer.WriteLine(str);
                }
            }
            while (str != null);
            writer.Flush();
            writer.Close();
            reader.Close();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return false;
        }
        return true;

    }

    public static bool OpenLog(string path)
    {
        //ストリームリーダーreaderから読み込む
        try
        {
            using (StreamReader reader = new StreamReader(Application.dataPath + "/Resources/" + path, false))
            {
                InitLog();
                string str;
                int PId = -1, LId = -1, CId = -1, MId = -1;
                do
                {
                    str = reader.ReadLine();
                    if (str == null) break;//多分要らない。
                    else
                    {
                        Log lg = GetLogFromString(str);
                        //AddLog(lg);
                        if (lg.ObjectType == "Point")
                        {
                            if (PId < lg.Id) PId = lg.Id;
                        }
                        else if (lg.ObjectType == "Line")
                        {
                            if (LId < lg.Id) LId = lg.Id;
                        }
                        else if (lg.ObjectType == "Circle")
                        {
                            if (CId < lg.Id) CId = lg.Id;
                        }
                        else if (lg.ObjectType == "Module")
                        {
                            if (MId < lg.Id) MId = lg.Id;
                        }
                    }
                }
                while (str != null);
                reader.Close();
                ClickOnPanel.SetId(PId + 1, LId + 1, CId + 1, MId + 1);
                AppMgr.pts = MonoBehaviour.FindObjectsOfType<Point>();
                AppMgr.lns = MonoBehaviour.FindObjectsOfType<Line>();
                AppMgr.cis = MonoBehaviour.FindObjectsOfType<Circle>();
                AppMgr.mds = MonoBehaviour.FindObjectsOfType<Module>();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return false;
        }
        return true;
    }

    public static bool SaveLogToTeX()
    {
        if (logs == null || AppMgr.cis == null || AppMgr.pts == null) return false;
        try
        {
            using (StreamWriter writer = new StreamWriter(Application.dataPath + "/../../PointLine.tex", false))
            {
                writer.WriteLine("\\documentclass[10pt,dvipdfmx]{article}");
                writer.WriteLine("\\usepackage{pgf,tikz}");
                writer.WriteLine("\\usepackage{mathrsfs}");
                writer.WriteLine("\\pagestyle{empty}");
                writer.WriteLine("\\begin{document}");
                writer.WriteLine("\\begin{tikzpicture}[line cap=round,line join=round,x=0.5cm,y=0.5cm]");
                writer.WriteLine("\\clip(" + AppMgr.LeftBottom.x + "," + AppMgr.LeftBottom.y + ") rectangle (" + AppMgr.RightUp.x + "," + AppMgr.RightUp.y + ");");
                writer.Flush();
                for (int i = 0; i < LogLength; i++)
                {
                    Log l = logs[i];
                    //Debug.Log(""+ l.ObjectType);
                    if (l.ObjectType == "Circle")
                    {
                        AppMgr.cis = MonoBehaviour.FindObjectsOfType<Circle>();
                        // l.Radiusの最新情報を取得する
                        for (int k = 0; k < AppMgr.cis.Length; k++)
                        {
                            if (AppMgr.cis[k].Id == l.Id)
                            {
                                l.Radius = AppMgr.cis[k].Radius;
                                break;
                            }
                        }
                        AppMgr.pts = MonoBehaviour.FindObjectsOfType<Point>();
                        for (int j = 0; j < AppMgr.pts.Length; j++)
                        {
                            if (AppMgr.pts[j].Id == l.Object1Id)
                            {
                                writer.Write("\\draw(" + AppMgr.pts[j].Vec.x + "," + AppMgr.pts[j].Vec.y + ")");
                                writer.WriteLine(" circle (" + l.Radius + ");");
                                writer.Flush();
                                break;
                            }
                        }

                    }
                    else if (l.ObjectType == "Line")
                    {
                        AppMgr.pts = MonoBehaviour.FindObjectsOfType<Point>();
                        int j1 = -1, j2 = -1;
                        for (int j = 0; j < AppMgr.pts.Length; j++)
                        {
                            if (AppMgr.pts[j].Id == l.Object1Id)
                                j1 = j;
                            if (AppMgr.pts[j].Id == l.Object2Id)
                                j2 = j;
                        }
                        writer.WriteLine("\\draw (" + AppMgr.pts[j1].Vec.x + "," + AppMgr.pts[j1].Vec.y + ")-- (" + AppMgr.pts[j2].Vec.x + "," + AppMgr.pts[j2].Vec.y + ");");
                        writer.Flush();
                    }
                    else if (l.ObjectType == "Point")
                    {
                        AppMgr.pts = MonoBehaviour.FindObjectsOfType<Point>();
                        int id = l.Id;
                        //FindPointFromId();
                        Point pt = null;
                        for (int k = 0; k < AppMgr.pts.Length; k++)
                        {
                            if (AppMgr.pts[k].Id == id)
                            {
                                pt = AppMgr.pts[k];
                            }
                        }
                        writer.WriteLine("\\draw[fill=black](" + pt.Vec.x + "," + pt.Vec.y + ") circle  (1.5pt);");
                        writer.Flush();
                        // 文字を添えるかどうか
                    }
                }
                writer.WriteLine("\\end{tikzpicture}");
                writer.WriteLine("\\end{document}");
                writer.Flush();
                writer.Close();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return false;
        }
        return true;

    }


    public static void DebugLog()
    {
        for (int i = 0; i < LogLength; i++)
        {
            Debug.Log(logs[i].ToString());
        }
    }

    public static void SetAllLogActive()
    {
        if (logs == null) return;
        for (int i = 0; i < LogLength; i++)
        {
            logs[i].parent.SetActive(true);
        }
    }
    #endregion

    #region Undo
    //undoするとき、点の位置を動かす。//0.770
    //private static void PerturbPoints(float delta)
    //{
    //    Vector3 dVec = new Vector3(0f, 0f, 0f);
    //    for (int i = 0; i < AppMgr.pts.Length; i++)
    //    {
    //        float th = UnityEngine.Random.Range(0.0f, 2 * Mathf.PI);
    //        dVec.x = delta * Mathf.Cos(th);
    //        dVec.y = delta * Mathf.Sin(th);
    //        AppMgr.pts[i].Vec += dVec;
    //    }
    //}
    public static void Undo()
    {
        if (LastLog > 0)
        {
            LastLog--;
            logs[LastLog].Active = false;
            logs[LastLog].parent.SetActive(false);
            //undoするとき、点の位置を動かす。//0.770
            //if (logs[LastLog].ObjectType == "Module")//
            //{
            //    PerturbPoints(0.4f);
            //}
            if (logs[LastLog].ObjectType == "Point")
            {
                logs[LastLog].parent.GetComponent<Point>().PTobject.SetActive(false);
            }
            if (logs[LastLog].ObjectType == "Module" && logs[LastLog].ModuleType == MENU.LINES_PERPENDICULAR)
            {
                AngleMark[] am = MonoBehaviour.FindObjectsOfType<AngleMark>();
                if (am != null)
                {
                    for (int i = 0; i < am.Length; i++)
                    {
                        if (am[i].Object1Id == logs[LastLog].Object1Id && am[i].Object2Id == logs[LastLog].Object2Id)
                        {
                            am[i].parent.SetActive(false);
                            logs[LastLog].child = am[i].parent;
                        }
                    }
                }
            }
            if (logs[LastLog].ObjectType == "Module" && logs[LastLog].ModuleType == MENU.POINT_ON_LINE)
            {
                ThinLine[] TL = MonoBehaviour.FindObjectsOfType<ThinLine>();
                if (TL != null)
                {
                    for (int i = 0; i < TL.Length; i++)
                    {
                        if (TL[i].PointId == logs[LastLog].Object1Id && TL[i].LineId == logs[LastLog].Object2Id)
                        {
                            TL[i].parent.SetActive(false);
                            logs[LastLog].child = TL[i].parent;
                        }
                    }
                }
            }
            Util.SetIsometry();
        }
    }
    #endregion

    #region Redo
    public static void Redo()
    {
        if (logs == null) return;
        if (LastLog < LogLength)
        {
            logs[LastLog].parent.SetActive(true);
            logs[LastLog].Active = true;
            if (logs[LastLog].ObjectType == "Point")
            {
                logs[LastLog].parent.GetComponent<Point>().PTobject.SetActive(true);
            }
            if (logs[LastLog].ObjectType == "Module" && logs[LastLog].ModuleType == MENU.LINES_PERPENDICULAR)
            {
                if (logs[LastLog].child != null)
                {
                    logs[LastLog].child.SetActive(true);
                }
            }
            if (logs[LastLog].ObjectType == "Module" && logs[LastLog].ModuleType == MENU.POINT_ON_LINE)
            {
                if (logs[LastLog].child != null)
                {
                    logs[LastLog].child.SetActive(true);
                }
            }
            Util.SetIsometry();
            LastLog++;
        }
    }
    #endregion

    public static void SetGameLogPosition()
    {
        for (int i = 0; i < logs.Count; i++)
        {
            logs[i].Position.y = StartTop - i;
        }
    }

    #region Isometry
    public static void SetIsometry()
    {
        Line[] lns = MonoBehaviour.FindObjectsOfType<Line>();
        GameObject[] objs = MonoBehaviour.FindObjectsOfType<GameObject>();
        for (int j = 0; j < lns.Length; j++)
        {
            lns[j].Isometry = -1;
        }
        int count = 0;
        for (int i = objs.Length - 1; i >= 0; i--)// ナゾの仕様だが仕方ない。
        {
            Module md = objs[i].GetComponent<Module>();
            if (md != null && md.Type == MENU.LINES_ISOMETRY)
            {
                //Debug.Log("line isometry exists"+i+":"+md);
                Line LinkLine1 = null, LinkLine2 = null;
                for (int j = 0; j < lns.Length; j++)
                {
                    Line ln = lns[j];
                    if (ln.Id == md.Object1Id)
                    {
                        LinkLine1 = ln;
                    }
                    else if (ln.Id == md.Object2Id)
                    {
                        LinkLine2 = ln;
                    }
                }
                if (LinkLine1.Isometry == -1 && LinkLine2.Isometry == -1)
                {//新しい等長の組
                    LinkLine1.Isometry = count;
                    LinkLine2.Isometry = count;
                    count++;
                }
                else if (LinkLine1.Isometry == -1 && LinkLine2.Isometry != -1)
                {
                    LinkLine1.Isometry = LinkLine2.Isometry;
                }
                else if (LinkLine1.Isometry != -1 && LinkLine2.Isometry == -1)
                {
                    LinkLine2.Isometry = LinkLine1.Isometry;
                }
                else if (LinkLine1.Isometry != -1 && LinkLine2.Isometry != -1)
                {
                    if (LinkLine1.Isometry < LinkLine2.Isometry)
                    {
                        for (int j = 0; j < lns.Length; j++)
                        {
                            if (lns[j].Isometry == LinkLine2.Isometry)
                            {
                                lns[i].Isometry = LinkLine1.Isometry;
                            }
                        }
                    }
                    else
                    {
                        for (int j = 0; j < lns.Length; j++)
                        {
                            if (lns[j].Isometry == LinkLine1.Isometry)
                            {
                                lns[j].Isometry = LinkLine2.Isometry;
                            }
                        }
                    }
                }
            }
        }
    }
    #endregion


    public static void StirPoints()
    {
        AppMgr.pts = MonoBehaviour.FindObjectsOfType<Point>();
        int ptsLen = AppMgr.pts.Length;
        for (int p=0; p<ptsLen; p++)
        {
            float x = UnityEngine.Random.Range(-3f, 3f);
            float y = UnityEngine.Random.Range(-3f, 3f);
            AppMgr.pts[p].Vec = new Vector3(x, y, 0f);
        }
        AppMgr.ExecuteAllModules();
    }
}