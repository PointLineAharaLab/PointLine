#define JAPANESE 
//#undef JAPANESE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MENU
{
    public const int ADD_POINT = 0;
    public const int ADD_MIDPOINT = 12;
    public const int ADD_LINE = 1;
    public const int ADD_CIRCLE = 4;
    public const int POINT_ON_POINT = 2;
    public const int POINT_ON_LINE = 3;
    public const int POINT_ON_CIRCLE = 5;
    public const int INTERSECTION = 22;
    public const int LINES_ISOMETRY = 6;
    public const int RATIO_LENGTH = 25;
    public const int LINES_PERPENDICULAR = 7;
    public const int LINES_PARALLEL = 8;
    public const int ANGLE = 23;
    public const int BISECTOR = 24;
    public const int CIRCLE_TANGENT_LINE = 9;
    public const int CIRCLE_TANGENT_CIRCLE = 10;
    public const int FIX_POINT = 11;
    public const int ADD_LOCUS = 26;// 
    public const int DELETE_POINT = 13;
    public const int DELETE_ALL = 14;
    public const int QUIT = 15;
    public const int UNDO = 16;
    public const int REDO = 17;
    public const int SHOW_LOGS = 18;
    public const int SAVE = 19;
    public const int OPEN = 20;
    public const int SAVE_TEX = 21;

}


public class Menu : MonoBehaviour
{

    public static string[] PrefabCloneName = new string[]
    {
        "ButtonMenuOff(Clone)", "ButtonAddPoint(Clone)", "ButtonAddMidPoint(Clone)","ButtonAddLine(Clone)","ButtonAddCircle(Clone)",
        "ButtonPointOnPoint(Clone)" ,"ButtonPointOnLine(Clone)","ButtonPointOnCircle(Clone)","ButtonIntersection(Clone)", 
        "ButtonIsom(Clone)", "ButtonRatioLength(Clone)", "ButtonPerp(Clone)", "ButtonPara(Clone)","ButtonAngle(Clone)" ,"ButtonBisector(Clone)" ,
        "ButtonTangentL2C(Clone)", "ButtonTangentC2C(Clone)",
        "ButtonFixPoint(Clone)", "ButtonAddLocus(Clone)", "ButtonDeletePoint(Clone)", "ButtonDeleteAll(Clone)", 
        "ButtonUndo(Clone)", "ButtonRedo(Clone)", "ButtonShowLogs(Clone)",
        "ButtonSave(Clone)", "ButtonOpen(Clone)", "ButtonSave2TeX(Clone)", "ButtonQuit(Clone)"


    };
    private string TextADD_POINT0 = "メニュー ";
    private string TextADD_POINT1 = " クリックすれば点を追加できます． ";
    private string TextADD_MIDPOINT0 = " 中点を追加: 頂点を選択.";
    private string TextADD_MIDPOINT1 = " 中点を追加: もう一つ頂点を選択.";
    private string TextADD_LINE0 = " 直線を追加: 頂点を選択.";
    private string TextADD_LINE1 = " 直線を追加: もう一つ頂点を選択.";
    private string TextADD_CIRCLE0 = " 円を追加: 頂点を選択.";
    private string TextADD_CIRCLE1 = " 円を追加: 任意の場所をクリック";
    private string TextPOINT_ON_POINT0 = " 点を点の上に載せる: 頂点を選択.";
    private string TextPOINT_ON_POINT1 = " 点を点の上に載せる: もう一つ頂点を選択.";
    private string TextPOINT_ON_LINE0 = " 点を直線の上に載せる: 頂点を選択.";
    private string TextPOINT_ON_LINE1 = " 点を直線の上に載せる: 直線を選択.";
    private string TextPOINT_ON_CIRCLE0 = " 点を円の上に載せる: 頂点を選択.";
    private string TextPOINT_ON_CIRCLE1 = " 点を円の上に載せる: 円を選択.";
    private string TextINTERSECTION0 = "交点を追加：一つ選択";
    private string TextINTERSECTION1 = "交点を追加：もう一つ選択";
    private string TextLINES_ISOMETRY0 = " ２直線を同じ長さにする: 直線を選択.";
    private string TextLINES_ISOMETRY1 = " ２直線を同じ長さにする: もう一つ直線を選択.";
    private string TextRATIO_LENGTH0 = "線分の長さの比：直線を選択";//
    private string TextRATIO_LENGTH1 = "線分の長さの比：もうひとつ直線を選択";
    private string TextLINES_PERPENDICULAR0 = " ２直線を直交させる: 直線を選択.";
    private string TextLINES_PERPENDICULAR1 = " ２直線を直交させる: もう一つ直線を選択.";
    private string TextLINES_PARALLEL0 = " ２直線を平行にする: 直線を選択.";
    private string TextLINES_PARALLEL1 = " ２直線を平行にする: もう一つ直線を選択.";
    private string TextANGLE0 = " 角度を指定する: 頂点を選択.";
    private string TextANGLE1 = " 角度を指定する: もう一つ頂点を選択.";
    private string TextANGLE2 = " 角度を指定する: さらに頂点を選択.";
    private string TextBISECTOR0 = " ２つの角度を等しくする: 角を選択.";
    private string TextBISECTOR1 = " ２つの角度を等しくする: もう一つ角を選択.";
    private string TextCIRLCE_TANGENT_LINE0 = " 円を直線に接させる: 円を選択.";
    private string TextCIRLCE_TANGENT_LINE1 = " 円を直線に接させる: 直線を選択.";
    private string TextCIRLCE_TANGENT_CIRCLE0 = " 円を円に接させる: 円を選択.";
    private string TextCIRLCE_TANGENT_CIRCLE1 = " 円を円に接させる: もう一つ円を選択.";
    private string TextFIX_POINT1 = " 点を固定する : 頂点を選択.";
    private string TextADD_LOCUS0 = " 軌跡を追加: 頂点を選択.";
    private string TextDELETE_POINT0 = " 点を消去する: 頂点を選択.";


