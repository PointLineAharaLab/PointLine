using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

//using GracesGames.Common.Scripts;
//using GracesGames.SimpleFileBrowser.Scripts;
//using GracesGames.SimpleFileBrowser.Scripts.UI;
using SimpleFileBrowser;



/// 様々なユーティリティ. <summary>
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
    public static bool ShowLog = false;

    public static Color[] IsometryColor;
    public static Color[] IsometrySelectedColor;

    /// <summary>
    /// 画面固定のためのフラグ
    /// </summary>
    public static bool FixDisplay = false;


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
    /// パーティクルの生成
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

    /// パーティクルの生成
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


    /// パーティクルの生成
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

    public static string GetModuleNameByType(int type)
    {
        switch (type)
        {
            case MENU.POINT_ON_POINT:
                return "点 - 点";
            case MENU.POINT_ON_LINE:
                return "点 - 直線";
            case MENU.POINT_ON_CIRCLE:
                return "点 - 円";
            case MENU.LINES_ISOMETRY:
                return "等長";//"辺長比"
            case MENU.LINES_PERPENDICULAR:
                return "垂直";//"一定角"
            case MENU.LINES_PARALLEL:
                return "平行";
            case MENU.CIRCLE_TANGENT_LINE:
                return "円 - 直線";
            case MENU.CIRCLE_TANGENT_CIRCLE:
                return "円 - 円";
            case MENU.ADD_MIDPOINT:
                return "中点";//"内分点"
            case MENU.ANGLE:
                return "角度";
            case MENU.BISECTOR:
                return "角度 - 角度";
            case MENU.ADD_LOCUS:
                return "軌跡";
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
        }
        return obj;
    }

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


    // Unity Action Event for selecting a file
    //public event Action<string> OnFileSelect = delegate { };

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
            else
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
        //SaveFileUsingPath(path);
        FileBrowser.SetFilters(
            true,
            new FileBrowser.Filter("Images", ".jpg", ".png"),
            new FileBrowser.Filter("Text Files", ".txt")
        );
        // ダイアログが表示されたときに選択されるデフォルトフィルタを設定します
        FileBrowser.SetDefaultFilter(".txt");
        // /除外する拡張子を設定します
        FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");
        // 新しいクイックリンクを追加します
        //FileBrowser.AddQuickLink(null, "Users", "C:\\Users");
        // ファイル保存ダイアログを表示します
        StartCoroutine(ShowSaveDialogCoroutine());
        //FileBrowser.ShowSaveDialog(null, null, FileBrowser.PickMode.FilesAndFolders, false, "", "SamplePointLine.txt", "Save");
        //Debug.Log("result="+FileBrowser.Result);
        return false;
    }

    public IEnumerator ShowSaveDialogCoroutine()
    {
        yield return FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.FilesAndFolders, false, null, "Sample.txt", "Save", "Save");
        Debug.Log(FileBrowser.Success + " " + FileBrowser.Result);
    }


    #endregion

    #region ファイルを開く

    private static void LoadFileUsingPath(string path)
    {
        ClickOnPanel.DeleteAll();
        if (path!=null && path.Length != 0)
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
                Debug.Log(e.Message);
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
        //LoadFileUsingPath(path);
        // フィルタを設定します
        FileBrowser.SetFilters( true,
            new FileBrowser.Filter("Images", ".jpg", ".png"),
            new FileBrowser.Filter("Text Files", ".txt")        );
        // ダイアログが表示されたときに選択されるデフォルトフィルタを設定します
        FileBrowser.SetDefaultFilter(".jpg");
        // /除外する拡張子を設定します
        FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");
        // 新しいクイックリンクを追加します
        //FileBrowser.AddQuickLink(null, "Users", "C:\\Users");
        // フォルダ選択ダイアログを表示します
        //FileBrowser.ShowLoadDialog
        //(
        //    path => Debug.Log("Selected: " + path),
        //    () => Debug.Log("Canceled"),
        //    true,
        //    null,
        //    "Select Folder",
        //    "Select"
        //);
        // コルーチンサンプル
        //StartCoroutine(ShowLoadDialogCoroutine());
        return false;
    }

    //private bool IEnumerator ShowLoadDialogCoroutine()
    //{
    //    // ファイル読み込みダイアログを表示してユーザーからの応答を待ちます
    //    yield return FileBrowser.WaitForLoadDialog(false, null, "Load File", "Load");
    //    Debug.Log(FileBrowser.Success + " " + FileBrowser.Result);
    //    return false;
    //}


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
            if (item.Length == 7)
            {
                char[] name = { 'A' };
                name[0] = (char)('A' + id);
                pname = new string(name);
            }
            else
            {
                pname = item[7];
            }
            pt.PTobject.GetComponent<TextMesh>().text = pname;
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
            int id = int.Parse(item[5]);
            bool act = bool.Parse(item[6]);
            float ra1 = 1f, ra2 = 1f, cst = Mathf.PI / 2;
            if (item.Length >= 10)
            {
                ra1 = float.Parse(item[7]);
                ra2 = float.Parse(item[8]);
                cst = float.Parse(item[9]);
            }
            bool showC = true, fixA = false, fixR = false;
            if (item.Length >= 13)
            {
                showC = (item[10] == "True");
                fixA = (item[11] == "True");
                fixR = (item[12] == "True");
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
            //ログ作成
            Log lg = MD.GameLog.GetComponent<Log>();
            lg.MakeModuleLog(id, mt, o1, o2, o3, MD.parent, act, MD.ModuleName);
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
                            if (AppMgr.cis != null) { 
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
        //SaveTeXFileUsingPath(path);

        return false;
    }

    #endregion

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
                        if(lg.ObjectType == "Point")
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
                while (str!=null);
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
        if (logs == null || AppMgr.cis == null || AppMgr.pts == null ) return false;
        try
        {
            using (StreamWriter writer = new StreamWriter(Application.dataPath + "/../../PointLine.tex" , false))
            {
                writer.WriteLine("\\documentclass[10pt,dvipdfmx]{article}");
                writer.WriteLine("\\usepackage{pgf,tikz}");
                writer.WriteLine("\\usepackage{mathrsfs}");
                writer.WriteLine("\\pagestyle{empty}");
                writer.WriteLine("\\begin{document}");
                writer.WriteLine("\\begin{tikzpicture}[line cap=round,line join=round,x=0.5cm,y=0.5cm]");
                writer.WriteLine("\\clip("+AppMgr.LeftBottom.x+","+AppMgr.LeftBottom.y+") rectangle ("+AppMgr.RightUp.x+","+AppMgr.RightUp.y+");");
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
                            if(AppMgr.cis[k].Id == l.Id)
                            {
                                l.Radius = AppMgr.cis[k].Radius;
                                break;
                            }
                        }
                        AppMgr.pts = MonoBehaviour.FindObjectsOfType<Point>();
                        for (int j=0; j<AppMgr.pts.Length; j++)
                        {
                            if(AppMgr.pts[j].Id == l.Object1Id)
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
                        writer.WriteLine("\\draw ("+AppMgr.pts[j1].Vec.x+","+ AppMgr.pts[j1].Vec.y + ")-- ("+ AppMgr.pts[j2].Vec.x + ","+ AppMgr.pts[j2].Vec.y + ");");
                        writer.Flush();
                    }
                    else if (l.ObjectType == "Point")
                    {
                        AppMgr.pts = MonoBehaviour.FindObjectsOfType<Point>();
                        int id = l.Id;
                        //FindPointFromId();
                        Point pt = null;
                        for (int k=0; k<AppMgr.pts.Length; k++)
                        {
                            if (AppMgr.pts[k].Id == id)
                            {
                                pt = AppMgr.pts[k]; 
                            }
                        }
                        writer.WriteLine("\\draw[fill=black]("+ pt.Vec.x + ","+pt.Vec.y+") circle  (1.5pt);");
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
        for (int i = 0; i<LogLength; i++)
        {
            Debug.Log(logs[i].ToString());
        }
    }

    public static void SetAllLogActive()
    {
        if (logs == null) return;
        for(int i=0; i<LogLength; i++)
        {
            logs[i].parent.SetActive(true);
        }
    }

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

    public static void Redo()
    {
        if (logs == null) return;
        if (LastLog <LogLength)
        {
            logs[LastLog].parent.SetActive(true);
            logs[LastLog].Active = true;
            if (logs[LastLog].ObjectType == "Point")
            {
                logs[LastLog].parent.GetComponent<Point>().PTobject.SetActive(true);
            }
            if (logs[LastLog].ObjectType == "Module" && logs[LastLog].ModuleType == MENU.LINES_PERPENDICULAR)
            {
                if(logs[LastLog].child != null)
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

    public static void SetGameLogPosition()
    {
        for(int i=0; i<logs.Count; i++)
        {
            logs[i].Position.y = StartTop - i;
        }
    }

    public static void SetIsometry() {
        Line[] lns = MonoBehaviour.FindObjectsOfType<Line>();
        GameObject[] objs = MonoBehaviour.FindObjectsOfType<GameObject>();
        for (int j = 0; j < lns.Length; j++)
        {
            lns[j].Isometry = -1;
        }
        int count = 0;
        for (int i = objs.Length-1; i >=0 ; i--)// ナゾの仕様だが仕方ない。
        {
            Module md = objs[i].GetComponent<Module>();
            if (md!=null && md.Type == MENU.LINES_ISOMETRY)
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
}