using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickOnPanel : AppMgr //MonoBehaviour
{
    static int PointId=0;
    static int LineId=1000;
    static int CircleId=2000;
    static int ModuleId=3000;

    public int DraggedObjectId = 0;
    public int DraggedPointId = -1;
    private Vector3 MouseDownVec=Vector3.zero;
    private Vector3 MouseUpVec = Vector3.zero;

    private int FirstClickId=-1;
    private Vector3 FirstClickVec = Vector3.zero;
    private int SecondClickId=-1;
    private int ThirdClickId = -1;

    string FirstKey="";

    public GameObject MainCamera;
    public GameObject PreferenceDialog;

    public static float WorldHeight;

    public GameObject Selector;

    // OnMouseDrag
    float DraggedGameLogStartTop = 0f;
    Vector3 DraggedPreferencePosition;
    Vector3 PreviousMouseDragPosition;

    // Use this for initialization
    void Start()
    {
        SetId(0, 1000, 2000, 3000);
        FirstKey = "";
        Util.InitLog();
        WorldHeight = MainCamera.GetComponent<Camera>().orthographicSize;
        //Debug.Log(WorldHeight);
        Util.LogLeft = (WorldHeight / Screen.height * Screen.width)-1.5f;
        Util.IsometryColor = new Color[10];
        Util.IsometrySelectedColor = new Color[10];
        for(int i=0; i<10; i++)
        {
            float vx = ColorCode[3 * i];
            float vy = ColorCode[3 * i + 1];
            float vz = ColorCode[3 * i + 2];
            Util.IsometryColor[i] = new Color((vx + 1f) * 0.5f, (vy + 1f) * 0.5f, (vz + 1f) * 0.5f);
            Util.IsometrySelectedColor[i] = new Color(vx, vy, vz);
        }
    }

    private readonly float[] ColorCode = new float[]{ 
        0.7f, 0.7f, 0.0f,
        0.0f, 0.7f, 0.7f, 
        0.7f, 0.0f, 0.7f,
        0.8f, 0.4f, 0.0f,
        0.0f, 0.8f, 0.4f,
        0.0f, 0.0f, 0.8f,
        0.4f, 0.0f, 0.0f,
        0.4f, 0.8f, 0.0f,
        0.0f, 0.4f, 0.8f,
        0.8f, 0.0f, 0.4f,
    };

    void Update()
    {
        if (AppMgr.KeyOn)
        {
            OnKey();
        }
        Util.SetGameLogPosition();
        if (Input.GetMouseButtonDown(0)){
            OnMyMouseDown();
        }
        else if (Input.GetMouseButton(0))
        {
            OnMouseDrag();
        }
        OnKeyCommand();
        //else if (Input.GetMouseButtonUp(0))
        //{
        //    print("mousebuttonup 0");
        //}
        Util.SetIsometry();
    }

    public static void SetId(int PId, int LId, int CId, int MId)
    {
        PointId = PId;
        LineId = LId;
        CircleId = CId;
        ModuleId = MId;
    }

    private float Hypot(float x, float y)
    {
        return Mathf.Sqrt(x * x + y * y);
    }

    private List<int> MouseOnPoints(Vector3 v)
    {
        List<int> ret = new List<int>();
        if (pts == null) return ret;
        for (int i = 0; i < pts.Length; i++)
        {
            double dist = Hypot(v.x - pts[i].Vec.x, v.y - pts[i].Vec.y);
            //Debug.Log("dist - " + dist);
            if (dist < 0.25)
            {
                ret.Add(pts[i].Id);
            }
        }
        return ret;
    }

    private int MouseOnLines(Vector3 v)
    {
        if (lns == null) return -1;
        for (int i = 0; i < lns.Length; i++)
        {
            if (lns[i].GetDistance(v))
            {
                return lns[i].Id;
            }
        }
        return -1;
    }

    private int MouseOnCircle(Vector3 v)
    {// 
        if (cis == null) return -1;
        Circle[] ci = FindObjectsOfType<Circle>();
        if (ci != null)
        {
            for (int i = 0; i < ci.Length; i++)
            {
                float dist = Hypot(ci[i].CenterVec.x - v.x, ci[i].CenterVec.y - v.y);
                if (ci[i].Radius - 0.25 < dist && dist < ci[i].Radius + 0.25)
                {
                    //Debug.Log("MouseOnCircle " + ci[i].Id);
                    return ci[i].Id;
                }
            }
        }
        return -1;
    }

    private int MouseOnAngle(Vector3 v)
    {
        AngleMark[] am = FindObjectsOfType<AngleMark>();
        if (am != null)
        {
            for (int i = 0; i < am.Length; i++)
            {
                float dist = (am[i].Origine - v).magnitude;
                //float dist = Hypot(am[i].Origine.x - v.x, am[i].Origine.y - v.y);
                float DeclineX = Mathf.Atan2(am[i].UnitX.y, am[i].UnitX.x);// UnitXの偏角
                float DeclineY = Mathf.Atan2(am[i].UnitY.y, am[i].UnitY.x);// UnitYの偏角
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
                float DeclineV = Mathf.Atan2(v.y - am[i].Origine.y, v.x - am[i].Origine.x);
                //Debug.Log("X:"+ DeclineX + "Y:" + DeclineY + "V:" + DeclineV);
                if((DeclineX - DeclineV)*(DeclineY - DeclineV) <= 0)
                {
                    if (0.25 < dist && dist < 1)
                    {
                        //Debug.Log("MouseOnAngle " + am[i].parent.GetComponent<Module>().Id);
                        return am[i].parent.GetComponent<Module>().Id;
                    }
                }
                
            }
        }
        return -1;
    }


    int MouseOnGameLog(Vector3 v)
    {
        if (Util.logs == null) return -1;
        for(int i=0; i<Util.logs.Count; i++)
        {
            Log lg  = Util.logs[i];
            if (lg.Show) {
                //if (lg.Position.x + 1.0f < v.x && v.x < lg.Position.x + 1.5f
                //    && lg.Position.y < v.y && v.y < lg.Position.y + 0.5)
                //{
                //    return i + 4500;
                //}
                if (lg.Position.x - 1.5f < v.x && v.x < lg.Position.x + 1.5f
                    && lg.Position.y - 0.5 < v.y && v.y < lg.Position.y + 0.5)
                {
                    return i + 4000;
                }
            }
        }
        return -1;
    }

    bool MouseOnPreference(Vector3 v)
    {
        Preference lg = PreferenceDialog.GetComponent<Preference>();
        //print(lg.Position);
        //print(v);
        if (lg.Position.x - 1.5f < v.x && v.x < lg.Position.x + 1.5f
            && lg.Position.y - 2.25f < v.y && v.y < lg.Position.y + 2.25f)
        {
            if (lg.show)
            {
                return true;
            }
        }
        return false;
    }
    private int MousePointPosition()
    {
        Vector3 v = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        v.z = 0.0f;
        List<int> mop = MouseOnPoints(v);// ポイントをクリックしたかどうかのチェック
        if (mop.Count>0)
        {
            return mop[0];
        }
        else
        {
            return -1;
        }
    }
    private int getObjectFromMousePosition(bool getSelector)
    {
        Vector3 v = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        v.z = 0.0f;
        bool MOPre = MouseOnPreference(v);// マウスがプリファレンスの上にあるかどうかのチェック
        if (MOPre)
        {
            return 5000;// プリファレンスのコード
        }
        int MOL = MouseOnGameLog(v);// マウスがログの上にあるかどうかのチェック
        if(MOL != -1)
        {
            //Debug.Log("MOL = "+MOL);
            return MOL;//ログの上にある時が優先
        }
        int MOP=-1;
        // 状況ごとに、優先されるべき要素に対応する。
        //Debug.Log("ClickRequire = " + AppMgr.ClickRequire);
        if (AppMgr.ClickRequire == AppMgr.CLICKREQ_POINT)
        {
            List<int> mop = MouseOnPoints(v);// ポイントをクリックしたかどうかのチェック
            if (mop.Count > 0)
            {
                if (getSelector)
                {
                    for (int m = 0; m < mop.Count; m++)
                    {
                        MOP = mop[m];
                        GameObject prefab = Resources.Load<GameObject>("Prefabs/SelectorDialog");
                        GameObject g_object = Line.Instantiate(prefab, v - new Vector3(0f, 0.5f * m, 0f), Quaternion.identity, Selector.transform) as GameObject;
                        SelectorDialog sd_obj = g_object.GetComponent<SelectorDialog>();
                        for (int p = 0; p < AppMgr.pts.Length; p++)
                        {
                            if (AppMgr.pts[p].Id == MOP)
                            {
                                sd_obj.Text1 = "点 " + AppMgr.pts[p].PointName;
                            }
                        }
                    }
                    AppMgr.SelectorOn = true;
                    AppMgr.DrawOn = true;
                }
                else MOP = mop[0];
            }
            else MOP = -1;
        }
        else if (AppMgr.ClickRequire == AppMgr.CLICKREQ_LINE)
        {
            MOP = MouseOnLines(v);// ラインをクリックしたかどうかのチェック
        }
        else if (AppMgr.ClickRequire == AppMgr.CLICKREQ_CIRCLE)
        {
            MOP = MouseOnCircle(v);//サークルをクリックしたかどうかのチェック
        }
        else if (AppMgr.ClickRequire == AppMgr.CLICKREQ_ANGLE)
        {
            MOP = MouseOnAngle(v);//角度をクリックしたかどうかのチェック
        }
        if (MOP == -1)
        {
            List<int> mop = new List<int>();
            mop = MouseOnPoints(v);// ポイントをクリックしたかどうかのチェック
            if (mop.Count > 0) MOP = mop[0]; else MOP = -1;
            if (MOP == -1)
            {
                MOP = MouseOnLines(v);// ラインをクリックしたかどうかのチェック
                if(MOP == -1)
                {
                    MOP = MouseOnCircle(v);//サークルをクリックしたかどうかのチェック
                    if (MOP == -1)
                    {
                        MOP = MouseOnAngle(v);//角度をクリックしたかどうかのチェック
                    }
                }
            }
        }
        if (MOP >= 0 && MOP < 4000)
        {// MOP番のオブジェクト
            Debug.Log("MOP = " + MOP);
            return MOP;
        }
        else if (ClickOnButton(Input.mousePosition))
        {
            Debug.Log("Click a menu icon");
            return -2;//メニューを押した。
        }
        else
        {
            return -1;// 何もないところ
        }
    }

    void OnKey()
    {
        if (FirstKey == "")
        {
            OnKeyFirst();
        }
        else if (FirstKey == "A")
        {
            OnKeyAdd();
        }
        else if (FirstKey == "P")
        {
            OnKeyPoint();
        }
        else if (FirstKey == "L")
        {
            OnKeyLine();
        }
        else if (FirstKey == "T")
        {
            OnKeyTangent();
        }
        else if (FirstKey == "F")
        {
            OnKeyFix();
        }
        else if (FirstKey == "D")
        {
            OnKeyDelete();
        }
    }

    void OnKeyFirst() {
        if (Input.GetKeyDown(KeyCode.A))
        {
            FirstKey = "A";
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            FirstKey = "P";
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            FirstKey = "L";
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            FirstKey = "T";
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            FirstKey = "F";
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            FirstKey = "D";
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("undo (key)");
            Mode = MENU.UNDO;
            Util.Undo();
            Mode = 0;
            ModeStep = 0;
            MenuOn = false;
            ModuleOn = true;
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            Debug.Log("redo (key)");
            Mode = MENU.REDO;
            Util.Redo();
            Mode = 0;
            ModeStep = 0;
            MenuOn = false;
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("Show/Hide logs (key)");

            Mode = MENU.ADD_POINT;
            Mode = 0;
            ModeStep = 0;
            MenuOn = false;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("save (key)");

            Mode = MENU.SAVE;
            FileDialogOn = true;
            Util.RemakeLog();
            Util.SaveLog("TmpLog.txt");
            Util.CopyLog("TmpLog.txt", "TmpSaveFile.txt");
            //モードを非描画モードにする。
            DrawOn = false;
            //保存ダイアログの描画＋ファイル保存
            Util.SaveLogSelectFile();
            // モードを通常に戻す。
            Mode = 0;
            ModeStep = 0;
            MenuOn = false;
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("open (key)");

            Mode = MENU.OPEN;
            FileDialogOn = true;
            //モードを非描画モードにする。
            DrawOn = false;
            Util.OpenLogSelectFile();
            //SceneManager.LoadScene("OpenDialog");
            Mode = 0;
            ModeStep = 0;
            MenuOn = false;
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("quit (key)");
            Mode = MENU.QUIT;
            ModeStep = 0;
            MenuOn = false;
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
		    Application.OpenURL("http://aharalab.sakura.ne.jp/");
#else
		    Application.Quit();
#endif
            return;
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            Util.DebugLog();
        }
        else if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (PreferenceDialog.GetComponent<Preference>().show) { 
                PreferenceDialog.GetComponent<Preference>().EnterKeyDownProc();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PreferenceDialog.GetComponent<Preference>().show)
            {
                PreferenceDialog.GetComponent<Preference>().show = false;
            }
        }
    }

    void OnKeyAdd()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("add a point (key)");
            Mode = MENU.ADD_POINT;
            ModeStep = 0;
            MenuOn = false;
            FirstKey = "";
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("add a midpoint (key)");
            Mode = MENU.ADD_MIDPOINT;
            ModeStep = 0;
            MenuOn = false;
            FirstKey = "";
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("add a line (key)");
            Mode = MENU.ADD_LINE;
            ModeStep = 0;
            MenuOn = false;
            FirstKey = "";
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("add a circle (key)");
            Mode = MENU.ADD_CIRCLE;
            ModeStep = 0;
            MenuOn = false;
            FirstKey = "";
        }
    }

    void OnKeyCommand()
    {
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetAxis("Mouse ScrollWheel")>0)
        {//拡大
            if (!Util.FixDisplay)
            {
                Vector3 MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                pts = FindObjectsOfType<Point>();
                for (int i = 0; i < pts.Length; i++)
                {
                    Point pt = pts[i];
                    Vector3 NewVec = 1.05f * (pt.Vec - MousePosition) + MousePosition;
                    pt.Vec = NewVec;
                }
            }
            
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetAxis("Mouse ScrollWheel") < 0)
        {//縮小 
            if (!Util.FixDisplay)
            {
                Vector3 MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                pts = FindObjectsOfType<Point>();
                for (int i = 0; i < pts.Length; i++)
                {
                    Point pt = pts[i];
                    Vector3 NewVec = 0.96f * (pt.Vec - MousePosition) + MousePosition;
                    pt.Vec = NewVec;
                }
            }
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {//右へ回す
            if (!Util.FixDisplay)
            {
                Vector3 MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                pts = FindObjectsOfType<Point>();
                float angle = -0.025f;
                for (int i = 0; i < pts.Length; i++)
                {
                    Point pt = pts[i];
                    float x = pt.Vec.x - MousePosition.x;
                    float y = pt.Vec.y - MousePosition.y;
                    Vector3 NewVec = new Vector3(Mathf.Cos(angle) * x - Mathf.Sin(angle) * y, Mathf.Sin(angle) * x + Mathf.Cos(angle) * y);
                    pt.Vec = NewVec + MousePosition;
                }
            }
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {//左へ回す
            if (!Util.FixDisplay)
            {
                Vector3 MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                pts = FindObjectsOfType<Point>();
                float angle = 0.025f;
                for (int i = 0; i < pts.Length; i++)
                {
                    Point pt = pts[i];
                    float x = pt.Vec.x - MousePosition.x;
                    float y = pt.Vec.y - MousePosition.y;
                    Vector3 NewVec = new Vector3(Mathf.Cos(angle) * x - Mathf.Sin(angle) * y, Mathf.Sin(angle) * x + Mathf.Cos(angle) * y);
                    pt.Vec = NewVec + MousePosition;
                }
            }
        }

    }
    void OnKeyPoint()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Set a point on a point(key)");
            Mode = MENU.POINT_ON_POINT;
            ModeStep = 0;
            MenuOn = false;
            FirstKey = "";
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Set a point on a line (key)");
            Mode = MENU.POINT_ON_LINE;
            ModeStep = 0;
            MenuOn = false;
            FirstKey = "";
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Set a point on a circle (key)");
            Mode = MENU.POINT_ON_CIRCLE;
            ModeStep = 0;
            MenuOn = false;
            FirstKey = "";
        }
    }

    void OnKeyLine()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("Let a line horizontal (key)");
            Mode = MENU.LINE_HORIZONTAL;
            ModeStep = 0;
            MenuOn = false;
            FirstKey = "";
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("Let two lines isometry(key)");
            Mode = MENU.LINES_ISOMETRY;
            ModeStep = 0;
            MenuOn = false;
            FirstKey = "";
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Let two lines perpendicular (key)");
            Mode = MENU.LINES_PERPENDICULAR;
            ModeStep = 0;
            MenuOn = false;
            FirstKey = "";
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Let two lines parallel (key)");
            Mode = MENU.LINES_PARALLEL;
            ModeStep = 0;
            MenuOn = false;
            FirstKey = "";
        }
    }

    void OnKeyTangent()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Make a circle tangent to a line (key)");
            Mode = MENU.CIRCLE_TANGENT_LINE;
            ModeStep = 0;
            MenuOn = false;
            FirstKey = "";
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Make a circle tangent to a circle  (key)");
            Mode = MENU.CIRCLE_TANGENT_CIRCLE;
            ModeStep = 0;
            MenuOn = false;
            FirstKey = "";
        }
    }

    void OnKeyFix()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("fix a point (key)");
            Mode = MENU.FIX_POINT;
            ModeStep = 0;
            MenuOn = false;
            FirstKey = "";
        }
    }

    void OnKeyDelete()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("delete a point (key)");
            Mode = MENU.DELETE_POINT;
            ModeStep = 0;
            MenuOn = false;
            FirstKey = "";
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("clear all  (key)");
            Mode = MENU.DELETE_ALL;
            ClickOnPanel.DeleteAll();
            Mode = 0;
            ModeStep = 0;
            MenuOn = false;
            FirstKey = "";
        }
    }


    public void OnMyMouseDown()
    {
        if (!DrawOn) return;
        if (Camera.main == null) return;
        MouseDownVec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        MouseDownVec.z = 0.0f;
        DraggedPointId = MousePointPosition();
        DraggedObjectId = getObjectFromMousePosition(true);
        //print(DraggedPointId);
        DraggedGameLogStartTop = Util.StartTop;
        DraggedPreferencePosition = PreferenceDialog.GetComponent<Preference>().Position;
        PreviousMouseDragPosition = MouseDownVec;
    }


    public void OnMouseDrag()
    {
        Vector3 MouseDragPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        MouseDragPosition.z = 0f;
        if (pts == null) return;
        if (!DrawOn) return;
        if (DraggedObjectId != -1)
        {
            Vector3 v3 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            v3.z = 0.0f;
            if(DraggedObjectId == 5000)
            {//プリファレンスダイアログをドラッグ
                PreferenceDialog.GetComponent<Preference>().Position = DraggedPreferencePosition + v3 - MouseDownVec;
                PreferenceDialog.GetComponent<Preference>().SetScreenPosition();
            }
            else if (DraggedObjectId >= 4000 && DraggedObjectId < 5000)
            {// ログをドラッグ
                Util.StartTop = DraggedGameLogStartTop + (v3.y - MouseDownVec.y);
                int LL = Util.LogLength;
                if (Util.StartTop > 3f + LL)
                {
                    Util.StartTop = 3f + LL;
                }
                else if(Util.StartTop < -4f)
                {
                    Util.StartTop = -4f;
                }
            }
            else
            {
                for (int i = 0; i < pts.Length; i++)
                {//点のドラッグ
                    if (pts[i].Id == DraggedPointId)
                    {
                        if (MouseDownVec - v3 != Vector3.zero)//0.768
                        {
                            pts[i].Vec = v3;
                            ExecuteAllModules();
                        }
                    }
                }
            }
        }
        else
        {//空ドラッグ→画面全体の平行移動
            if (!Util.FixDisplay)
            {
                pts = FindObjectsOfType<Point>();
                for (int i = 0; i < pts.Length; i++)
                {
                    Point pt = pts[i];
                    pt.Vec += (MouseDragPosition - PreviousMouseDragPosition);
                }
                PreviousMouseDragPosition = MouseDragPosition;
            }
        }
    }

    private int AddNewPoint()
    {
        Util.AddPoint(MouseUpVec, PointId);
        //        Util.DebugLog();
        // 新しい点をselected，そのほかの点は選択をはずす。
        Point.MakeOnePointSelected(PointId);
        Line.AllLinesUnselected();
        Circle.AllCirclesUnselected();
        PointId++;
        return PointId - 1;
    }

    private void AddNewMidpoint(int MOP)
    {
        if (ModeStep == 0)
        {//モード１２ステップ０ならば，「一つ目の点」をFirstClickIdに記録
            Point.MakeOnePointSelected(MOP);//クリックしたポイントのみを選択
            FirstClickId = MOP;
            ModeStep = 1;
        }
        else if (ModeStep == 1)
        {//モード１２ステップ１ならば，「二つ目の点」をSecondClickIdに記録
            Point.AddOnePointSelected(MOP);//クリックしたポイントを追加選択
            SecondClickId = MOP;
            if (FirstClickId != SecondClickId)
            {
                // 新しい点の追加
                Util.AddMidpoint(FirstClickId, SecondClickId, PointId, ModuleId);
                //ログの作成とlogsへの追加
                Point.AddOnePointSelected(PointId);
                PointId++;
                ModuleId++;
            }
            Mode = MENU.ADD_MIDPOINT;
            //Mode = 0;
            ModeStep = 0;
        }
    }

    private void AddNewLine(int MOP)
    {
        if (ModeStep == 0)
        {//モード１ステップ０ならば，「一つ目の点」をFirstClickIdに記録
            Point.MakeOnePointSelected(MOP);//クリックしたポイントのみを選択
            FirstClickId = MOP;
            ModeStep = 1;
        }
        else if (ModeStep == 1)
        {//モード１ステップ１ならば，「二つ目の点」をSecondClickIdに記録
            Point.AddOnePointSelected(MOP);//クリックしたポイントを追加選択
            SecondClickId = MOP;
            // 新しい線の追加
            if (FirstClickId != SecondClickId)
            {
                Util.AddLine(FirstClickId, SecondClickId, LineId++);
                //追加したラインを選択
                Line.MakeOneLineSelected(LineId - 1);
            }
            Point.AllPointsUnselected();
            Mode = MENU.ADD_LINE;
            ModeStep = 0;
        }
    }

    private int AddCircle(int MOP)
    {// 円を追加
        if (ModeStep == 0 && 0 <= MOP && MOP < 1000)
        {//モード４ステップ０ならば一つ目の点のIDをFirstClickIdに記録
            Point.MakeOnePointSelected(MOP);//クリックしたポイントのみを選択
            FirstClickId = MOP;
            FirstClickVec = MouseUpVec;
            ModeStep = 1;
            return CircleId;
        }
        else if (ModeStep == 1)
        {// モード４ステップ１ならば、点 MouseUpVec を使って円を描く。
            //半径を計算する
            float rd = Hypot(MouseUpVec.x - FirstClickVec.x, MouseUpVec.y - FirstClickVec.y);
            Util.AddCircle(FirstClickId, rd, CircleId);
            // 新しい点をselected，そのほかの点は選択をはずす。
            Circle.MakeOneCircleSelected(CircleId);
            Point.AllPointsUnselected();
            //cis = FindObjectsOfType<Circle>();
            Mode = MENU.ADD_CIRCLE;
            ModeStep = 0;
            CircleId++;
            return CircleId - 1;
        }
        return -1;
    }

    private void MakeTwoPointsIntoOne(int MOP)
    {
        if (ModeStep == 0)
        {//モード２ステップ０ならば，「一つ目の点」をFirstClickIdに記録
            Point.MakeOnePointSelected(MOP);//クリックしたポイントのみを選択
            FirstClickId = MOP;
            ModeStep = 1;
        }
        else if (ModeStep == 1)
        {//モード２ステップ１ならば，「二つ目の点」をSecondClickIdに記録
            Point.AddOnePointSelected(MOP);//クリックしたポイントを追加選択
            SecondClickId = MOP;
            if (FirstClickId != SecondClickId)
            {
                // 新しいモジュールの追加
                Util.AddModule(MENU.POINT_ON_POINT, FirstClickId, SecondClickId, 0, ModuleId++);
                AppMgr.ExecuteAllModules();
            }
            Mode = MENU.POINT_ON_POINT;
            //Mode = 0;
            ModeStep = 0;
        }
    }

    private void MakeAPointOnLine(int MOP)
    {
        if (ModeStep == 0 && 0<= MOP && MOP <1000)
        {//モード３ステップ０ならば，「一つ目の点」をFirstClickIdに記録
            Point.MakeOnePointSelected(MOP);//クリックしたポイントのみを選択
            FirstClickId = MOP;
            ModeStep = 1;
        }
        else if (ModeStep == 1 && 1000 <= MOP && MOP < 2000)
        {//モード３ステップ１ならば，「一つ目の線」をSecondClickIdに記録
            Line.AddOneLineSelected(MOP);//クリックしたポイントを追加選択
            SecondClickId = MOP;
            // 新しいモジュールの追加
            Util.AddModule(MENU.POINT_ON_LINE, FirstClickId, SecondClickId, 0, ModuleId++);
            // Debug.Log("New module was created " + FirstClickId + " " + SecondClickId);
            // 細い補助線の追加
            Util.AddThinLine(FirstClickId, SecondClickId);
            Mode = MENU.POINT_ON_LINE;
            ModeStep = 0;
            AppMgr.ExecuteAllModules();
        }
    }

    private void MakeAPointOnCircle(int MOP)
    {
        if (ModeStep == 0 && 0 <= MOP && MOP < 1000)
        {//モード５ステップ０ならば，「一つ目の点」をFirstClickIdに記録
            Point.MakeOnePointSelected(MOP);//クリックしたポイントのみを選択
            FirstClickId = MOP;
            ModeStep = 1;
        }
        else if (ModeStep == 1 && 2000 <= MOP && MOP < 3000)
        {//モード５ステップ１ならば，「一つ目の円」をSecondClickIdに記録
            Circle.AddOneCircleSelected(MOP);//クリックしたポイントを追加選択
            SecondClickId = MOP;
            // 新しいモジュールの追加
            Util.AddModule(MENU.POINT_ON_CIRCLE, FirstClickId, SecondClickId, 0, ModuleId++);
            // Debug.Log("New module was created " + FirstClickId + " " + SecondClickId);
            Mode = MENU.POINT_ON_CIRCLE;
            //Mode = 0;
            ModeStep = 0;
            AppMgr.ExecuteAllModules();
        }
    }

    private void MakeIntersection(int MOP)
    {
        Debug.Log(ModeStep + "," + MOP);
        if (ModeStep == 0 && 1000 <= MOP && MOP < 2000)
        {//モード６ステップ０ならば，「一つ目の線」をFirstClickIdに記録
            Line.MakeOneLineSelected(MOP);//クリックしたポイントのみを選択
            FirstClickId = MOP;
            ModeStep = 1;
        }
        else if (ModeStep == 0 && 2000 <= MOP && MOP < 3000)
        {//ステップ０ならば，「一つ目の円」をFirstClickIdに記録
            Circle.MakeOneCircleSelected(MOP);//クリックしたポイントのみを選択
            FirstClickId = MOP;
            ModeStep = 1;
        }
        else if (ModeStep == 1 && 1000 <= MOP && MOP < 2000)
        {//ステップ1ならば，「2つ目の線」をSecondClickIdに記録
            Line.AddOneLineSelected(MOP);//クリックしたラインのみを選択
            SecondClickId = MOP;
            if (FirstClickId != SecondClickId)
            {
                // 新しい点の追加
                int NewPointId = PointId;
                Util.AddPoint(MouseUpVec, PointId++);
                // 新しいモジュールの追加
                if (1000 <= FirstClickId && FirstClickId < 2000)
                {// 一つ目に選んだものが直線のとき
                    Util.AddModule(MENU.POINT_ON_LINE, NewPointId, FirstClickId, 0, ModuleId++);
                    Util.AddModule(MENU.POINT_ON_LINE, NewPointId, SecondClickId, 0, ModuleId++);
                }
                else if (2000 <= FirstClickId && FirstClickId < 3000)
                {// 一つ目に選んだものが円のとき
                    Util.AddModule(MENU.POINT_ON_CIRCLE, NewPointId, FirstClickId, 0, ModuleId++);
                    Util.AddModule(MENU.POINT_ON_LINE, NewPointId, SecondClickId, 0, ModuleId++);
                }
                Mode = MENU.POINT_ON_LINE;
                AppMgr.ExecuteAllModules();
            }
            else
            {
                Mode = MENU.INTERSECTION;
            }
            ModeStep = 0;
        }
        else if (ModeStep == 1 && 2000 <= MOP && MOP < 3000)
        {//ステップ1ならば，「2つ目の円」をSecondClickIdに記録
            Line.AddOneLineSelected(MOP);//クリックした円のみを選択
            SecondClickId = MOP;
            if (FirstClickId != SecondClickId)
            {
                // 新しい点の追加
                int NewPointId = PointId;
                Util.AddPoint(MouseUpVec, PointId++);
                // 新しいモジュールの追加
                if (1000 <= FirstClickId && FirstClickId < 2000)
                {// 一つ目に選んだものが直線のとき
                    Util.AddModule(MENU.POINT_ON_LINE, NewPointId, FirstClickId, 0, ModuleId++);
                    Util.AddModule(MENU.POINT_ON_CIRCLE, NewPointId, SecondClickId, 0, ModuleId++);
                }
                else if (2000 <= FirstClickId && FirstClickId < 3000)
                {// 一つ目に選んだものが円のとき
                    Util.AddModule(MENU.POINT_ON_CIRCLE, NewPointId, FirstClickId, 0, ModuleId++);
                    Util.AddModule(MENU.POINT_ON_CIRCLE, NewPointId, SecondClickId, 0, ModuleId++);
                }
                Mode = MENU.POINT_ON_LINE;
                AppMgr.ExecuteAllModules();
            }
            else
            {
                Mode = MENU.INTERSECTION;
            }
            ModeStep = 0;
        }

    }

    private void MakeTwoLinesIsometry(int MOP)
    {
        if (ModeStep == 0 && 1000 <= MOP && MOP < 2000)
        {//モード６ステップ０ならば，「一つ目の線」をFirstClickIdに記録
            Line.MakeOneLineSelected(MOP);//クリックしたポイントのみを選択
            FirstClickId = MOP;
            ModeStep = 1;
        }
        else if (ModeStep == 1 && 1000 <= MOP && MOP < 2000)
        {//モード６ステップ１ならば，「二つ目の線」をSecondClickIdに記録
            Line.AddOneLineSelected(MOP);//クリックしたポイントを追加選択
            SecondClickId = MOP;
            if (FirstClickId != SecondClickId)
            {
                Module NewMd = Util.AddModule(MENU.LINES_ISOMETRY, FirstClickId, SecondClickId, 0, ModuleId++);
                Util.SetIsometry();
            }
            Mode = MENU.LINES_ISOMETRY;
            //Mode = 0;
            ModeStep = 0;
        }
    }

    private void MakeRatioLength(int MOP)
    {
        if (ModeStep == 0 && 1000 <= MOP && MOP < 2000)
        {//ステップ０ならば，「一つ目の線」をFirstClickIdに記録
            Line.MakeOneLineSelected(MOP);//クリックしたポイントのみを選択
            FirstClickId = MOP;
            ModeStep = 1;
        }
        else if (ModeStep == 1 && 1000 <= MOP && MOP < 2000)
        {//ステップ１ならば，「二つ目の線」をSecondClickIdに記録
            Line.AddOneLineSelected(MOP);//クリックしたポイントを追加選択
            SecondClickId = MOP;
            if (FirstClickId != SecondClickId)
            {
                //追加するモジュールとしてはLINES_ISOMETRY
                Module NewMd = Util.AddModule(MENU.LINES_ISOMETRY, FirstClickId, SecondClickId, 0, ModuleId++, false);
                Util.SetIsometry();
            }
            Mode = MENU.LINES_ISOMETRY;
            //Mode = 0;
            ModeStep = 0;
        }
    }

    private void MakeTwoLinesPerpendicular(int MOP)
    {
        if (ModeStep == 0 && 1000 <= MOP && MOP < 2000)
        {//モード７ステップ０ならば，「一つ目の線」をFirstClickIdに記録
            Line.MakeOneLineSelected(MOP);//クリックしたポイントのみを選択
            FirstClickId = MOP;
            ModeStep = 1;
        }
        else if (ModeStep == 1 && 1000 <= MOP && MOP < 2000)
        {//モード７ステップ１ならば，「二つ目の線」をSecondClickIdに記録
            Line.AddOneLineSelected(MOP);//クリックしたポイントを追加選択
            SecondClickId = MOP;
            if (FirstClickId != SecondClickId)
            {
                // 新しいモジュールの追加
                Module MD = Util.AddModule(MENU.LINES_PERPENDICULAR, FirstClickId, SecondClickId, 0, ModuleId++);
                // 新しい直角記号の追加
                Util.AddAngleMark(FirstClickId, SecondClickId, MD.gameObject);
                AppMgr.ExecuteAllModules();
            }
            Mode = MENU.LINES_PERPENDICULAR;
            //Mode = 0;
            ModeStep = 0;
        }
    }

    private void MakeTwoLinesParallel(int MOP)
    {
        if (ModeStep == 0 && 1000 <= MOP && MOP < 2000)
        {//モード８ステップ０ならば，「一つ目の線」をFirstClickIdに記録
            Line.MakeOneLineSelected(MOP);//クリックしたポイントのみを選択
            FirstClickId = MOP;
            ModeStep = 1;
        }
        else if (ModeStep == 1 && 1000 <= MOP && MOP < 2000)
        {//モード８ステップ１ならば，「二つ目の線」をSecondClickIdに記録
            Line.AddOneLineSelected(MOP);//クリックしたポイントを追加選択
            SecondClickId = MOP;
            // 新しいモジュールの追加
            Util.AddModule(MENU.LINES_PARALLEL, FirstClickId, SecondClickId, 0, ModuleId++);
            Mode = MENU.LINES_PARALLEL;
            AppMgr.ExecuteAllModules();
            //Mode = 0;
            ModeStep = 0;
        }
    }
    private void MakeALineHorizontal(int MOP)
    {
        if (ModeStep == 0 && 1000 <= MOP && MOP < 2000)
        {//モード２７ステップ０ならば，「一つ目の線」をFirstClickIdに記録
            Line.MakeOneLineSelected(MOP);//クリックしたポイントのみを選択
            FirstClickId = MOP;
            // 新しいモジュールの追加
            Util.AddModule(MENU.LINE_HORIZONTAL, FirstClickId, 0, 0, ModuleId++);
            Mode = MENU.LINE_HORIZONTAL;
            AppMgr.ExecuteAllModules();
            //Mode = 0;
            ModeStep = 0;
        }
    }


    private void MakeThreePointAngle(int MOP)
    {
        if (ModeStep == 0 && 0 <= MOP && MOP < 1000)
        {//アングルモード、ステップ０ならば，「一つ目の点」をFirstClickIdに記録
            Point.AllPointsUnselected();
            Point.AddOnePointSelected(MOP);//クリックした点を選択
            FirstClickId = MOP;
            ModeStep = 1;
        }
        else if (ModeStep == 1 && 0 <= MOP && MOP < 1000)
        {//アングルモード、ステップ1ならば，「２つ目の点」をSecondClickIdに記録
            Point.AddOnePointSelected(MOP);//クリックした点のみを選択
            SecondClickId = MOP;
            ModeStep = 2;
        }
        else if (ModeStep == 2 && 0 <= MOP && MOP < 1000)
        {//アングルモード,ステップ２ならば，「3つ目の線」をThirdClickIdに記録
            Point.AddOnePointSelected(MOP);//クリックした点を選択
            ThirdClickId = MOP;
            // 新しいモジュールの追加
            Module MD = Util.AddModule(MENU.ANGLE, FirstClickId, SecondClickId, ThirdClickId, ModuleId++);
            MD.Constant = Mathf.PI / 3f;// 定数をプリセット
            //新たな直角マークの追加
            Util.AddAngleMark(FirstClickId, SecondClickId, ThirdClickId, MD.gameObject);
            Mode = MENU.ANGLE;
            ModeStep = 0;
        }
    }

    private void MakeBisector(int MOP)
    {
        if (ModeStep == 0 && 3000 <= MOP && MOP < 4000)//AngleMarkを選択した時、という条件を付け加えたい
        {//アングルモード、ステップ０ならば，「一つ目の点」をFirstClickIdに記録
            //Point.AllPointsUnselected();
            //Point.AddOnePointSelected(MOP);//クリックした点を選択
              //選んだ時に選択状態にしておく
            FirstClickId = MOP;
            ModeStep = 1;
        }
        else if (ModeStep == 1 && 3000 <= MOP && MOP < 4000)
        {//アングルモード,ステップ２ならば，「3つ目の線」をSecondClickIdに記録
            //Point.AddOnePointSelected(MOP);//クリックした点を選択
            SecondClickId = MOP;
            // 新しいモジュールの追加
            Module MD = Util.AddModule(MENU.BISECTOR, FirstClickId, SecondClickId, 0, ModuleId++);
            Mode = MENU.BISECTOR;
            AppMgr.ExecuteAllModules();
            ModeStep = 0;
        }
    }

    private void MakeCircleTangentLine(int MOP)
    {
        if (ModeStep == 0 && 2000 <= MOP && MOP < 3000)
        {//モード９ステップ０ならば，「一つ目の円」をFirstClickIdに記録
            Circle.MakeOneCircleSelected(MOP);//クリックしたポイントのみを選択
            FirstClickId = MOP;
            ModeStep = 1;
        }
        else if (ModeStep == 1 && 1000 <= MOP && MOP < 2000)
        {//モード９ステップ１ならば，「二つ目の線」をSecondClickIdに記録
            Line.AddOneLineSelected(MOP);//クリックしたポイントを追加選択
            SecondClickId = MOP;
            // 新しいモジュールの追加
            Util.AddModule(MENU.CIRCLE_TANGENT_LINE, FirstClickId, SecondClickId, 0, ModuleId++);
            Mode = MENU.CIRCLE_TANGENT_LINE;
            AppMgr.ExecuteAllModules();
            //Mode = 0;
            ModeStep = 0;
        }
    }

    private void MakeCircleTangentCircle(int MOP)
    {
        if (ModeStep == 0 && 2000 <= MOP && MOP < 3000)
        {//モード１０ステップ０ならば，「一つ目の円」をFirstClickIdに記録
            Circle.MakeOneCircleSelected(MOP);//クリックした円のみを選択
            FirstClickId = MOP;
            ModeStep = 1;
        }
        else if (ModeStep == 1 && 2000 <= MOP && MOP < 3000)
        {//モード１０ステップ１ならば，「二つ目の円」をSecondClickIdに記録
            Circle.AddOneCircleSelected(MOP);//クリックした円を追加選択
            SecondClickId = MOP;
            if (FirstClickId != SecondClickId)
            {
                // 新しいモジュールの追加
                Util.AddModule(MENU.CIRCLE_TANGENT_CIRCLE, FirstClickId, SecondClickId, 0, ModuleId++);
            }
            Mode = MENU.CIRCLE_TANGENT_CIRCLE;
            AppMgr.ExecuteAllModules();
            //Mode = 0;
            ModeStep = 0;
        }

    }

    private void MakeAPointFixed(int MOP)
    {//モード１１ステップ０ならば，「一つ目の点」のMOPから
     // 該当する点のFixedフラグを反転させる。
        if (pts == null) return;

        Point.MakeOnePointSelected(MOP);//クリックしたポイントのみを選択
        FirstClickId = MOP;

        for(int i = 0; i < pts.Length; ++i)
        {
            if(pts[i].Id == MOP)
            {
                pts[i].Fixed = !pts[i].Fixed;
            }
        }

        Util.MakeALogFixed(MOP);
        Util.CopyLog("TmpLog.txt", "TmpBeforeLastLog.txt");
        Util.SaveLog("TmpLog.txt");

        Mode = MENU.FIX_POINT;
        //Mode = 0;
        ModeStep = 0;
    }

    private void AddNewLocus(int MOP)
    {//ステップ０ならば，「一つ目の点」のMOPから
        if (pts == null) return;
        Util.FixDisplay = true;
        Point.MakeOnePointSelected(MOP);//クリックしたポイントのみを選択
        FirstClickId = MOP;
        Module NewMd = Util.AddModule(MENU.ADD_LOCUS, FirstClickId, 0, 0, ModuleId++);
        Mode = MENU.ADD_LOCUS;
        ModeStep = 0;
    }

    private void DeleteAPoint(int MOP)
    {
        GameObject[] gp = FindObjectsOfType<GameObject>();
        if (gp != null)
        {
            for (int i = 0; i < gp.Length; ++i)
            {
                Module md = (Module)gp[i].GetComponent("Module");
                if (md != null)
                {
                    if (md.Object1Id == MOP || md.Object2Id == MOP || md.Object3Id == MOP)
                    {
                        Util.DeleteLogAtID(md.Id);//ログの消去
                        Destroy(gp[i]);//モジュールの消去
                        mds = FindObjectsOfType<Module>();
                    }
                }
                Circle ci = (Circle)gp[i].GetComponent("Circle");
                if (ci != null)
                {
                    if (ci.CenterPointId == MOP)
                    {
                        GameObject[] gp2 = FindObjectsOfType<GameObject>();
                        if (gp2 != null)
                        {
                            for (int j = 0; j < gp2.Length; ++j)
                            {
                                Module md2 = (Module)gp2[j].GetComponent("Module");
                                if (md2 != null)
                                {
                                    if (md2.Object1Id == ci.Id || md2.Object2Id == ci.Id || md2.Object3Id == ci.Id)
                                    {
                                        Util.DeleteLogAtID(md2.Id);//ログの消去
                                        Destroy(gp2[j]);//モジュールの消去
                                        mds = FindObjectsOfType<Module>();
                                        break;
                                    }

                                }

                            }
                            Util.DeleteLogAtID(ci.Id);//ログの消去
                            Destroy(gp[i]);//円の消去
                            cis = FindObjectsOfType<Circle>();
                        }
                    }
                }
                Line ln = (Line)gp[i].GetComponent("Line");
                if (ln != null)
                {
                    if (ln.Point1Id == MOP || ln.Point2Id == MOP)
                    {
                        GameObject[] gp2 = FindObjectsOfType<GameObject>();
                        if (gp2 != null)
                        {
                            for (int j = 0; j < gp2.Length; ++j)
                            {
                                Module md2 = (Module)gp2[j].GetComponent("Module");
                                if (md2 != null)
                                {
                                    if (md2.Object1Id == ln.Id || md2.Object2Id == ln.Id || md2.Object3Id == ln.Id)
                                    {
                                        // モジュールを消去する際に，「直角マーク」が関連していたらそれも消去する。
                                        if (md2.Type == MENU.LINES_PERPENDICULAR)//直交させるモジュールの場合
                                        {
                                            GameObject[] gp3 = FindObjectsOfType<GameObject>();
                                            if (gp3 != null)
                                            {
                                                for (int k = 0; k < gp3.Length; k++)
                                                {
                                                    AngleMark am = (AngleMark)gp3[k].GetComponent("AngleMark");
                                                    if (am != null)
                                                    {
                                                        if (am.Object1Id == ln.Id || am.Object2Id == ln.Id)
                                                        {
                                                            Destroy(gp3[k]);
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        // モジュールを消去する際に，「細線補助線」が関連していたらそれも消去する。
                                        if (md2.Type == MENU.POINT_ON_LINE)//直交させるモジュールの場合
                                        {
                                            GameObject[] gp3 = FindObjectsOfType<GameObject>();
                                            if (gp3 != null)
                                            {
                                                for (int k = 0; k < gp3.Length; k++)
                                                {
                                                    ThinLine TL = (ThinLine)gp3[k].GetComponent("ThinLine");
                                                    if (TL != null)
                                                    {
                                                        if (TL.LineId == ln.Id)
                                                        {
                                                            Destroy(gp3[k]);
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        Util.DeleteLogAtID(md2.Id);//ログの消去
                                        Destroy(gp2[j]);//モジュールの消去
                                        mds = FindObjectsOfType<Module>();
                                        break;
                                    }
                                }
                            }
                            Util.DeleteLogAtID(ln.Id);//ログの消去
                            Destroy(gp[i]);//直線の消去
                            lns = FindObjectsOfType<Line>();
                        }
                    }
                }
                Point obj = (Point)gp[i].GetComponent("Point");
                if (obj != null)
                {
                    if (obj.Id == MOP)
                    {
                        GameObject[] gp2 = FindObjectsOfType<GameObject>();
                        if (gp2 != null)
                        {
                            for (int j = 0; j < gp2.Length; ++j)
                            {
                                Module md2 = (Module)gp2[j].GetComponent("Module");
                                if (md2 != null)
                                {
                                    if (md2.Object1Id == MOP || md2.Object2Id == MOP || md2.Object3Id == MOP)
                                    {
                                        // モジュールを消去する際に，「細線補助線」が関連していたらそれも消去する。
                                        if (md2.Type == MENU.POINT_ON_LINE)//直交させるモジュールの場合
                                        {
                                            GameObject[] gp3 = FindObjectsOfType<GameObject>();
                                            if (gp3 != null)
                                            {
                                                for (int k = 0; k < gp3.Length; k++)
                                                {
                                                    ThinLine TL = (ThinLine)gp3[k].GetComponent("ThinLine");
                                                    if (TL != null)
                                                    {
                                                        if (TL.PointId == MOP)
                                                        {
                                                            Destroy(gp3[k]);
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        Util.DeleteLogAtID(MOP);//ログの消去
                        Destroy(obj.PTobject, 0f);//点に付随する文字の消去
                        Destroy(gp[i], 0.5f);//モジュールの消去

                    }
                }
            }
        }
        Mode = MENU.DELETE_POINT;
        ModeStep = 0;

    }

    public static  void DeleteAll()
    {
        Util.SetAllLogActive();
        GameObject[] gp = FindObjectsOfType<GameObject>();
        for (int i = gp.Length-1; i>=0; i--)
        {
            Module md = (Module)gp[i].GetComponent("Module");
            if (md != null)
            {
                Destroy(gp[i]);
            }
            //直角マーク(角度マーク)も消しておく。
            AngleMark am = (AngleMark)gp[i].GetComponent("AngleMark");
            if (am != null)
            {
                Destroy(gp[i]);
            }
            //細線補助線も消しておく。
            ThinLine TL = (ThinLine)gp[i].GetComponent("ThinLine");
            if (TL != null)
            {
                Destroy(gp[i]);
            }
            //軌跡も消しておく。
            LocusDot LD = (LocusDot)gp[i].GetComponent("LocusDot");
            if (LD != null)
            {
                Destroy(gp[i]);
            }
            Circle ci = (Circle)gp[i].GetComponent("Circle");
            if (ci != null)
            {
                    Destroy(gp[i],0.2f);
            }
            Line ln = (Line)gp[i].GetComponent("Line");
            if (ln != null)
            {
                    Destroy(gp[i],0.4f);
            }
            Point obj = (Point)gp[i].GetComponent("Point");
            if (obj != null)
            {
                    Destroy(gp[i], 0.6f);
            }
        }
        Util.InitLog();
        pts = null;
        lns = null;
        cis = null;
        mds = null;
        Mode = MENU.DELETE_ALL;
        //Mode = 0;
        ModeStep = 0;
    }


    public void OnMouseUp()
    {
        if (!DrawOn) return;
        Vector3 mPs = Input.mousePosition;
        mPs.y = Screen.height - mPs.y;
        if (mPs.x < 110 && mPs.y < 90) return;
        if (Camera.main == null) return;
        MouseUpVec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        MouseUpVec.z = 0.0f;
        if (Hypot(MouseDownVec.x - MouseUpVec.x, MouseDownVec.y - MouseUpVec.y) < 0.1)
        {// クリックののちマウスアップ
            //ログの右上をクリック
            int MOL = MouseOnGameLog(MouseUpVec); 
            if (4000 <= MOL && MOL < 4500)
            {
                PreferenceDialog.GetComponent<Preference>().SetData(Util.logs[MOL - 4000]);
                Debug.Log("thru here");
                PreferenceDialog.GetComponent<Preference>().show = true;
                Debug.Log(PreferenceDialog.GetComponent<Preference>().show);
            }
            //アニメーション
            GameObject _prefab = Resources.Load<GameObject>("Prefabs/MouseDown");
            GameObject g = Point.Instantiate(_prefab, MouseDownVec, Quaternion.identity) as GameObject;
            MouseDown obj = g.GetComponent<MouseDown>();
            obj.Vec = MouseDownVec;
            //Point obj = g.GetComponent<Point>();
            Destroy(g, 1.2f);//モジュールの消去

            int MOP = getObjectFromMousePosition(false);
            //Debug.Log("MOP (OnMouseUp) = " + MOP);
            if (MOP == -2)
            {//モード切替
                MenuOn = true;
            }
            if (MOP == -1)
            {//カラ打ち
                if(Mode == MENU.ADD_LINE && ModeStep == 0)
                {
                    MOP = AddNewPoint();
                    AddNewLine(MOP);
                }
                else if (Mode == MENU.ADD_LINE && ModeStep == 1)
                {
                    MOP = AddNewPoint();
                    AddNewLine(MOP);
                }
                else if (Mode == MENU.POINT_ON_LINE && ModeStep == 0)
                {
                    MOP = AddNewPoint();
                    MakeAPointOnLine(MOP);
                }
                else if (Mode == MENU.POINT_ON_CIRCLE && ModeStep == 0)
                {
                    MOP = AddNewPoint();
                    MakeAPointOnCircle(MOP);
                }

                

                else if (Mode == MENU.ADD_CIRCLE && ModeStep == 0)
                {//円を描く。から打ちして場所を決める。
                    MOP = AddNewPoint();
                    AddCircle(MOP);
                }
                else if (Mode == MENU.ADD_CIRCLE && ModeStep == 1)
                {//円を描く。から打ちして場所を決める。
                    AddCircle(MOP);
                }
                else
                {//新しい点の追加
                    AddNewPoint();
                }
            }
            else if (0 <= MOP && MOP < 1000)
            {// 点の上をクリック
                if (Mode == MENU.ADD_POINT)
                {//クリックしたポイントのみを選択
                    Point.MakeOnePointSelected(MOP);
                    Line.AllLinesUnselected();
                }
                if (Mode == MENU.ADD_LINE)
                {//２点を通る線分を追加
                    AddNewLine(MOP);
                }
                else if (Mode == MENU.POINT_ON_POINT)
                {//二つの点を合流する
                    MakeTwoPointsIntoOne(MOP);
                }
                else if (Mode == MENU.POINT_ON_LINE && ModeStep == 0)
                {//点を線に載せるのに、まず点を選ぶ
                    MakeAPointOnLine(MOP);
                }
                else if(Mode == MENU.ADD_CIRCLE && ModeStep == 0)
                {//円を追加するのに、まず点を選ぶ
                    AddCircle(MOP);
                }
                else if (Mode == MENU.ADD_CIRCLE && ModeStep == 1)
                {//円を追加するのに、点をクリックしたら、その点を通るような円にする
                    int id = AddCircle(MOP);// 円のId
                    Mode = MENU.POINT_ON_CIRCLE;
                    ModeStep = 0;
                    MakeAPointOnCircle(MOP);
                    MakeAPointOnCircle(id);
                }
                else if (Mode == MENU.POINT_ON_CIRCLE && ModeStep == 0)
                {//点を円に載せるのに、まず点を選ぶ
                    MakeAPointOnCircle(MOP);
                }
                else if (Mode == MENU.FIX_POINT)
                {//点を固定するのに、点を選ぶ
                    MakeAPointFixed(MOP);
                }
                else if (Mode == MENU.ADD_MIDPOINT)
                {//２点の中点を追加
                    AddNewMidpoint(MOP);
                }
                else if (Mode == MENU.ADD_LOCUS)
                {//点の軌跡を追加
                    AddNewLocus(MOP);
                }
                else if(Mode == MENU.DELETE_POINT)
                {// 点を一つ消去する
                    DeleteAPoint(MOP);
                }
                else if (Mode == MENU.ANGLE)
                {//角度を決める三点を選ぶのに、点を選ぶ
                    MakeThreePointAngle(MOP);
                }
            }
            else if (1000 <= MOP && MOP < 2000)
            {// ラインの上をクリック
                if(Mode == MENU.ADD_POINT)
                {
                    Mode = MENU.POINT_ON_LINE;
                    ModeStep = 0;
                    int id = AddNewPoint();
                    MakeAPointOnLine(id);
                    MakeAPointOnLine(MOP);
                }
                else if (Mode == MENU.POINT_ON_LINE && ModeStep == 0)
                {
                    int id = AddNewPoint();
                    MakeAPointOnLine(id);
                    MakeAPointOnLine(MOP);
                }
                else if (Mode == MENU.POINT_ON_LINE && ModeStep == 1)
                {//点を線に載せるのに、次に線をを選ぶ
                    MakeAPointOnLine(MOP);
                }

                else if (Mode == MENU.INTERSECTION && ModeStep == 0)
                {
                    MakeIntersection(MOP);
                }
                else if (Mode == MENU.INTERSECTION && ModeStep == 1)
                {
                    MakeIntersection(MOP);
                }

                else if (Mode == MENU.LINES_ISOMETRY)
                {//２つの線の長さを同じにするのに、線を選ぶ
                    MakeTwoLinesIsometry(MOP);
                }
                else if (Mode == MENU.RATIO_LENGTH)
                {//２つの線の長さの比を表示するのに、線を選ぶ
                    MakeRatioLength(MOP);
                }
                else if (Mode == MENU.LINES_PERPENDICULAR)
                {//２つの線を直交にするのに、線を選ぶ
                    MakeTwoLinesPerpendicular(MOP);
                }
                else if (Mode == MENU.LINES_PARALLEL)
                {//２つの線を並行にするのに、線を選ぶ
                    MakeTwoLinesParallel(MOP);
                }
                else if (Mode == MENU.LINE_HORIZONTAL)
                {//１つの線を水平にするのに、線を選ぶ
                    MakeALineHorizontal(MOP);
                }
                else if (Mode == MENU.CIRCLE_TANGENT_LINE && ModeStep == 1)
                {
                    MakeCircleTangentLine(MOP);
                }
            }
            else if (2000 <= MOP && MOP < 3000)
            {// サークルの上をクリック
                if (Mode == MENU.ADD_POINT)
                {
                    Mode = MENU.POINT_ON_CIRCLE;
                    ModeStep = 0;
                    int id0 = AddNewPoint();
                    MakeAPointOnCircle(id0);
                    MakeAPointOnCircle(MOP);
                }
                else if (Mode == MENU.POINT_ON_CIRCLE && ModeStep == 0)
                {//点を円に載せる
                    int id0 = AddNewPoint();
                    MakeAPointOnCircle(id0);
                    MakeAPointOnCircle(MOP);
                }
                else if (Mode == MENU.POINT_ON_CIRCLE && ModeStep == 1)
                {//点を円に載せるのに、次に円を選ぶ
                        MakeAPointOnCircle(MOP);
                }

                else if (Mode == MENU.INTERSECTION && ModeStep == 0)
                {
                    MakeIntersection(MOP);
                }
                else if (Mode == MENU.INTERSECTION && ModeStep == 1)
                {
                    MakeIntersection(MOP);
                }

                else if(Mode == MENU.CIRCLE_TANGENT_LINE && ModeStep == 0)
                {
                    MakeCircleTangentLine(MOP);
                }
                else if (Mode == MENU.CIRCLE_TANGENT_CIRCLE)
                {
                    MakeCircleTangentCircle(MOP);
                }
            }
            else if (3000 <= MOP && MOP < 4000)
            {//モジュール(角度)をクリック
                if (Mode == MENU.BISECTOR)
                {
                    MakeBisector(MOP);
                }
            }
        }
        else 
        {//　ドラッグののちマウスアップ
            DraggedPointId = -1;
        }
    }


}