    public GUIStyle MyStyle;
    public static Canvas canvas;

    public static GUIStyle gst;
    static int FontSize;
//    static float FontHeight;

    static float MenuOnTextWidth;
    static Rect GuideTextRect;


    //  initialization
    void Start() {
        if (AppMgr.Japanese != 1) {// English 
            TextADD_POINT0 = "Menu";
            TextADD_POINT1 = " Click once to make a new point. ";
            TextADD_MIDPOINT0 = " Draw a midpoint : Select a point.";
            TextADD_MIDPOINT1 = " Draw a midpoint : Select another point.";
            TextADD_LINE0 = " Add a line : Select a point.";
            TextADD_LINE1 = " Add a line : Select another point.";
            TextADD_CIRCLE0 = " Add a circle: Select a point.";
            TextADD_CIRCLE1 = " Add a circle: Click anywhere once.";
            TextPOINT_ON_POINT0 = " Set a point on another point: Select a point.";
            TextPOINT_ON_POINT1 = " Set a point on another point: Select an object.";
            TextPOINT_ON_LINE0 = " Set a point on a line: Select a point.";
            TextPOINT_ON_LINE1 = " Set a point on a line: Select a line.";
            TextPOINT_ON_CIRCLE0 = " Set a point on a circle: Select a point.";
            TextPOINT_ON_CIRCLE1 = " Set a point on a circle: Select a circle.";
            TextLINES_ISOMETRY0 = " Make two lines isometry: Select a line.";
            TextLINES_ISOMETRY1 = " Make two lines isometry: Select another line.";
            TextRATIO_LENGTH0 = "Ratio of two segments : Select a line";//
            TextRATIO_LENGTH1 = "Ratio of two segments : Select another line";//
            TextLINES_PERPENDICULAR0 = " Make two lines perpendicular: Select a line.";
            TextLINES_PERPENDICULAR1 = " Make two lines perpendicular: Select another line.";
            TextLINES_PARALLEL0 = " Make two lines parallel: Select a line.";
            TextLINES_PARALLEL1 = " Make two lines parallel: Select another line.";
            TextANGLE0 = " Angle: Select a point.";
    　　　  TextANGLE1 = " Angle: Select another point.";
            TextANGLE2 = " Angle: Select the third point.";
            TextBISECTOR0 = " Make two angles equal; Select an angle.";
            TextBISECTOR1 = " Make two angles equal; Select another angle.";
            TextCIRLCE_TANGENT_LINE0 = " Make a circle tangent to a line: Select a circle.";
            TextCIRLCE_TANGENT_LINE1 = " Make a circle tangent to a line: Select a line.";
            TextCIRLCE_TANGENT_CIRCLE0 = " Make a circle tangent to a circle: Select a circle.";
            TextCIRLCE_TANGENT_CIRCLE1 = " Make a circle tangent to a circle: Select another circle.";
            TextFIX_POINT1 = " Fix/Unfix a point : Select a point.";
            TextADD_LOCUS0 = " Add a locus : Select a point."; 
            TextDELETE_POINT0 = " Delete a point : Select a point.";
        }

        gst = new GUIStyle(MyStyle);
        FontSize = (int)Mathf.Floor(Screen.width / 39);
        //FontHeight = 1.2f * FontSize;
        gst.fontSize = FontSize;// // 横幅のサイズからここを決めるべき。
        gst.hover.textColor = new Color(255,255,0);

        MenuOnTextWidth = gst.CalcSize(new GUIContent(TextADD_POINT0)).x;
        ////Debug.Log("MenuOnTextWidth = " + MenuOnTextWidth);
        GuideTextRect = new Rect(10 + MenuOnTextWidth, 10, 512, 40);

        //Canvasの取得
        canvas = FindObjectOfType<Canvas>();

        //ガイドボタンの表示
        CreateMenuOffUI();


    }

