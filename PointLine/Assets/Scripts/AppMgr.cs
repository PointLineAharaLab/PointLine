
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
    public static string TmpFilename = null;

    public static Vector3 LeftBottom, RightUp;

    public GameObject LogFolder;

    public static int ConvergencyCount=0;
    public GameObject ConvergencyCountText;
    public static float ConvergencyError = 0.0001f;

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
        ConvergencyCountText.GetComponent<TextMesh>().text = ConvergencyCount.ToString();
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
        //if (0 <= v.x && v.x <= Screen.width && Screen.height-50 <= v.y && v.y <= Screen.height)
        //{
        //    //Debug.Log("ClickOnButton  = true");
        //    return true;
        //}
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
                for (int i = 0; i < md.Length; i++)
                {
                    md[i].ExecuteModule();
                }
            }
        }
    }



}
