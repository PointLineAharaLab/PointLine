﻿#define JAPANESE 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButton : Menu, IPointerEnterHandler, IPointerExitHandler
{

    bool HoverOn = false;
    
    public Vector3 LeftTop = Vector3.zero, RightTop = Vector3.zero;

    public static GameObject Go;
    private string[] MenuText;
    public string PrefabName;
    public GUIStyle menuHoverStyle;

    #region Start
    // Use this for initialization
    void Start () {
        if (AppMgr.Japanese == 1) {
            MenuText = new string[] { //MenuBottonにホバリングした時のメッセージ
                " メニューから出る " , " 点追加 ",  " 中点追加(AM) ", " 直線追加(AL) ", " 円追加(AC) ", // 0 - 4
                " ほかの頂点に点を載せる(PP) ", " 直線に点を載せる(PL) ", " 円に点を載せる(PC) ","交点を追加",// 5 - 7
                " 2直線を等長に(LI) ","2直線の長さの比", " 2直線を垂直に(LP) ", " 2直線を平行に(LQ) ", " 直線を水平に(LH) ", "角度", "角の二等分線", // 8 - 11
                " 円を直線に接させる(TL) ", " 円を他の円に接させる(TC) "," 三角形を追加する() "," 四角形を追加する() ",//12 - 15
                " 頂点を固定する(FP) ","軌跡追加", " 頂点を消去する(DP) ", " すべて消去する(DA) ", //16 - 18
                " 戻る(Z) ", " 進む(Y) ",// 19, 20
                " ログ表示・非表示(W) ",// 21
                " 保存(S) "," 開く(O) "," TeX保存 "," 終了(Q) " // 22 - 25
             };
        }
        else
        {
            MenuText = new string[]
            {
                " out of menu ", " add a point ", " add a midpoint(AM) ", " add a line(AL) ", " add a circle(AC) ", // 0 - 4
                " Set a point on another point(PP) ", " Set a point on a line(PL) ", " Set a point on a circle(PC) ", " Add a crossing point ", //5 - 7
                " Let two lines be isometry(LI) ","Ratio of two lines", " Let two lines be perpendicular(LP) ", " Let two lines be parallel(LQ) ", " Let a line be horizontal(LH) ", " Angle ", "Angle bisector", // 8 - 10
                " Make a circle tangent to a line(TL) ", " Make a circle tangent to another circle(TC) ", "Add Triangle", "Add Quadrilateral", // 11 - 12
                " Fix a point(FP)  ", "add a locus"," Delete a point(DP) ", " Delete all(DA) ", //13 - 15
                " Undo(Z) ", " Redo(Y) "," Show Log ", // 16, 17
                " Save(S) "," Open(O) "," Save as TeX "," Quit(Q) " // 18 - 21
            };
        }
        LeftTop = Camera.main.ScreenToWorldPoint(new Vector3(0f, Screen.height, 0f));
        RightTop = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
        HoverOn = false;
        menuHoverStyle = new GUIStyle(MyStyle);
        menuHoverStyle.fontSize = (int)Mathf.Floor(Screen.width / 39);// // 横幅のサイズからここを決めるべき。
    }
    #endregion

    #region hovering on menues
    // Update is called once per frame
    void OnGUI()
    {
        string message = name;
        PrefabName = name;
        if (HoverOn && AppMgr.MenuOn)
        {
            for (int i = 0; i < PrefabCloneName.Length; i++)
            {
                if (name.Contains(PrefabCloneName[i]))
                {
                    message = MenuText[i];
                    break;
                }
            }
            float rectX = menuHoverStyle.CalcSize(new GUIContent(message)).x;
            float rectY = menuHoverStyle.CalcSize(new GUIContent(message)).y;
            GUI.Label(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, rectX, rectY), message, menuHoverStyle);
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        HoverOn = true;
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        HoverOn = false;

    }
    #endregion

    #region MenuOn/Off
    public void OnClick()
    {
        Debug.Log("menu on (mouse)");
        AppMgr.MenuOn = true;
        DestroyMenuOffUI();
        CreateMenuOnUI();
        AppMgr.DrawOn = false;

    }

    public void OnClickMenuOff()
    {
        Debug.Log("menu off (mouse)");
        AppMgr.Mode = MENU.ADD_POINT;
        AppMgr.ModeStep = 0;
        AppMgr.MenuOn = false;
        DestroyMenuOnUI();
        CreateMenuOffUI();
        AppMgr.DrawOn = true;
    }
    #endregion

    #region menu buttons
    public void OnClickAddPoint()
    {
        Debug.Log("add a point (mouse)");
        AppMgr.Mode = MENU.ADD_POINT;
        AppMgr.ModeStep = 0;
        AppMgr.MenuOn = false;
        DestroyMenuOnUI();
        CreateMenuOffUI();
        AppMgr.DrawOn = true;
    }

    public void OnClickAddLine()
    {
        Debug.Log("add a line (mouse)");
        AppMgr.Mode = MENU.ADD_LINE;
        AppMgr.ModeStep = 0;
        AppMgr.MenuOn = false;
        DestroyMenuOnUI();
        CreateMenuOffUI();
        AppMgr.DrawOn = true;
    }

    public void OnClickAddMidPoint()
    {
        Debug.Log("add a midpoint (mouse)");
        AppMgr.Mode = MENU.ADD_MIDPOINT;
        AppMgr.ModeStep = 0;
        AppMgr.MenuOn = false;
        DestroyMenuOnUI();
        CreateMenuOffUI();
        AppMgr.DrawOn = true;
    }

    public void OnClickAddCircle()
    {
        Debug.Log("add a circle (mouse)");
        AppMgr.Mode = MENU.ADD_CIRCLE;
        AppMgr.ModeStep = 0;
        AppMgr.MenuOn = false;
        DestroyMenuOnUI();
        CreateMenuOffUI();
        AppMgr.DrawOn = true;
    }

    public void OnClickPointOnPoint()
    {
        Debug.Log("Set a point on a point(mouse)");
        AppMgr.Mode = MENU.POINT_ON_POINT;
        AppMgr.ModeStep = 0;
        AppMgr.MenuOn = false;
        DestroyMenuOnUI();
        CreateMenuOffUI();
        AppMgr.DrawOn = true;
        AppMgr.ModuleOn = true;
    }

    public void OnClickPointOnLine()
    {
        Debug.Log("Set a point on a line(mouse)");
        AppMgr.Mode = MENU.POINT_ON_LINE;
        AppMgr.ModeStep = 0;
        AppMgr.MenuOn = false;
        DestroyMenuOnUI();
        CreateMenuOffUI();
        AppMgr.DrawOn = true;
        AppMgr.ModuleOn = true;
    }

    public void OnClickPointOnCircle()
    {
        Debug.Log("Set a point on a circle(mouse)");
        AppMgr.Mode = MENU.POINT_ON_CIRCLE;
        AppMgr.ModeStep = 0;
        AppMgr.MenuOn = false;
        DestroyMenuOnUI();
        CreateMenuOffUI();
        AppMgr.DrawOn = true;
        AppMgr.ModuleOn = true;
    }

    public void OnClickCrossingLL()
    {
        Debug.Log("Add an crossing(mouse)");
        AppMgr.Mode = MENU.CROSSING_LL;
        AppMgr.ModeStep = 0;
        AppMgr.MenuOn = false;
        DestroyMenuOnUI();
        CreateMenuOffUI();
        AppMgr.DrawOn = true;
        AppMgr.ModuleOn = true;
    }

    public void OnClickCrossingCL()
    {
        Debug.Log("Add an crossing(mouse)");
        AppMgr.Mode = MENU.CROSSING_CL;
        AppMgr.ModeStep = 0;
        AppMgr.MenuOn = false;
        DestroyMenuOnUI();
        CreateMenuOffUI();
        AppMgr.DrawOn = true;
        AppMgr.ModuleOn = true;
    }

    public void OnClickCrossingCC()
    {
        Debug.Log("Add an crossing(mouse)");
        AppMgr.Mode = MENU.CROSSING_CC;
        AppMgr.ModeStep = 0;
        AppMgr.MenuOn = false;
        DestroyMenuOnUI();
        CreateMenuOffUI();
        AppMgr.DrawOn = true;
        AppMgr.ModuleOn = true;
    }

    public void OnClickIsom()
    {
        Debug.Log("Let two lines isometry(mouse)");
        AppMgr.Mode = MENU.LINES_ISOMETRY;
        AppMgr.ModeStep = 0;
        AppMgr.MenuOn = false;
        DestroyMenuOnUI();
        CreateMenuOffUI();
        AppMgr.DrawOn = true;
        AppMgr.ModuleOn = true;
    }

    public void OnClickRatioLength()
    {
        Debug.Log("Set an ratio length(mouse)");
        AppMgr.Mode = MENU.RATIO_LENGTH;
        AppMgr.ModeStep = 0;
        AppMgr.MenuOn = false;
        DestroyMenuOnUI();
        CreateMenuOffUI();
        AppMgr.DrawOn = true;
        AppMgr.ModuleOn = true;
    }

    public void OnClickPerp()
    {
        Debug.Log("Let two lines perpendicular(mouse)");
        AppMgr.Mode = MENU.LINES_PERPENDICULAR;
        AppMgr.ModeStep = 0;
        AppMgr.MenuOn = false;
        DestroyMenuOnUI();
        CreateMenuOffUI();
        AppMgr.DrawOn = true;
        AppMgr.ModuleOn = true;
    }

    public void OnClickPara()
    {
        Debug.Log("Let two lines parallel(mouse)");
        AppMgr.Mode = MENU.LINES_PARALLEL;
        AppMgr.ModeStep = 0;
        AppMgr.MenuOn = false;
        DestroyMenuOnUI();
        CreateMenuOffUI();
        AppMgr.DrawOn = true;
        AppMgr.ModuleOn = true;
    }

    public void OnClickHori()
    {
        Debug.Log("Let a line horizontal(mouse)");
        AppMgr.Mode = MENU.LINE_HORIZONTAL;
        AppMgr.ModeStep = 0;
        AppMgr.MenuOn = false;
        DestroyMenuOnUI();
        CreateMenuOffUI();
        AppMgr.DrawOn = true;
        AppMgr.ModuleOn = true;
    }

    public void OnClickAngle()
    {
        Debug.Log("Set an angle(mouse)");
        AppMgr.Mode = MENU.ANGLE;
        AppMgr.ModeStep = 0;
        AppMgr.MenuOn = false;
        DestroyMenuOnUI();
        CreateMenuOffUI();
        AppMgr.DrawOn = true;
        AppMgr.ModuleOn = true;
    }

    public void OnClickBisector()
    {
        Debug.Log("Set an bisector(mouse)");
        AppMgr.Mode = MENU.BISECTOR;
        AppMgr.ModeStep = 0;
        AppMgr.MenuOn = false;
        DestroyMenuOnUI();
        CreateMenuOffUI();
        AppMgr.DrawOn = true;
        AppMgr.ModuleOn = true;
    }

    public void OnClickTangentL2C()
    {
        Debug.Log("Make a circle tangent to a line (mouse)");
        AppMgr.Mode = MENU.CIRCLE_TANGENT_LINE;
        AppMgr.ModeStep = 0;
        AppMgr.MenuOn = false;
        DestroyMenuOnUI();
        CreateMenuOffUI();
        AppMgr.DrawOn = true;
    }
    public void OnClickTangentC2C()
    {
        Debug.Log("Make a circle tangent to a circle (mouse)");
        AppMgr.Mode = MENU.CIRCLE_TANGENT_CIRCLE;
        AppMgr.ModeStep = 0;
        AppMgr.MenuOn = false;
        DestroyMenuOnUI();
        CreateMenuOffUI();
        AppMgr.DrawOn = true;
    }

    public void OnClickTriangle()
    {
        Debug.Log("Add a triangle (mouse)");
        AppMgr.Mode = MENU.TRIANGLE;
        AppMgr.ModeStep = 0;
        AppMgr.MenuOn = false;
        DestroyMenuOnUI();
        CreateMenuOffUI();
        AppMgr.DrawOn = true;
    }

    public void OnClickQudrilateral()
    {
        Debug.Log("Add a quadilateral (mouse)");
        AppMgr.Mode = MENU.QUADRILATERAL;
        AppMgr.ModeStep = 0;
        AppMgr.MenuOn = false;
        DestroyMenuOnUI();
        CreateMenuOffUI();
        AppMgr.DrawOn = true;
    }

    public void OnClickFixPoint()
    {
        Debug.Log("fix a point (mouse)");
        AppMgr.Mode = MENU.FIX_POINT;
        AppMgr.ModeStep = 0;
        AppMgr.MenuOn = false;
        DestroyMenuOnUI();
        CreateMenuOffUI();
        AppMgr.DrawOn = true;
    }

    public void OnClickAddLocus()
    {
        Debug.Log("add a locus (mouse)");
        AppMgr.Mode = MENU.ADD_LOCUS;
        AppMgr.ModeStep = 0;
        AppMgr.MenuOn = false;
        DestroyMenuOnUI();
        CreateMenuOffUI();
        AppMgr.DrawOn = true;
    }

    public void OnClickDeletePoint()
    {
        Debug.Log("delete a point (mouse)");
        AppMgr.Mode = MENU.DELETE_POINT;
        AppMgr.ModeStep = 0;
        AppMgr.MenuOn = false;
        DestroyMenuOnUI();
        CreateMenuOffUI();
        AppMgr.DrawOn = true;
    }
    public void OnClickDeleteAll()
    {
        Debug.Log("clear all  (mouse)");
        AppMgr.Mode = MENU.DELETE_ALL;
        ClickOnPanel.DeleteAll();
        AppMgr.Mode = 0;
        AppMgr.ModeStep = 0;
        AppMgr.MenuOn = false;
        DestroyMenuOnUI();
        CreateMenuOffUI();
        AppMgr.DrawOn = true;
    }
    public void OnClickUndo()
    {
        Debug.Log("undo (mouse)");
        AppMgr.Mode = MENU.UNDO;
        Util.Undo();
        AppMgr.Mode = 0;
        AppMgr.ModeStep = 0;
        AppMgr.MenuOn = false;
        DestroyMenuOnUI();
        CreateMenuOffUI();
        AppMgr.DrawOn = true;
        AppMgr.ModuleOn = true;
    }
    public void OnClickRedo()
    {
        Debug.Log("redo  (mouse)");
        AppMgr.Mode = MENU.REDO;
        Util.Redo();
        AppMgr.Mode = 0;
        AppMgr.ModeStep = 0;
        AppMgr.MenuOn = false;
        DestroyMenuOnUI();
        CreateMenuOffUI();
        AppMgr.DrawOn = true;
    }
    public void OnClickShowLogs()
    {
        Debug.Log("show/hide logs  (mouse)");
        AppMgr.Mode = MENU.ADD_POINT;
        AppMgr.Mode = 0;
        AppMgr.ModeStep = 0;
        AppMgr.MenuOn = false;
        Log[] Logs = MonoBehaviour.FindObjectsOfType<Log>();
        Util.ShowLog = !Util.ShowLog;
        for (int i=0; i<Logs.Length; i++)
        {
            Logs[i].Show = Util.ShowLog;
        }
        DestroyMenuOnUI();
        CreateMenuOffUI();
        AppMgr.DrawOn = true;
    }
    public void OnClickSave()
    {
        Debug.Log("save  (mouse)");
        AppMgr.Mode = MENU.SAVE;
        AppMgr.FileDialogOn = true;
        AppMgr.KeyOn = false;
        Util.RemakeLog();
        Util.SaveLog("TmpLog.txt");
        Util.CopyLog("TmpLog.txt", "TmpSaveFile.txt");
        //モードを非描画モードにする。
        AppMgr.DrawOn = false;
        AppMgr.KeyOn = false;
        DestroyMenuOnUI();
        CreateMenuOffUI();
        //保存ダイアログの描画＋ファイル保存
        Util.SaveLogSelectFile();
        // モードを通常に戻す。
        AppMgr.Mode = 0;
        AppMgr.ModeStep = 0;
        AppMgr.MenuOn = false;
        //AppMgr.DrawOn = true;
    }
    public void OnClickOpen()
    {
        Debug.Log("open  (mouse)");
        AppMgr.Mode = MENU.DELETE_ALL;
        ClickOnPanel.DeleteAll();
        AppMgr.Mode = MENU.OPEN;
        AppMgr.FileDialogOn = true;
        //モードを非描画モードにする。
        AppMgr.DrawOn = false;
        AppMgr.KeyOn = false;
        DestroyMenuOnUI();
        CreateMenuOffUI();
        Util.OpenLogSelectFile();
        //SceneManager.LoadScene("OpenDialog");
        AppMgr.Mode = 0;
        AppMgr.ModeStep = 0;
        AppMgr.MenuOn = false;
        //AppMgr.DrawOn = true;
    }
    public void OnClickSave2TeX()
    {
        Debug.Log("Save TeX file  (mouse)");
        AppMgr.DrawOn = false;
        AppMgr.KeyOn = false;
        DestroyMenuOnUI();
        CreateMenuOffUI();
        Util.SaveTeXFileSelectFile();
        AppMgr.Mode = 0;
        AppMgr.ModeStep = 0;
        AppMgr.MenuOn = false;
        //AppMgr.DrawOn = true;
    }
    public void OnClickQuit()
    {
        Debug.Log("Quit  (mouse)");
        AppMgr.Mode = MENU.QUIT;
        AppMgr.ModeStep = 0;
        AppMgr.MenuOn = false;
#if UNITY_WEBPLAYER
		Application.OpenURL("http://aharalab.sakura.ne.jp/");
#else
		Application.Quit();
#endif
        return;

    }
#endregion

}