    void OnGUI()
    {

        if (AppMgr.MenuOn)
        {
        }
        else//ガイドの文字列でのみ使う
        //ちなみに、GUI関係の作業はOnGUI()から呼ばなければならない。
        {
            DrawGuideText();
        }
    }



    /// ラベルの描画.
    public  void GUILabel(string text)
    {
        GUI.Label(GuideTextRect, text, gst);
    }

    //ホバーの処理
    static void OnHover(Rect r)
    {
        if (gst == null) return;
        Vector3 v = Input.mousePosition;
        v.y = Screen.height - v.y;
        if (r.Contains(v))
            gst.normal.textColor = Color.green;
        else
            gst.normal.textColor = Color.white;
    }

    // ガイドの表示
    public void DrawGuideText()
    {
        //ガイドテキストを表示する。(メニュへ誘導するボタンは別扱い。)
        //ガイドテキストはMyStyleに従う。
        switch (AppMgr.Mode)
        {
            case MENU.ADD_POINT:
                GUILabel(TextADD_POINT1);
                break;
            case MENU.ADD_LINE:// draw a line
                if (AppMgr.ModeStep == 0)
                {
                    GUILabel(TextADD_LINE0);
                }
                else
                {
                    GUILabel(TextADD_LINE1);
                }
                break;
            case MENU.POINT_ON_POINT:// lay a point on another object.
                if (AppMgr.ModeStep == 0)
                {
                    GUILabel(TextPOINT_ON_POINT0);
                }
                else
                {
                    GUILabel(TextPOINT_ON_POINT1);
                }
                break;
            case MENU.POINT_ON_LINE:// lay a point on a line.
                if (AppMgr.ModeStep == 0)
                {
                    GUILabel(TextPOINT_ON_LINE0);
                }
                else
                {
                    GUILabel(TextPOINT_ON_LINE1);
                }
                break;
            case MENU.ADD_CIRCLE:// draw a circle.
                if (AppMgr.ModeStep == 0)
                {
                    GUILabel(TextADD_CIRCLE0);
                }
                else
                {
                    GUILabel(TextADD_CIRCLE1);
                }
                break;
            case MENU.POINT_ON_CIRCLE:// lay a point on a circle.
                if (AppMgr.ModeStep == 0)
                {
                    GUILabel(TextPOINT_ON_CIRCLE0);
                }
                else
                {
                    GUILabel(TextPOINT_ON_CIRCLE1);
                }
                break;
            case MENU.INTERSECTION:// どこかに挿入.
                if (AppMgr.ModeStep == 0)
                {
                    GUILabel(TextINTERSECTION0);
                }
                else
                {
                    GUILabel(TextINTERSECTION1);
                }
                break;
            case MENU.LINES_ISOMETRY:// make two lines isometry
                if (AppMgr.ModeStep == 0)
                {
                    GUILabel(TextLINES_ISOMETRY0);
                }
                else
                {
                    GUILabel(TextLINES_ISOMETRY1);
                }
                break;
            case MENU.RATIO_LENGTH:
                if (AppMgr.ModeStep == 0)
                {
                    GUILabel(TextRATIO_LENGTH0);
                }
                else if (AppMgr.ModeStep == 1)
                {
                    GUILabel(TextRATIO_LENGTH1);
                }
                break;
            case MENU.LINES_PERPENDICULAR://make two lines perpendicular
                if (AppMgr.ModeStep == 0)
                {
                    GUILabel(TextLINES_PERPENDICULAR0);
                }
                else
                {
                    GUILabel(TextLINES_PERPENDICULAR1);
                }
                break;
            case MENU.LINES_PARALLEL:// make two lines parallel
                if (AppMgr.ModeStep == 0)
                {
                    GUILabel(TextLINES_PARALLEL0);
                }
                else
                {
                    GUILabel(TextLINES_PARALLEL1);
                }
                break;
            case MENU.ANGLE:
                if (AppMgr.ModeStep == 0)
                {
                    GUILabel(TextANGLE0);
                }
                else if (AppMgr.ModeStep == 1)
                {
                    GUILabel(TextANGLE1);
                }
                else
                {
                    GUILabel(TextANGLE2);
                }
                break;
            case MENU.BISECTOR:
                if (AppMgr.ModeStep == 0)
                {
                    GUILabel(TextBISECTOR0);
                }
                else if (AppMgr.ModeStep == 1)
                {
                    GUILabel(TextBISECTOR1);
                }
                break;
            case MENU.CIRCLE_TANGENT_LINE:// make a circle tangent to a line
                if (AppMgr.ModeStep == 0)
                {
                    GUILabel(TextCIRLCE_TANGENT_LINE0);
                }
                else
                {
                    GUILabel(TextCIRLCE_TANGENT_LINE1);
                }
                break;
            case MENU.CIRCLE_TANGENT_CIRCLE:// make a circle tangent to a circle
                if (AppMgr.ModeStep == 0)
                {
                    GUILabel(TextCIRLCE_TANGENT_CIRCLE0);
                }
                else
                {
                    GUILabel(TextCIRLCE_TANGENT_CIRCLE1);
                }
                break;
            case MENU.FIX_POINT:// fix/unfix a point.
                GUILabel(TextFIX_POINT1);
                break;
            case MENU.ADD_MIDPOINT:// add a midpoint
                if (AppMgr.ModeStep == 0)
                {
                    GUILabel(TextADD_MIDPOINT0);
                }
                else
                {
                    GUILabel(TextADD_MIDPOINT1);
                }
                break;
            case MENU.ADD_LOCUS:// add a locus
                GUILabel(TextADD_LOCUS0);
                break;
            case MENU.DELETE_POINT:// delete a point
                if (AppMgr.ModeStep == 0)
                {
                    GUILabel(TextDELETE_POINT0);
                }
                break;
        }
    }


