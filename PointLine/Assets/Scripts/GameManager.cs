using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int StageNumber=0;// AppMgr.GameStageNumber‚Æ˜AŒg
    public Module[] UserModules = null;
    public Module[] MasterModules = null;
    public Point[] MasterPoints = null;
    public GameObject masterObjects;//= GameObject.Find("MasterObjects");
    public GameObject masterMenu;
    

    // Start is called before the first frame update
    void Start()
    {
        if (AppMgr.GameOn)
        {
            AppMgr.GameMenuItems  = new List<int>();
            InitilizeStage(StageNumber);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// SewlectStage
    /// </summary>
    private void SelectStage()
    {

    }


    private Point InitializeFixedPoint(Vector3 v)
    {
        Point newPoint = Util.AddPoint(v);
        newPoint.Fixed = true;
        newPoint.gameObject.transform.SetParent(masterObjects.transform, false);
        AppMgr.pts = FindObjectsOfType<Point>();
        return newPoint;
    }

    private Line InitializeLine(int p1, int p2)
    {
        Line newLine = Util.AddLine(p1, p2);
        newLine.gameObject.transform.SetParent(masterObjects.transform, false);
        newLine.GetPoint1OfLine();
        newLine.GetPoint2OfLine();
        newLine.LineUpdate();
        newLine.Isometry = -1;
        AppMgr.lns = MonoBehaviour.FindObjectsOfType<Line>();
        return newLine;
    }
    private void InitilizeStage(int stageNmuber)
    {
        if (stageNmuber == 0) {
            InitializeFixedPoint(new Vector3(1f, 0f, 0f));//0
            InitializeFixedPoint(new Vector3(0f, 1f, 0f));//1
            InitializeFixedPoint(new Vector3(-0.5f, -0.5f, 0f));//2
            InitializeLine(0, 1);
            InitializeLine(1, 2);
            InitializeLine(0, 2);
            AppMgr.ExecuteAllModules();
            AppMgr.GameMenuItems.Add(MENU.ADD_CIRCLE);
        }
    }

}


class MenuItem
{
    int MenuId;
    int Remaining;
    public MenuItem() { }

}

