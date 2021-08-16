
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppMgr : MonoBehaviour {
    /// <summary>
    /// Language
    /// </summary>
    public static int Japanese = 1;
    /// 

    public static int Mode = 0;
    public static int ModeStep = 0;

    public static bool DrawOn = true;
    public static bool MenuOn = false;
    public static bool KeyOn = true;
    public static bool FileDialogOn = false;

    public static Point[] pts = null;
    public static Line[] lns = null;
    public static Circle[] cis = null;
    public static Module[] mds = null;
    public static AngleMark[] ams = null; 
    public static string TmpFilename = null;

    public static Vector3 LeftBottom, RightUp;

    public GameObject LogFolder;

    public static int ConvergencyCount=0;
    public GameObject ConvergencyAlertText;
    public static float ConvergencyError = 0.0001f;
    public static bool ModuleOn = true;


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


    }


    public static int GetMode()
    {
        return Mode;
    }

	// Update is called once per frame
	void Update () {
        //ExecuteAllModules();
        Rescaling();
        if (ConvergencyCount > 1000)
        {
            if(Japanese==1)
                ConvergencyAlertText.GetComponent<TextMesh>().text = "競合(press Z)";
            else 
                ConvergencyAlertText.GetComponent<TextMesh>().text = "Conflict(press Z)";
            ModuleOn = false;
        }
        else
        {
            ConvergencyAlertText.GetComponent<TextMesh>().text = "";// ConvergencyCount.ToString();
        }
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
        if (md != null)
        {
            for (int repeat = 0; repeat < 2000; repeat++)
            {
                AppMgr.ConvergencyCount = 0;
                for (int i = 0; i < md.Length; i++)
                {
                    if (md[i].Type != MENU.ADD_LOCUS)
                    {
                        md[i].ExecuteModule();
                    }
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



}