    public void DestroyMenuOnUI()
    {
        GameObject[] go = MonoBehaviour.FindObjectsOfType<GameObject>();
        for (int i = 0; i < go.Length; i++)
        {
            if (go[i].name == "ButtonMenuOff(Clone)" ||
                go[i].name == "ButtonAddPoint(Clone)" ||
                go[i].name == "ButtonAddMidPoint(Clone)" ||
                go[i].name == "ButtonAddLine(Clone)" ||
                go[i].name == "ButtonAddCircle(Clone)" ||
                go[i].name == "ButtonPointOnPoint(Clone)" ||
                go[i].name == "ButtonPointOnLine(Clone)" ||
                go[i].name == "ButtonPointOnCircle(Clone)" ||
                go[i].name == "ButtonIntersection(Clone)" ||
                go[i].name == "ButtonIsom(Clone)" ||
                go[i].name == "ButtonRatioLength(Clone)" || //
                go[i].name == "ButtonPerp(Clone)" ||
                go[i].name == "ButtonPara(Clone)" ||
                go[i].name == "ButtonAngle(Clone)" ||
                go[i].name == "ButtonBisector(Clone)"||
                go[i].name == "ButtonTangentL2C(Clone)" ||
                go[i].name == "ButtonTangentC2C(Clone)" ||
                go[i].name == "ButtonFixPoint(Clone)" ||
                go[i].name == "ButtonAddLocus(Clone)" ||
                go[i].name == "ButtonDeletePoint(Clone)" ||
                go[i].name == "ButtonDeleteAll(Clone)" ||
                go[i].name == "ButtonUndo(Clone)" ||
                go[i].name == "ButtonRedo(Clone)" ||
                go[i].name == "ButtonShowLogs(Clone)" ||
                go[i].name == "ButtonSave(Clone)" ||
                go[i].name == "ButtonOpen(Clone)" ||
                go[i].name == "ButtonSave2TeX(Clone)" ||
                go[i].name == "ButtonQuit(Clone)" 
                )
            {
                Destroy(go[i]);
            }
        }
    }

