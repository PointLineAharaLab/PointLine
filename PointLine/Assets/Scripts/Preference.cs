using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Preference : MonoBehaviour
{

    public bool show = true;
    public Vector3 Position;
    public Vector3 ScreenPosition;

    public GUIStyle WindowStyle;
    public GUIStyle LabelStyle;
    public GUIStyle FieldStyle;
    public GUIStyle ButtonStyle;
    public float WorldHeight = 5;
    public int MaxFontSize = 20;
    float DialogWidth;
    public string ObjectType = "";
    public string ObjectName = "";
    public string LogEnglishComment = "";
    public string CoordX = "", CoordY = "";
    string AngleConstant = "";
    public bool Fixed = false;
    public bool Delete = false;
    public bool ShowName = true;
    public bool ShowConstant = false;
    public Log LogParent = null;
    public GameObject Parent = null;

    // Start is called before the first frame update
    void Start()
    {
        float PreferenceX = -(WorldHeight / Screen.height * Screen.width) + 1.5f;
        MaxFontSize = (int)Mathf.Floor(Screen.width / 45);
        WindowStyle.fontSize = MaxFontSize;
        FieldStyle.fontSize = MaxFontSize;
        LabelStyle.fontSize = MaxFontSize;
        ButtonStyle.fontSize = MaxFontSize;
        　　//ここでフォントサイズを決められる
        Position = new Vector3(PreferenceX, 1.77f, -2f);
        GetScreenPosition(Position, out ScreenPosition);
    }

    public void SetData(Log lg)
    {
        ObjectType = lg.ObjectType;
        LogParent = lg;
        ObjectName = lg.PName;
        if (ObjectType == "Point")
        {
            CoordX = "" + Mathf.Round(1000f * lg.parent.GetComponent<Point>().Vec.x) / 1000f;
            CoordY = "" + Mathf.Round(1000f * lg.parent.GetComponent<Point>().Vec.y) / 1000f;
            Fixed = lg.parent.GetComponent<Point>().Fixed;
            ShowName = lg.parent.GetComponent<Point>().ShowPointName;
        }
        else if (ObjectType == "Line")
        {
        }
        else if (ObjectType == "Circle")
        {
            CoordX = "" + Mathf.Round(1000f * lg.parent.GetComponent<Circle>().Radius) / 1000f;
        }
        else if (ObjectType == "Module")
        {
            if (ObjectName == "中点" || ObjectName == "等長")
            {
                CoordX = "" + Mathf.Round(10f * lg.parent.GetComponent<Module>().Ratio1) / 10f;
                CoordY = "" + Mathf.Round(10f * lg.parent.GetComponent<Module>().Ratio2) / 10f;
            }
            else if (ObjectName == "角度")
            {
                Fixed = lg.parent.GetComponent<Module>().FixAngle;
                AngleConstant = "" + Mathf.Round(10f * lg.parent.GetComponent<Module>().Constant * 180f / Mathf.PI) / 10f;
            }
        }
    }

    float rate = 1f;
    void GetScreenPosition(Vector3 p, out Vector3 q)
    {
        rate = Screen.height / WorldHeight / 2f;
        //print(ClickOnPanel.WorldHeight);
        float WorldWidth = Screen.width / rate / 2;
        q.x = ((p.x - 1.5f) + WorldWidth) * rate;
        q.y = (-(p.y + 2.25f) + WorldHeight) * rate;
        q.z = 0f;
        DialogWidth = 3f * rate;
    }

    private void OnGUI()
    {
        if (show)
        {
            GUI.Window(0, new Rect(ScreenPosition.x, ScreenPosition.y, DialogWidth + MaxFontSize * 1f, MaxFontSize * 15f), WindowProc, "", WindowStyle);
        }
    }

    /// <summary>
    /// 設定ダイアログの内容設定
    /// </summary>
    /// <param name="id"></param>
    private void WindowProc(int id)
    {
        Vector3 Pos = Position;
        GetScreenPosition(Position, out ScreenPosition);

        transform.position = Pos;
        if (show)
        {
            float Left = MaxFontSize * 0.5f;
            float Top = MaxFontSize * 0.5f;
            float Step = MaxFontSize * 1.5f;
            float height = MaxFontSize * 1.5f;
            if (ObjectType == "Point")//点に関するプリファレンス
            {
                if (AppMgr.Japanese == 1)
                {// Japanese
                    PointPreferenceJapanese(Left, Top, Step, height);
                }
                else//English
                {
                    PointPreferenceEnglish(Left, Top, Step, height);
                }
            }
            else if (ObjectType == "Line")
            {
                if (AppMgr.Japanese == 1)
                {// Japanese
                    LinePreferenceJapanese(Left, Top, Step, height);
                }
                else//English
                {
                    LinePreferenceEnglish(Left, Top, Step, height);
                }
            }
            else if (ObjectType == "Circle")
            {
                if (AppMgr.Japanese == 1)
                {// Japanese
                    CirclePreferenceJapanese(Left, Top, Step, height);
                }
                else//English
                {
                    CirclePreferenceEnglish(Left, Top, Step, height);
                }
            }
            else if (ObjectType == "Module")
            {
                if (ObjectName == "中点")
                {
                    if (AppMgr.Japanese == 1)
                    {
                        ModuleMidpointPreferenceJapanese(Left, Top, Step, height);
                    }
                    else
                    {
                        ModuleMidpointPreferenceEnglish(Left, Top, Step, height);
                    }
                }
                else if (ObjectName == "等長")
                {
                    if (AppMgr.Japanese == 1)
                    {
                        ModuleIsometryPreferenceJapanese(Left, Top, Step, height);
                    }
                    else
                    {
                        ModuleIsometryPreferenceEnglish(Left, Top, Step, height);
                    }
                }
                else if (ObjectName == "角度")
                {
                    if (AppMgr.Japanese == 1)
                    {
                        ModuleAnglePreferenceJapanese(Left, Top, Step, height);
                    }
                    else
                    {
                        ModuleAnglePreferenceEnglish(Left, Top, Step, height);
                    }
                }
                else 
                {
                    if (AppMgr.Japanese == 1)
                    {
                        ModulePreferenceJapanese(Left, Top, Step, height);
                    }
                    else
                    {
                        ModulePreferenceEnglish(Left, Top, Step, height);
                    }
                }
            }

        }
    }

    public void EnterKeyDownProc()
    {
        if (ObjectType == "Point")
        {
            show = false;
            Point pt = LogParent.parent.GetComponent<Point>();
            pt.Fixed = Fixed;
            pt.Vec = new Vector3(float.Parse(CoordX), float.Parse(CoordY), 0f);
            pt.PointName = ObjectName;

        }
        else if (ObjectType == "Module")
        {
            show = false;
            if (ObjectName == "中点")
            {
                Module md = LogParent.parent.GetComponent<Module>();
                md.Ratio1 = float.Parse(CoordX);
                md.Ratio2 = float.Parse(CoordX);
            }
        }
    }
    public void SetScreenPosition()
    {
        GetScreenPosition(Position, out ScreenPosition);
    }

    /// <summary>
    /// ClickOnPnael.DeleteAPointのコピー
    /// </summary>
    /// <param name="MOP"></param>
    private void DeleteAPoint(int MOP)
    {
        GameObject[] gp = FindObjectsOfType<GameObject>();
        if (gp != null)
        {
            for (int i = 0; i < gp.Length; ++i)
            {
                Module md = gp[i].GetComponent<Module>();
                if (md != null)
                {
                    if (md.Object1Id == MOP || md.Object2Id == MOP || md.Object3Id == MOP)
                    {
                        // モジュールを消去する際に，「角度マーク」が関連していたらそれも消去する。
                        if (md.Type == MENU.ANGLE)//角度モジュールの場合
                        {
                            GameObject[] gp3 = FindObjectsOfType<GameObject>();
                            if (gp3 != null)
                            {
                                for (int k = 0; k < gp3.Length; k++)
                                {
                                    AngleMark am = (AngleMark)gp3[k].GetComponent("AngleMark");
                                    if (am != null)
                                    {
                                        if (am.Object1Id == MOP || am.Object2Id == MOP || am.Object3Id == MOP)
                                        {
                                            Destroy(gp3[k]);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        Util.DeleteLogAtID(md.Id);//ログの消去
                        Destroy(gp[i]);//モジュールの消去

                        AppMgr.mds = FindObjectsOfType<Module>();
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
                                        AppMgr.mds = FindObjectsOfType<Module>();
                                        break;
                                    }

                                }

                            }
                            Util.DeleteLogAtID(ci.Id);//ログの消去
                            Destroy(gp[i]);//円の消去
                            AppMgr.cis = FindObjectsOfType<Circle>();
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
                                        AppMgr.mds = FindObjectsOfType<Module>();
                                        break;
                                    }
                                }
                            }
                            Util.DeleteLogAtID(ln.Id);//ログの消去
                            Destroy(gp[i]);//直線の消去
                            AppMgr.lns = FindObjectsOfType<Line>();
                        }
                    }
                }
                Point obj = gp[i].GetComponent<Point>();
                if (obj != null)
                {
                    if (obj.Id == MOP)
                    {
                        GameObject[] gp2 = FindObjectsOfType<GameObject>();
                        if (gp2 != null)
                        {
                            for (int j = 0; j < gp2.Length; ++j)
                            {
                                Module md2 = gp2[j].GetComponent<Module>();
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
                                                    ThinLine TL = gp3[k].GetComponent<ThinLine>();
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
        AppMgr.Mode = MENU.ADD_POINT;
        AppMgr.ModeStep = 0;
    }

    private void DeleteALine(int MOP)
    {
        GameObject[] gp = FindObjectsOfType<GameObject>();
        if (gp != null)
        {
            for (int i = 0; i < gp.Length; ++i)
            {
                Module md = gp[i].GetComponent<Module>();
                if (md != null)
                {
                    if (md.Object1Id == MOP || md.Object2Id == MOP || md.Object3Id == MOP)
                    {
                        Util.DeleteLogAtID(md.Id);//ログの消去
                        Destroy(gp[i]);//モジュールの消去

                        AppMgr.mds = FindObjectsOfType<Module>();
                    }
                }
                Line obj = gp[i].GetComponent<Line>();
                if (obj != null)
                {
                    if (obj.Id == MOP)
                    {
                        Util.DeleteLogAtID(MOP);//ログの消去
                        Destroy(gp[i], 0.5f);//モジュールの消去

                    }
                }
            }
        }
        AppMgr.Mode = MENU.ADD_POINT;
        AppMgr.ModeStep = 0;
    }

    private void DeleteACircle(int MOP)
    {
        GameObject[] gp = FindObjectsOfType<GameObject>();
        if (gp != null)
        {
            for (int i = 0; i < gp.Length; ++i)
            {
                Module md = gp[i].GetComponent<Module>();
                if (md != null)
                {
                    if (md.Object1Id == MOP || md.Object2Id == MOP || md.Object3Id == MOP)
                    {
                        Util.DeleteLogAtID(md.Id);//ログの消去
                        Destroy(gp[i]);//モジュールの消去

                        AppMgr.mds = FindObjectsOfType<Module>();
                    }
                }
                Circle obj = gp[i].GetComponent<Circle>();
                if (obj != null)
                {
                    if (obj.Id == MOP)
                    {
                        Util.DeleteLogAtID(MOP);//ログの消去
                        Destroy(gp[i], 0.5f);//モジュールの消去

                    }
                }
            }
        }
        AppMgr.Mode = MENU.ADD_POINT;
        AppMgr.ModeStep = 0;
    }

    /// <summary>
    /// モジュールを消す。ClickOnPanelにあるべきものだと思うが、あっちでは呼び出すUIが作れないので、こちらに置いておく。
    /// </summary>
    /// <param name="MOP"></param>
    private void DeleteAModule(int MOP)
    {
        GameObject[] OBJs = FindObjectsOfType<GameObject>();
        if (OBJs != null)
        {
            for (int i = 0; i < OBJs.Length; ++i)
            {
                Module md = OBJs[i].GetComponent<Module>();
                if (md != null)
                {
                    if (md.Id == MOP)
                    {
                        //Debug.Log(md.Type);
                        // モジュールを消去する際に，「細線補助線」が関連していたらそれも消去する。
                        if (md.Type == MENU.POINT_ON_LINE)//直交させるモジュールの場合
                        {
                            ThinLine[] TLs = FindObjectsOfType<ThinLine>();
                            for (int j = 0; j < TLs.Length; j++)
                            {
                                Debug.Log(TLs[j].PointId + "," + md.Object1Id + "," + TLs[j].LineId + "," + md.Object2Id);
                                if (TLs[j].PointId == md.Object1Id && TLs[j].LineId == md.Object2Id)
                                {
                                    Destroy(TLs[j].gameObject);
                                }
                            }
                        }
                        // モジュールを消去する際に，「直角補助」が関連していたらそれも消去する。
                        else if (md.Type == MENU.LINES_PERPENDICULAR)//直交させるモジュールの場合
                        {
                            AngleMark[] AMs = FindObjectsOfType<AngleMark>();
                            for (int j = 0; j < AMs.Length; j++)
                            {
                                if (AMs[j].Object1Id == md.Object1Id && AMs[j].Object2Id == md.Object2Id)// 逆かも。
                                {
                                    Destroy(AMs[j].gameObject);
                                }
                            }
                        }
                        // モジュールを消去する際に，「角」が関連していたらそれも消去する。
                        else if (md.Type == MENU.ANGLE)//角度モジュールの場合
                        {
                            AngleMark[] AMs = FindObjectsOfType<AngleMark>();
                            for (int j = 0; j < AMs.Length; j++)
                            {
                                if (AMs[j].Object1Id == md.Object1Id && AMs[j].Object2Id == md.Object2Id && AMs[j].Object3Id == md.Object3Id)//順番が違うかも。
                                {
                                    Destroy(AMs[j].gameObject);
                                }
                            }
                        }
                        // 等長の時には、色も消す
                        else if (md.Type == MENU.LINES_ISOMETRY)
                        {
                            Line[] LNs = FindObjectsOfType<Line>();
                            for (int j = 0; j < LNs.Length; j++)
                            {
                                if (LNs[j].Id == md.Object1Id || LNs[j].Id == md.Object2Id)
                                {
                                    LNs[j].Isometry = -1;
                                }
                            }
                        }
                        Destroy(md.gameObject);//モジュールの消去
                        Util.DeleteLogAtID(MOP);//ログの消去
                    }
                }
            }
        }
        AppMgr.Mode = MENU.ADD_POINT;
        AppMgr.ModeStep = 0;
    }


    void PointPreferenceJapanese(float Left, float Top, float Step, float height)
    {
        GUI.Label(new Rect(Left, Top, DialogWidth, height), "点 " + ObjectName, LabelStyle);
        Top += Step;
        GUI.Label(new Rect(Left, Top, DialogWidth, height), "名称 ", LabelStyle);
        ObjectName = GUI.TextField(new Rect(Left + MaxFontSize * 2.5f, Top, DialogWidth - MaxFontSize * 2.5f, height), ObjectName, FieldStyle);
        Top += Step;
        GUIStyle BS = new GUIStyle(ButtonStyle);
        if (ShowName)
        {
            BS.fontSize = Mathf.FloorToInt(DialogWidth / 8);
            if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
            GUI.Label(new Rect(Left, Top, DialogWidth, height), "表示", LabelStyle);
            if (GUI.Button(new Rect(Left + DialogWidth *0.5f, Top, DialogWidth *0.5f, height), "非表示", BS))
            {
                ShowName = false;
            }
            Top += Step;
        }
        else
        {
            BS.fontSize = Mathf.FloorToInt(DialogWidth / 6);
            if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
            GUI.Label(new Rect(Left, Top, DialogWidth, height), "非表示", LabelStyle);
            if (GUI.Button(new Rect(Left + DialogWidth *0.5f, Top, DialogWidth *0.5f, height), "表示", BS))
            {
                ShowName = true;
            }
            Top += Step;
        }
        GUI.Label(new Rect(Left, Top, DialogWidth, height), "(" + CoordX + "," + CoordY + ")", LabelStyle);
        Top += Step;
        GUI.Label(new Rect(Left, Top, DialogWidth, height), "X: ", LabelStyle);
        CoordX = GUI.TextField(new Rect(Left + MaxFontSize * 1.5f, Top, DialogWidth - MaxFontSize * 1.5f, height), CoordX, FieldStyle);
        Top += Step;
        GUI.Label(new Rect(Left, Top, DialogWidth, height), "Y: ", LabelStyle);
        CoordY = GUI.TextField(new Rect(Left + MaxFontSize * 1.5f, Top, DialogWidth - MaxFontSize * 1.5f, height), CoordY, FieldStyle);
        Top += Step;
        if (Fixed)
        {
            BS = new GUIStyle(ButtonStyle);
            BS.fontSize = Mathf.FloorToInt(DialogWidth / 12);
            if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
            GUI.Label(new Rect(Left, Top, DialogWidth, height), "固定", LabelStyle);
            if (GUI.Button(new Rect(Left + DialogWidth *0.5f, Top, DialogWidth *0.5f, height), "可動にする", BS))
            {
                Fixed = false;
            }
            Top += Step;
        }
        else
        {
            BS = new GUIStyle(ButtonStyle);
            BS.fontSize = Mathf.FloorToInt(DialogWidth / 12);
            if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
            GUI.Label(new Rect(Left, Top, DialogWidth, height), "可動", LabelStyle);
            if (GUI.Button(new Rect(Left + DialogWidth * 0.5f, Top, DialogWidth * 0.5f, height), "固定にする", BS))
            {
                Fixed = true;
            }
            Top += Step;
        }
        BS = new GUIStyle(ButtonStyle);
        BS.fontSize = Mathf.FloorToInt(DialogWidth / 6);
        if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
        if (GUI.Button(new Rect(Left, Top, DialogWidth, height), "消去", BS))
        {
            Point pt = LogParent.parent.GetComponent<Point>();
            DeleteAPoint(pt.Id);
            show = false;
        }
        Top += Step;
        BS = new GUIStyle(ButtonStyle);
        BS.fontSize = Mathf.FloorToInt(DialogWidth / 8);
        if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
        if (GUI.Button(new Rect(Left, Top, DialogWidth * 0.5f, height), "Cancel", BS))
        {
            show = false;
        }
        BS = new GUIStyle(ButtonStyle);
        BS.fontSize = Mathf.FloorToInt(DialogWidth / 4);
        if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
        if (GUI.Button(new Rect(Left + DialogWidth * 0.5f, Top, DialogWidth * 0.5f, height), "OK", BS))
        {
            show = false;
            Point pt = LogParent.parent.GetComponent<Point>();
            pt.Fixed = Fixed;
            pt.Vec = new Vector3(float.Parse(CoordX), float.Parse(CoordY), 0f);
            pt.PointName = ObjectName;
            pt.ShowPointName = ShowName;
        }
    }

    void PointPreferenceEnglish(float Left, float Top, float Step, float height)
    {
        GUIStyle LS = new GUIStyle(LabelStyle);
        GUI.Label(new Rect(Left, Top, DialogWidth, height), "Point " + ObjectName, LabelStyle);
        Top += Step;
        GUI.Label(new Rect(Left, Top, DialogWidth, height), "Name ", LabelStyle);
        ObjectName = GUI.TextField(new Rect(Left + MaxFontSize * 3f, Top, DialogWidth - MaxFontSize * 3f, height), ObjectName, FieldStyle);
        Top += Step;
        GUIStyle BS = new GUIStyle(ButtonStyle);
        if (ShowName)
        {
            BS = new GUIStyle(ButtonStyle);
            BS.fontSize = Mathf.FloorToInt(DialogWidth / 6);
            if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
            LS.fontSize = Mathf.FloorToInt(DialogWidth / 11);
            if (LS.fontSize > MaxFontSize) LS.fontSize = MaxFontSize;
            GUI.Label(new Rect(Left, Top, DialogWidth, height), "Show Name", LS);
            if (GUI.Button(new Rect(Left + DialogWidth / 2, Top, DialogWidth / 2, height), "Hide", BS))
            {
                ShowName = false;
            }
            Top += Step;
        }
        else
        {
            BS = new GUIStyle(ButtonStyle);
            BS.fontSize = Mathf.FloorToInt(DialogWidth / 6);
            if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
            LS.fontSize = Mathf.FloorToInt(DialogWidth / 11);
            if (LS.fontSize > MaxFontSize) LS.fontSize = MaxFontSize;
            GUI.Label(new Rect(Left, Top, DialogWidth, height), "Hide Name", LS);
            if (GUI.Button(new Rect(Left + DialogWidth / 2, Top, DialogWidth / 2, height), "Show", BS))
            {
                ShowName = true;
            }
            Top += Step;
        }
        GUI.Label(new Rect(Left, Top, DialogWidth, height), "(" + CoordX + "," + CoordY + ")", LabelStyle);
        Top += Step;
        GUI.Label(new Rect(Left, Top, DialogWidth, height), "X: ", LabelStyle);
        CoordX = GUI.TextField(new Rect(Left + MaxFontSize * 1.5f, Top, DialogWidth - MaxFontSize * 1.5f, height), CoordX, FieldStyle);
        Top += Step;
        GUI.Label(new Rect(Left, Top, DialogWidth, height), "Y: ", LabelStyle);
        CoordY = GUI.TextField(new Rect(Left + MaxFontSize * 1.5f, Top, DialogWidth - MaxFontSize * 1.5f, height), CoordY, FieldStyle);
        Top += Step;
        if (Fixed)
        {
            BS = new GUIStyle(ButtonStyle);
            BS.fontSize = Mathf.FloorToInt(DialogWidth / 6);
            if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
            GUI.Label(new Rect(Left, Top, DialogWidth, height), "Fixed", LabelStyle);
            if (GUI.Button(new Rect(Left + DialogWidth / 2, Top, DialogWidth / 2, height), "Free", BS))
            {
                Fixed = false;
            }
            Top += Step;
        }
        else
        {
            BS = new GUIStyle(ButtonStyle);
            BS.fontSize = Mathf.FloorToInt(DialogWidth / 7);
            if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
            GUI.Label(new Rect(Left, Top, DialogWidth, height), "Free", LabelStyle);
            if (GUI.Button(new Rect(Left + DialogWidth / 2, Top, DialogWidth / 2, height), "Fixed", BS))
            {
                Fixed = true;
            }
            Top += Step;
        }
        BS = new GUIStyle(ButtonStyle);
        BS.fontSize = Mathf.FloorToInt(DialogWidth / 8);
        if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
        if (GUI.Button(new Rect(Left, Top, DialogWidth, height), "Delete", BS))
        {
            Point pt = LogParent.parent.GetComponent<Point>();
            DeleteAPoint(pt.Id);
            show = false;
        }
        Top += Step;
        BS = new GUIStyle(ButtonStyle);
        BS.fontSize = Mathf.FloorToInt(DialogWidth / 8);
        if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
        if (GUI.Button(new Rect(Left, Top, DialogWidth / 2, height), "Cancel", BS))
        {
            show = false;
        }
        BS = new GUIStyle(ButtonStyle);
        BS.fontSize = Mathf.FloorToInt(DialogWidth / 4);
        if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
        if (GUI.Button(new Rect(Left + DialogWidth / 2, Top, DialogWidth / 2, height), "OK", BS))
        {
            show = false;
            Point pt = LogParent.parent.GetComponent<Point>();
            pt.Fixed = Fixed;
            pt.Vec = new Vector3(float.Parse(CoordX), float.Parse(CoordY), 0f);
            pt.PointName = ObjectName;
            pt.ShowPointName = ShowName;
        }
    }

    void LinePreferenceJapanese(float Left, float Top, float Step, float height)
    {
        Point pt1 = LogParent.GetComponent<Log>().Object1.GetComponent<Point>();
        Point pt2 = LogParent.GetComponent<Log>().Object2.GetComponent<Point>();
        float dist = (pt1.Vec - pt2.Vec).magnitude;
        GUI.Label(new Rect(Left, Top, DialogWidth, height), "線 " + ObjectName, LabelStyle);
        Top += Step;
        GUI.Label(new Rect(Left, Top, DialogWidth, height), LogParent.GetComponent<Log>().Text2, LabelStyle);
        Top += Step;
        GUI.Label(new Rect(Left, Top, DialogWidth, height), "辺長 " + Mathf.Round(dist * 1000f) / 1000f, LabelStyle);
        Top += Step;
        GUIStyle BS = new GUIStyle(ButtonStyle);
        BS.fontSize = Mathf.FloorToInt(DialogWidth / 6);
        if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
        if (GUI.Button(new Rect(Left, Top, DialogWidth, height), "消去", BS))
        {
            Line ln = LogParent.parent.GetComponent<Line>();
            DeleteALine(ln.Id);
            show = false;
        }
        Top += Step;
        BS = new GUIStyle(ButtonStyle);
        BS.fontSize = Mathf.FloorToInt(DialogWidth / 8);
        if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
        if (GUI.Button(new Rect(Left, Top, DialogWidth / 2, height), "Cancel", BS))
        {
            show = false;
        }
        BS = new GUIStyle(ButtonStyle);
        BS.fontSize = Mathf.FloorToInt(DialogWidth / 6);
        if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
        if (GUI.Button(new Rect(Left + DialogWidth / 2, Top, DialogWidth / 2, height), "OK", BS))
        {
            show = false;
        }
    }

    void LinePreferenceEnglish(float Left, float Top, float Step, float height)
    {
        Point pt1 = LogParent.GetComponent<Log>().Object1.GetComponent<Point>();
        Point pt2 = LogParent.GetComponent<Log>().Object2.GetComponent<Point>();
        float dist = (pt1.Vec - pt2.Vec).magnitude;
        GUI.Label(new Rect(Left, Top, DialogWidth, height), "Line " + ObjectName, LabelStyle);
        Top += Step;
        GUI.Label(new Rect(Left, Top, DialogWidth, height), LogParent.GetComponent<Log>().Text2, LabelStyle);
        Top += Step;
        GUI.Label(new Rect(Left, Top, DialogWidth, height), "Length " + Mathf.Round(dist * 1000f) / 1000f, LabelStyle);
        Top += Step;
        GUIStyle BS = new GUIStyle(ButtonStyle);
        BS.fontSize = Mathf.FloorToInt(DialogWidth / 8);
        if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
        if (GUI.Button(new Rect(Left, Top, DialogWidth, height), "Delete", BS))
        {
            Line ln = LogParent.parent.GetComponent<Line>();
            DeleteALine(ln.Id);
            show = false;
        }
        Top += Step;
        BS = new GUIStyle(ButtonStyle);
        BS.fontSize = Mathf.FloorToInt(DialogWidth / 8);
        if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
        if (GUI.Button(new Rect(Left, Top, DialogWidth / 2, height), "Cancel", BS))
        {
            show = false;
        }
        BS = new GUIStyle(ButtonStyle);
        BS.fontSize = Mathf.FloorToInt(DialogWidth / 4);
        if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
        if (GUI.Button(new Rect(Left + DialogWidth / 2, Top, DialogWidth / 2, height), "OK", BS))
        {
            show = false;
        }
    }
    void CirclePreferenceJapanese(float Left, float Top, float Step, float height)
    {
        GUI.Label(new Rect(Left, Top, DialogWidth, height), "円 " + ObjectName, LabelStyle);
        Top += Step;
        GUI.Label(new Rect(Left, Top, DialogWidth, height), LogParent.GetComponent<Log>().Text2, LabelStyle);
        Top += Step;
        GUIStyle FS = new GUIStyle(FieldStyle);
        FS.fontSize = Mathf.FloorToInt(DialogWidth / 2 / 4);
        if (FS.fontSize > MaxFontSize) FS.fontSize = MaxFontSize;
        GUI.Label(new Rect(Left, Top, DialogWidth, height), "半径", LabelStyle);
        CoordX = GUI.TextField(new Rect(Left + DialogWidth / 2, Top, DialogWidth / 2, height), CoordX, FS);
        Top += Step;
        GUIStyle BS = new GUIStyle(ButtonStyle);
        BS.fontSize = Mathf.FloorToInt(DialogWidth / 6);
        if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
        if (GUI.Button(new Rect(Left, Top, DialogWidth, height), "消去", BS))
        {
            Circle ci = LogParent.parent.GetComponent<Circle>();
            DeleteACircle(ci.Id);
            show = false;
        }
        Top += Step;
        BS = new GUIStyle(ButtonStyle);
        BS.fontSize = Mathf.FloorToInt(DialogWidth / 8);
        if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
        if (GUI.Button(new Rect(Left, Top, DialogWidth / 2, height), "Cancel", BS))
        {
            show = false;
        }
        BS = new GUIStyle(ButtonStyle);
        BS.fontSize = Mathf.FloorToInt(DialogWidth / 4);
        if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
        if (GUI.Button(new Rect(Left + DialogWidth / 2, Top, DialogWidth / 2, height), "OK", BS))
        {
            Circle ci = LogParent.parent.GetComponent<Circle>();
            ci.Radius = float.Parse(CoordX);
            show = false;
        }
    }

    void CirclePreferenceEnglish(float Left, float Top, float Step, float height)
    {
        GUI.Label(new Rect(Left, Top, DialogWidth, height), "Circle " + ObjectName, LabelStyle);
        Top += Step;
        GUI.Label(new Rect(Left, Top, DialogWidth, height), "Center " + LogParent.Object1.GetComponent<Point>().PointName, LabelStyle);
        Top += Step;
        GUIStyle LS = new GUIStyle(LabelStyle);
        LS.fontSize = Mathf.FloorToInt(DialogWidth / 3 / 3.5f);
        if (LS.fontSize > MaxFontSize) LS.fontSize = MaxFontSize;
        GUIStyle FS = new GUIStyle(FieldStyle);
        FS.fontSize = Mathf.FloorToInt(DialogWidth * 2 / 3 / 4);
        if (FS.fontSize > MaxFontSize) FS.fontSize = MaxFontSize;
        GUI.Label(new Rect(Left, Top, DialogWidth / 3, height), "Radius", LS);
        CoordX = GUI.TextField(new Rect(Left + DialogWidth / 3, Top, DialogWidth * 2 / 3, height), CoordX, FS);
        Top += Step;
        GUIStyle BS = new GUIStyle(ButtonStyle);
        BS.fontSize = Mathf.FloorToInt(DialogWidth / 8);
        if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
        if (GUI.Button(new Rect(Left, Top, DialogWidth, height), "Delete", BS))
        {
            Circle ci = LogParent.parent.GetComponent<Circle>();
            DeleteACircle(ci.Id);
            show = false;
        }
        Top += Step;
        BS = new GUIStyle(ButtonStyle);
        BS.fontSize = Mathf.FloorToInt(DialogWidth / 8);
        if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
        if (GUI.Button(new Rect(Left, Top, DialogWidth / 2, height), "Cancel", BS))
        {
            show = false;
        }
        BS = new GUIStyle(ButtonStyle);
        BS.fontSize = Mathf.FloorToInt(DialogWidth / 4);
        if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
        if (GUI.Button(new Rect(Left + DialogWidth / 2, Top, DialogWidth / 2, height), "OK", BS))
        {
            Circle ci = LogParent.parent.GetComponent<Circle>();
            ci.Radius = float.Parse(CoordX);
            show = false;
        }
    }

    void ModuleMidpointPreferenceJapanese(float Left, float Top, float Step, float height)
    {
        GUI.Label(new Rect(Left, Top, DialogWidth, height), ObjectName, LabelStyle);
        Top += Step;
        GUI.Label(new Rect(Left, Top, DialogWidth, height), LogParent.GetComponent<Log>().Text2, LabelStyle);
        Top += Step;
        GUI.Label(new Rect(Left, Top, DialogWidth, height), "内分比(" + CoordX + " : " + CoordY + ")", LabelStyle);
        Top += Step;
        GUI.Label(new Rect(Left, Top, DialogWidth, height), "比1", LabelStyle);
        CoordX = GUI.TextField(new Rect(Left + 40, Top, DialogWidth - 40, height), CoordX, FieldStyle);
        Top += Step;
        GUI.Label(new Rect(Left, Top, DialogWidth, height), "比2", LabelStyle);
        CoordY = GUI.TextField(new Rect(Left + 40, Top, DialogWidth - 40, height), CoordY, FieldStyle);
        Top += Step;
        GUIStyle BS = new GUIStyle(ButtonStyle);
        BS.fontSize = Mathf.FloorToInt(DialogWidth / 6);
        if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
        if (GUI.Button(new Rect(Left, Top, DialogWidth, height), "消去", BS))
        {
            Module md = LogParent.parent.GetComponent<Module>();
            DeleteAModule(md.Id);
            show = false;
        }
        Top += Step;
        BS = new GUIStyle(ButtonStyle);
        BS.fontSize = Mathf.FloorToInt(DialogWidth / 8);
        if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
        if (GUI.Button(new Rect(Left, Top, DialogWidth / 2, height), "Cancel", BS))
        {
            show = false;
        }
        BS = new GUIStyle(ButtonStyle);
        BS.fontSize = Mathf.FloorToInt(DialogWidth / 4);
        if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
        if (GUI.Button(new Rect(Left + DialogWidth / 2, Top, DialogWidth / 2, height), "OK", BS))
        {
            show = false;
            Module md = LogParent.parent.GetComponent<Module>();
            md.Ratio1 = float.Parse(CoordX);
            md.Ratio2 = float.Parse(CoordY);
        }
    }
    void ModuleMidpointPreferenceEnglish(float Left, float Top, float Step, float height)
    {
        if(CoordX == CoordY)
            GUI.Label(new Rect(Left, Top, DialogWidth, height), "Midpoint", LabelStyle);
        else
            GUI.Label(new Rect(Left, Top, DialogWidth, height), "Div point", LabelStyle);
        Top += Step;
        GUI.Label(new Rect(Left, Top, DialogWidth, height), LogParent.GetComponent<Log>().Text2, LabelStyle);
        Top += Step;
        GUI.Label(new Rect(Left, Top, DialogWidth, height), "Ratio(" + CoordX + ":" + CoordY + ")", LabelStyle);
        Top += Step;
        GUI.Label(new Rect(Left, Top, DialogWidth, height), "Ratio 1", LabelStyle);
        CoordX = GUI.TextField(new Rect(Left + 70, Top, DialogWidth - 70, height), CoordX, FieldStyle);
        Top += Step;
        GUI.Label(new Rect(Left, Top, DialogWidth, height), "Ratio 2", LabelStyle);
        CoordY = GUI.TextField(new Rect(Left + 70, Top, DialogWidth - 70, height), CoordY, FieldStyle);
        Top += Step;
        GUIStyle BS = new GUIStyle(ButtonStyle);
        BS.fontSize = Mathf.FloorToInt(DialogWidth / 8);
        if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
        if (GUI.Button(new Rect(Left, Top, DialogWidth, height), "Delete", BS))
        {
            Module md = LogParent.parent.GetComponent<Module>();
            DeleteAModule(md.Id);
            show = false;
        }
        Top += Step;
        BS = new GUIStyle(ButtonStyle);
        BS.fontSize = Mathf.FloorToInt(DialogWidth / 8);
        if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
        if (GUI.Button(new Rect(Left, Top, DialogWidth / 2, height), "Cancel", BS))
        {
            show = false;
        }
        BS = new GUIStyle(ButtonStyle);
        BS.fontSize = Mathf.FloorToInt(DialogWidth / 4);
        if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
        if (GUI.Button(new Rect(Left + DialogWidth / 2, Top, DialogWidth / 2, height), "OK", BS))
        {
            show = false;
            Module md = LogParent.parent.GetComponent<Module>();
            md.Ratio1 = float.Parse(CoordX);
            md.Ratio2 = float.Parse(CoordY);
        }
    }

    void ModuleIsometryPreferenceJapanese(float Left, float Top, float Step, float height)
    {
        GUI.Label(new Rect(Left, Top, DialogWidth, height), ObjectName, LabelStyle);
        Top += Step;
        GUI.Label(new Rect(Left, Top, DialogWidth, height), LogParent.GetComponent<Log>().Text2, LabelStyle);
        Top += Step;
        GUI.Label(new Rect(Left, Top, DialogWidth, height), "線分比(" + CoordX + " : " + CoordY + ")", LabelStyle);
        Top += Step;
        GUI.Label(new Rect(Left, Top, DialogWidth, height), "比1", LabelStyle);
        CoordX = GUI.TextField(new Rect(Left + 40, Top, DialogWidth - 40, height), CoordX, FieldStyle);
        Top += Step;
        GUI.Label(new Rect(Left, Top, DialogWidth, height), "比2", LabelStyle);
        CoordY = GUI.TextField(new Rect(Left + 40, Top, DialogWidth - 40, height), CoordY, FieldStyle);
        Top += Step;
        GUIStyle BS = new GUIStyle(ButtonStyle);
        BS.fontSize = Mathf.FloorToInt(DialogWidth / 6);
        if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
        if (GUI.Button(new Rect(Left, Top, DialogWidth, height), "消去", BS))
        {
            Module md = LogParent.parent.GetComponent<Module>();
            DeleteAModule(md.Id);
            show = false;
        }
        Top += Step;
        BS = new GUIStyle(ButtonStyle);
        BS.fontSize = Mathf.FloorToInt(DialogWidth / 8);
        if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
        if (GUI.Button(new Rect(Left, Top, DialogWidth / 2, height), "Cancel", BS))
        {
            show = false;
        }
        BS = new GUIStyle(ButtonStyle);
        BS.fontSize = Mathf.FloorToInt(DialogWidth / 4);
        if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
        if (GUI.Button(new Rect(Left + DialogWidth / 2, Top, DialogWidth / 2, height), "OK", BS))
        {
            show = false;
            Module md = LogParent.parent.GetComponent<Module>();
            md.Ratio1 = float.Parse(CoordX);
            md.Ratio2 = float.Parse(CoordY);
        }
    }

    void ModuleIsometryPreferenceEnglish(float Left, float Top, float Step, float height)
    {
        if (CoordX == CoordY)
            GUI.Label(new Rect(Left, Top, DialogWidth, height), "Isometric", LabelStyle);
        else
            GUI.Label(new Rect(Left, Top, DialogWidth, height), "segment ratio", LabelStyle);
        Top += Step;
        GUI.Label(new Rect(Left, Top, DialogWidth, height), LogParent.GetComponent<Log>().Text2, LabelStyle);
        Top += Step;
        GUI.Label(new Rect(Left, Top, DialogWidth, height), "Ratio(" + CoordX + ":" + CoordY + ")", LabelStyle);
        Top += Step;
        GUI.Label(new Rect(Left, Top, DialogWidth, height), "Ratio 1", LabelStyle);
        CoordX = GUI.TextField(new Rect(Left + 70, Top, DialogWidth - 70, height), CoordX, FieldStyle);
        Top += Step;
        GUI.Label(new Rect(Left, Top, DialogWidth, height), "Ratio 2", LabelStyle);
        CoordY = GUI.TextField(new Rect(Left + 70, Top, DialogWidth - 70, height), CoordY, FieldStyle);
        Top += Step;
        GUIStyle BS = new GUIStyle(ButtonStyle);
        BS.fontSize = Mathf.FloorToInt(DialogWidth / 8);
        if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
        if (GUI.Button(new Rect(Left, Top, DialogWidth, height), "Delete", BS))
        {
            Module md = LogParent.parent.GetComponent<Module>();
            DeleteAModule(md.Id);
            show = false;
        }
        Top += Step;
        BS = new GUIStyle(ButtonStyle);
        BS.fontSize = Mathf.FloorToInt(DialogWidth / 8);
        if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
        if (GUI.Button(new Rect(Left, Top, DialogWidth / 2, height), "Cancel", BS))
        {
            show = false;
        }
        BS = new GUIStyle(ButtonStyle);
        BS.fontSize = Mathf.FloorToInt(DialogWidth / 4);
        if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
        if (GUI.Button(new Rect(Left + DialogWidth / 2, Top, DialogWidth / 2, height), "OK", BS))
        {
            show = false;
            Module md = LogParent.parent.GetComponent<Module>();
            md.Ratio1 = float.Parse(CoordX);
            md.Ratio2 = float.Parse(CoordY);
        }
    }

    void ModuleAnglePreferenceJapanese(float Left, float Top, float Step, float height)
    {
        GUI.Label(new Rect(Left, Top, DialogWidth, height), ObjectName, LabelStyle);
        Top += Step;
        GUI.Label(new Rect(Left, Top, DialogWidth, height), LogParent.GetComponent<Log>().Text2, LabelStyle);
        Top += Step;
        AngleConstant = GUI.TextField(new Rect(Left, Top, DialogWidth - 40, height), AngleConstant, FieldStyle);
        GUI.Label(new Rect(DialogWidth - 20, Top, DialogWidth, height), "度", LabelStyle);
        Top += Step;
        GUIStyle BS = new GUIStyle(ButtonStyle);
        if (ShowConstant)
        {
            BS.fontSize = Mathf.FloorToInt(DialogWidth / 14);
            if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
            GUI.Label(new Rect(Left, Top, DialogWidth, height), "表示", LabelStyle);
            if (GUI.Button(new Rect(Left + DialogWidth / 2, Top, DialogWidth / 2, height), "非表示にする", BS))
            {
                ShowConstant = false;
            }
        }
        else
        {
            BS = new GUIStyle(ButtonStyle);
            BS.fontSize = Mathf.FloorToInt(DialogWidth / 12);
            if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
            GUI.Label(new Rect(Left, Top, DialogWidth, height), "非表示", LabelStyle);
            if (GUI.Button(new Rect(Left + DialogWidth / 2, Top, DialogWidth / 2, height), "表示にする", BS))
            {
                ShowConstant = true;
            }
        }
        Top += Step;
        if (Fixed)
        {
            BS = new GUIStyle(ButtonStyle);
            BS.fontSize = Mathf.FloorToInt(DialogWidth / 12);
            if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
            GUI.Label(new Rect(Left, Top, DialogWidth, height), "固定", LabelStyle);
            if (GUI.Button(new Rect(Left + DialogWidth / 2, Top, DialogWidth / 2, height), "可動にする", BS))
            {
                Fixed = false;
            }
            Top += Step;
        }
        else
        {
            BS = new GUIStyle(ButtonStyle);
            BS.fontSize = Mathf.FloorToInt(DialogWidth / 12);
            if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
            GUI.Label(new Rect(Left, Top, DialogWidth, height), "可動", LabelStyle);
            if (GUI.Button(new Rect(Left + DialogWidth / 2, Top, DialogWidth / 2, height), "固定にする", BS))
            {
                Fixed = true;
            }
            Top += Step;
        }
        BS = new GUIStyle(ButtonStyle);
        BS.fontSize = Mathf.FloorToInt(DialogWidth / 4);
        if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
        if (GUI.Button(new Rect(Left, Top, DialogWidth, height), "消去", BS))
        {
            Module md = LogParent.parent.GetComponent<Module>();
            DeleteAModule(md.Id);
            show = false;
        }
        Top += Step;
        BS = new GUIStyle(ButtonStyle);
        BS.fontSize = Mathf.FloorToInt(DialogWidth / 8);
        if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
        if (GUI.Button(new Rect(Left, Top, DialogWidth / 2, height), "Cancel", BS))
        {
            show = false;
        }
        BS = new GUIStyle(ButtonStyle);
        BS.fontSize = Mathf.FloorToInt(DialogWidth / 4);
        if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
        if (GUI.Button(new Rect(Left + DialogWidth / 2, Top, DialogWidth / 2, height), "OK", BS))
        {
            show = false;
            Module md = LogParent.parent.GetComponent<Module>();
            md.Constant = float.Parse(AngleConstant) * Mathf.PI / 180f;
            md.FixAngle = Fixed;
            md.ShowConstant = ShowConstant;
        }

    }
    void ModuleAnglePreferenceEnglish(float Left, float Top, float Step, float height)
    {
        GUIStyle LS = new GUIStyle(LabelStyle);
        GUI.Label(new Rect(Left, Top, DialogWidth, height), " Angle ", LabelStyle);
        Top += Step;
        GUI.Label(new Rect(Left, Top, DialogWidth, height), LogParent.GetComponent<Log>().Text2, LabelStyle);
        Top += Step;
        GUIStyle FS = new GUIStyle(FieldStyle);
        FS.fontSize = Mathf.FloorToInt(DialogWidth * 2 / 3 / 4);
        if (FS.fontSize > MaxFontSize) FS.fontSize = MaxFontSize;
        LS.fontSize = Mathf.FloorToInt(DialogWidth / 3 / 4);
        if (LS.fontSize > MaxFontSize) LS.fontSize = MaxFontSize;
        AngleConstant = GUI.TextField(new Rect(Left, Top, DialogWidth * 2 / 3, height), AngleConstant, FS);
        GUI.Label(new Rect(Left + DialogWidth * 2 / 3, Top, DialogWidth / 3, height), "Degree", LS);
        Top += Step;
        GUIStyle BS = new GUIStyle(ButtonStyle);
        if (ShowConstant)
        {
            BS.fontSize = Mathf.FloorToInt(DialogWidth / 6);
            if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
            LS.fontSize = Mathf.FloorToInt(DialogWidth / 6);
            if (LS.fontSize > MaxFontSize) LS.fontSize = MaxFontSize;
            GUI.Label(new Rect(Left, Top, DialogWidth, height), "Show", LS);
            if (GUI.Button(new Rect(Left + DialogWidth / 2, Top, DialogWidth / 2, height), "Hide", BS))
            {
                ShowConstant = false;
            }
        }
        else
        {
            BS = new GUIStyle(ButtonStyle);
            BS.fontSize = Mathf.FloorToInt(DialogWidth / 6);
            if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
            LS.fontSize = Mathf.FloorToInt(DialogWidth / 6);
            if (LS.fontSize > MaxFontSize) LS.fontSize = MaxFontSize;
            GUI.Label(new Rect(Left, Top, DialogWidth, height), "Hide", LS);
            if (GUI.Button(new Rect(Left + DialogWidth / 2, Top, DialogWidth / 2, height), "Show", BS))
            {
                ShowConstant = true;
            }
        }
        Top += Step;
        if (Fixed)
        {
            BS = new GUIStyle(ButtonStyle);
            BS.fontSize = Mathf.FloorToInt(DialogWidth / 9);
            if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
            LS = new GUIStyle(LabelStyle);
            LS.fontSize = Mathf.FloorToInt(DialogWidth / 7);
            if (LS.fontSize > MaxFontSize) LS.fontSize = MaxFontSize;
            GUI.Label(new Rect(Left, Top, DialogWidth, height), "Fixed", LS);
            if (GUI.Button(new Rect(Left + DialogWidth / 2, Top, DialogWidth / 2, height), "Unfixed", BS))
            {
                Fixed = false;
            }
            Top += Step;
        }
        else
        {
            BS = new GUIStyle(ButtonStyle);
            BS.fontSize = Mathf.FloorToInt(DialogWidth / 7);
            if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
            LS = new GUIStyle(LabelStyle);
            LS.fontSize = Mathf.FloorToInt(DialogWidth / 9);
            if (LS.fontSize > MaxFontSize) LS.fontSize = MaxFontSize;
            GUI.Label(new Rect(Left, Top, DialogWidth, height), "Unfixed", LS);
            if (GUI.Button(new Rect(Left + DialogWidth / 2, Top, DialogWidth / 2, height), "Fixed", BS))
            {
                Fixed = true;
            }
            Top += Step;
        }
        BS = new GUIStyle(ButtonStyle);
        BS.fontSize = Mathf.FloorToInt(DialogWidth / 8);
        if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
        if (GUI.Button(new Rect(Left, Top, DialogWidth, height), "Delete", BS))
        {
            Module md = LogParent.parent.GetComponent<Module>();
            DeleteAModule(md.Id);
            show = false;
        }
        Top += Step;
        BS = new GUIStyle(ButtonStyle);
        BS.fontSize = Mathf.FloorToInt(DialogWidth / 8);
        if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
        if (GUI.Button(new Rect(Left, Top, DialogWidth / 2, height), "Cancel", BS))
        {
            show = false;
        }
        BS = new GUIStyle(ButtonStyle);
        BS.fontSize = Mathf.FloorToInt(DialogWidth / 4);
        if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
        if (GUI.Button(new Rect(Left + DialogWidth / 2, Top, DialogWidth / 2, height), "OK", BS))
        {
            show = false;
            Module md = LogParent.parent.GetComponent<Module>();
            md.Constant = float.Parse(AngleConstant) * Mathf.PI / 180f;
            md.FixAngle = Fixed;
            md.ShowConstant = ShowConstant;
        }

    }
    void ModulePreferenceJapanese(float Left, float Top, float Step, float height)
    {
        GUI.Label(new Rect(Left, Top, DialogWidth, height), ObjectName, LabelStyle);
        Top += Step;
        GUI.Label(new Rect(Left, Top, DialogWidth, height), LogParent.GetComponent<Log>().Text2, LabelStyle);
        Top += Step;
        GUIStyle BS = new GUIStyle(ButtonStyle);
        BS.fontSize = Mathf.FloorToInt(DialogWidth / 4);
        if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
        if (GUI.Button(new Rect(Left, Top, DialogWidth, height), "消去", BS))
        {
            Module md = LogParent.parent.GetComponent<Module>();
            DeleteAModule(md.Id);
            show = false;
        }
        Top += Step;
        BS = new GUIStyle(ButtonStyle);
        BS.fontSize = Mathf.FloorToInt(DialogWidth / 8);
        if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
        if (GUI.Button(new Rect(Left, Top, DialogWidth / 2, height), "Cancel", BS))
        {
            show = false;
        }
        BS = new GUIStyle(ButtonStyle);
        BS.fontSize = Mathf.FloorToInt(DialogWidth / 4);
        if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
        if (GUI.Button(new Rect(Left + DialogWidth / 2, Top, DialogWidth / 2, height), "OK", BS))
        {
            show = false;
        }
    }

    void ModulePreferenceEnglish(float Left, float Top, float Step, float height)
    {
        GUI.Label(new Rect(Left, Top, DialogWidth, height), LogParent.GetComponent<Log>().Text1, LabelStyle);
        Top += Step;
        GUI.Label(new Rect(Left, Top, DialogWidth, height), LogParent.GetComponent<Log>().Text2, LabelStyle);
        Top += Step;
        GUIStyle BS = new GUIStyle(ButtonStyle);
        BS.fontSize = Mathf.FloorToInt(DialogWidth / 8);
        if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
        if (GUI.Button(new Rect(Left, Top, DialogWidth, height), "Delete", BS))
        {
            Module md = LogParent.parent.GetComponent<Module>();
            DeleteAModule(md.Id);
            show = false;
        }
        Top += Step;
        BS = new GUIStyle(ButtonStyle);
        BS.fontSize = Mathf.FloorToInt(DialogWidth / 8);
        if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
        if (GUI.Button(new Rect(Left, Top, DialogWidth / 2, height), "Cancel", BS))
        {
            show = false;
        }
        BS = new GUIStyle(ButtonStyle);
        BS.fontSize = Mathf.FloorToInt(DialogWidth / 4);
        if (BS.fontSize > MaxFontSize) BS.fontSize = MaxFontSize;
        if (GUI.Button(new Rect(Left + DialogWidth / 2, Top, DialogWidth / 2, height), "OK", BS))
        {
            show = false;
        }
    }

}