    public void CreateMenuOnUI()
    {
        CreateMenuOffButton();
        CreateAddPointButton();
        CreateAddMidPointButton();
        CreateAddLineButton();
        CreateAddCircleButton();
        CreatePointOnPointButton();
        CreatePointOnLineButton();
        CreatePointOnCircleButton();
        CreateIntersectionButton();
        CreateIsomButton();
        CreateRatioLengthButton();
        CreatePerpButton();
        CreateParaButton();
        CreateAngleButton();
        CreateBisectorButton();
        CreateTangentL2CButton();
        CreateTangentC2CButton();
        CreateFixPointButton();
        CreateAddLocusButton();
        CreateDeletePointButton();
        CreateDeleteAllButton();
        CreateUndoButton();
        CreateRedoButton();
        CreateShowLogsButton();
        CreateSaveButton();
        CreateOpenButton();
        CreateSaveToTeXButton();
        CreateQuitButton();
    }

    public void DestroyMenuOffUI()
    {
        GameObject [] go = MonoBehaviour.FindObjectsOfType<GameObject>();
        for(int i=0; i<go.Length; i++)
        {
            if(go[i].name == "ButtonMenuOn(Clone)")
            {
                Destroy(go[i]);
            }
        }

    }
    public void CreateMenuOffUI()
    {
        CreateMenuOnButton();
    }

    public void CreateMenuOnButton()
    {
        // MenuOn button

        GameObject Prefab = Resources.Load<GameObject>("Prefabs/ButtonMenuOn");
        MenuButton.Go = MenuButton.Instantiate<GameObject>(Prefab, new Vector3(100f, -75f, 0f), Quaternion.identity);
        MenuButton.Go.transform.SetParent(canvas.transform, false);
    }

    public void CreateMenuOffButton()
    {
        // MenuOff button
        GameObject Prefab = Resources.Load<GameObject>("Prefabs/ButtonMenuOff");
        MenuButton.Go = MenuButton.Instantiate<GameObject>(Prefab, new Vector3(100f, -75f, 0f), Quaternion.identity);
        MenuButton.Go.transform.SetParent(canvas.transform, false);
    }

    public void CreateAddPointButton()
    {
        // AddPoint button
        GameObject Prefab = Resources.Load<GameObject>("Prefabs/ButtonAddPoint");
        MenuButton.Go = MenuButton.Instantiate<GameObject>(Prefab, new Vector3(100f, -75f-150f, 0f), Quaternion.identity);
        MenuButton.Go.transform.SetParent(canvas.transform, false);
    }

    public void CreateAddMidPointButton()
    {
        // AddMidpoint button
        GameObject Prefab = Resources.Load<GameObject>("Prefabs/ButtonAddMidPoint");
        MenuButton.Go = MenuButton.Instantiate<GameObject>(Prefab, new Vector3(100f + 150f, -75f - 150f, 0f), Quaternion.identity);
        MenuButton.Go.transform.SetParent(canvas.transform, false);
    }
    public void CreateAddLineButton()
    {
        // AddLine button
        GameObject Prefab = Resources.Load<GameObject>("Prefabs/ButtonAddLine");
        MenuButton.Go = MenuButton.Instantiate<GameObject>(Prefab, new Vector3(100f + 150f * 2, -75f - 150f, 0f), Quaternion.identity);
        MenuButton.Go.transform.SetParent(canvas.transform, false);
    }
    public void CreateAddCircleButton()
    {
        // AddCircle button
        GameObject Prefab = Resources.Load<GameObject>("Prefabs/ButtonAddCircle");
        MenuButton.Go = MenuButton.Instantiate<GameObject>(Prefab, new Vector3(100f + 150f * 3, -75f - 150f, 0f), Quaternion.identity);
        MenuButton.Go.transform.SetParent(canvas.transform, false);
    }

    public void CreatePointOnPointButton()
    {
        // PointToPoint button
        GameObject Prefab = Resources.Load<GameObject>("Prefabs/ButtonPointOnPoint");
        MenuButton.Go = MenuButton.Instantiate<GameObject>(Prefab, new Vector3(100f + 150f * 0, -75f - 150f * 2, 0f), Quaternion.identity);
        MenuButton.Go.transform.SetParent(canvas.transform, false);
    }

    public void CreatePointOnLineButton()
    {
        // PointToPoint button
        GameObject Prefab = Resources.Load<GameObject>("Prefabs/ButtonPointOnLine");
        MenuButton.Go = MenuButton.Instantiate<GameObject>(Prefab, new Vector3(100f + 150f * 1, -75f - 150f * 2, 0f), Quaternion.identity);
        MenuButton.Go.transform.SetParent(canvas.transform, false);
    }

    public void CreatePointOnCircleButton()
    {
        // PointToPoint button
        GameObject Prefab = Resources.Load<GameObject>("Prefabs/ButtonPointOnCircle");
        MenuButton.Go = MenuButton.Instantiate<GameObject>(Prefab, new Vector3(100f + 150f * 2, -75f - 150f * 2, 0f), Quaternion.identity);
        MenuButton.Go.transform.SetParent(canvas.transform, false);
    }

    public void CreateIntersectionButton()// CreatePointOnCircleButton()の後に挿入
    {
        // Intersection button
        GameObject Prefab = Resources.Load<GameObject>("Prefabs/ButtonIntersection");
        MenuButton.Go = MenuButton.Instantiate<GameObject>(Prefab, new Vector3(100f + 150f * 3, -75f - 150f * 2, 0f), Quaternion.identity);
        MenuButton.Go.transform.SetParent(canvas.transform, false);
    }

    public void CreateIsomButton()
    {
        // isom button
        GameObject Prefab = Resources.Load<GameObject>("Prefabs/ButtonIsom");
        MenuButton.Go = MenuButton.Instantiate<GameObject>(Prefab, new Vector3(100f + 150f * 0, -75f - 150f * 3, 0f), Quaternion.identity);
        MenuButton.Go.transform.SetParent(canvas.transform, false);
    }
    
    public void CreateRatioLengthButton()
    {
        // ratio_length button
        GameObject Prefab = Resources.Load<GameObject>("Prefabs/ButtonRatioLength");
        MenuButton.Go = MenuButton.Instantiate<GameObject>(Prefab, new Vector3(100f + 150f * 1, -75f - 150f * 3, 0f), Quaternion.identity);// ここは、座標計算をしなくて済むように改造。
        MenuButton.Go.transform.SetParent(canvas.transform, false);
    } //

    public void CreatePerpButton()
    {
        // perp button
        GameObject Prefab = Resources.Load<GameObject>("Prefabs/ButtonPerp");
        MenuButton.Go = MenuButton.Instantiate<GameObject>(Prefab, new Vector3(100f + 150f * 2, -75f - 150f * 3, 0f), Quaternion.identity);
        MenuButton.Go.transform.SetParent(canvas.transform, false);
    }

    public void CreateParaButton()
    {
        // para button
        GameObject Prefab = Resources.Load<GameObject>("Prefabs/ButtonPara");
        MenuButton.Go = MenuButton.Instantiate<GameObject>(Prefab, new Vector3(100f + 150f * 3, -75f - 150f * 3, 0f), Quaternion.identity);
        MenuButton.Go.transform.SetParent(canvas.transform, false);
    }

    public void CreateAngleButton()
    {
        // angle button
        GameObject Prefab = Resources.Load<GameObject>("Prefabs/ButtonAngle");
        MenuButton.Go = MenuButton.Instantiate<GameObject>(Prefab, new Vector3(100f + 150f * 4, -75f - 150f * 3, 0f), Quaternion.identity);
        MenuButton.Go.transform.SetParent(canvas.transform, false);
    }

    public void CreateBisectorButton()
    {
        // bisector button
        GameObject Prefab = Resources.Load<GameObject>("Prefabs/ButtonBisector");
        MenuButton.Go = MenuButton.Instantiate<GameObject>(Prefab, new Vector3(100f + 150f * 5, -75f - 150f * 3, 0f), Quaternion.identity);
        MenuButton.Go.transform.SetParent(canvas.transform, false);
    }

    public void CreateTangentL2CButton()
    {
        // TangentL2C button
        GameObject Prefab = Resources.Load<GameObject>("Prefabs/ButtonTangentL2C");
        MenuButton.Go = MenuButton.Instantiate<GameObject>(Prefab, new Vector3(100f + 150f * 0, -75f - 150f * 4, 0f), Quaternion.identity);
        MenuButton.Go.transform.SetParent(canvas.transform, false);
    }

    public void CreateTangentC2CButton()
    {
        // TangentC2C button
        GameObject Prefab = Resources.Load<GameObject>("Prefabs/ButtonTangentC2C");
        MenuButton.Go = MenuButton.Instantiate<GameObject>(Prefab, new Vector3(100f + 150f * 1, -75f - 150f * 4, 0f), Quaternion.identity);
        MenuButton.Go.transform.SetParent(canvas.transform, false);
    }

    public void CreateFixPointButton()
    {
        // FixPoint button
        GameObject Prefab = Resources.Load<GameObject>("Prefabs/ButtonFixPoint");
        MenuButton.Go = MenuButton.Instantiate<GameObject>(Prefab, new Vector3(100f + 150f * 0, -75f - 150f * 5, 0f), Quaternion.identity);
        MenuButton.Go.transform.SetParent(canvas.transform, false);
    }
    public void CreateAddLocusButton()
    {
        // AddLine button
        GameObject Prefab = Resources.Load<GameObject>("Prefabs/ButtonAddLocus");
        MenuButton.Go = MenuButton.Instantiate<GameObject>(Prefab, new Vector3(100f + 150f * 1, -75f - 150f * 5, 0f), Quaternion.identity);
        MenuButton.Go.transform.SetParent(canvas.transform, false);
    }

    public void CreateDeletePointButton()
    {
        // DeletePoint button
        GameObject Prefab = Resources.Load<GameObject>("Prefabs/ButtonDeletePoint");
        MenuButton.Go = MenuButton.Instantiate<GameObject>(Prefab, new Vector3(100f + 150f * 2, -75f - 150f * 5, 0f), Quaternion.identity);
        MenuButton.Go.transform.SetParent(canvas.transform, false);
    }
    public void CreateDeleteAllButton()
    {
        // DeleteAll button
        GameObject Prefab = Resources.Load<GameObject>("Prefabs/ButtonDeleteAll");
        MenuButton.Go = MenuButton.Instantiate<GameObject>(Prefab, new Vector3(100f + 150f * 3, -75f - 150f * 5, 0f), Quaternion.identity);
        MenuButton.Go.transform.SetParent(canvas.transform, false);
    }

    public void CreateUndoButton()
    {
        // Undo button
        GameObject Prefab = Resources.Load<GameObject>("Prefabs/ButtonUndo");
        MenuButton.Go = MenuButton.Instantiate<GameObject>(Prefab, new Vector3(100f + 150f * 0, -75f - 150f * 6, 0f), Quaternion.identity);
        MenuButton.Go.transform.SetParent(canvas.transform, false);
    }

    public void CreateRedoButton()
    {
        // Redo button
        GameObject Prefab = Resources.Load<GameObject>("Prefabs/ButtonRedo");
        MenuButton.Go = MenuButton.Instantiate<GameObject>(Prefab, new Vector3(100f + 150f * 1, -75f - 150f * 6, 0f), Quaternion.identity);
        MenuButton.Go.transform.SetParent(canvas.transform, false);
    }

    public void CreateShowLogsButton()
    {
        // Redo button
        GameObject Prefab = Resources.Load<GameObject>("Prefabs/ButtonShowLogs");
        MenuButton.Go = MenuButton.Instantiate<GameObject>(Prefab, new Vector3(100f + 150f * 2, -75f - 150f * 6, 0f), Quaternion.identity);
        MenuButton.Go.transform.SetParent(canvas.transform, false);
    }

    public void CreateSaveButton()
    {
#if UNITY_STANDALONE
        // Save button
        GameObject Prefab = Resources.Load<GameObject>("Prefabs/ButtonSave");
        MenuButton.Go = MenuButton.Instantiate<GameObject>(Prefab, new Vector3(100f + 150f * 0, -75f - 150f * 7, 0f), Quaternion.identity);
        MenuButton.Go.transform.SetParent(canvas.transform, false);
#endif
    }

    public void CreateOpenButton()
    {
#if UNITY_STANDALONE
        // Open button
        GameObject Prefab = Resources.Load<GameObject>("Prefabs/ButtonOpen");
        MenuButton.Go = MenuButton.Instantiate<GameObject>(Prefab, new Vector3(100f + 150f * 1, -75f - 150f * 7, 0f), Quaternion.identity);
        MenuButton.Go.transform.SetParent(canvas.transform, false);
#endif
    }

        public void CreateSaveToTeXButton()
    {
#if UNITY_STANDALONE
        // SaveToTeX button
        GameObject Prefab = Resources.Load<GameObject>("Prefabs/ButtonSave2TeX");
        MenuButton.Go = MenuButton.Instantiate<GameObject>(Prefab, new Vector3(100f + 150f * 2, -75f - 150f * 7, 0f), Quaternion.identity);
        MenuButton.Go.transform.SetParent(canvas.transform, false);
#endif
    }

    public void CreateQuitButton()
    {
        // Quit button
        GameObject Prefab = Resources.Load<GameObject>("Prefabs/ButtonQuit");
        MenuButton.Go = MenuButton.Instantiate<GameObject>(Prefab, new Vector3(100f + 150f * 3, -75f - 150f * 7, 0f), Quaternion.identity);
        MenuButton.Go.transform.SetParent(canvas.transform, false);
    }

}
