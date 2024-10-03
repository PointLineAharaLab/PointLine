using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int StageNumber=0;// AppMgr.GameStageNumber‚Æ˜AŒg
    public Module[] UserModules = null;
    public Module[] MasterModules = null;
    public Point[] MasterPoints = null;
    public GameObject masterObjects;//= GameObject.Find("MasterObjects");

    // Start is called before the first frame update
    void Start()
    {
        if (AppMgr.GameOn)
        {
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

    private void InitilizeStage(int stageNmuber)
    {
        if (stageNmuber == 0) { 
            Point newPoint1 = Util.AddPoint(new Vector3(1f, 0f, 0f));
            Point newPoint2 = Util.AddPoint(new Vector3(0f, 1f, 0f));
            Point newPoint3 = Util.AddPoint(new Vector3(-0.5f, -0.5f, 0f));
            newPoint1.Fixed = true;
            newPoint2.Fixed = true;
            newPoint3.Fixed = true;
            newPoint1.gameObject.transform.SetParent(masterObjects.transform, false);
            newPoint2.gameObject.transform.SetParent(masterObjects.transform, false);
            newPoint3.gameObject.transform.SetParent(masterObjects.transform, false);
            Line newLine1 = Util.AddLine(0, 1);
            newLine1.gameObject.transform.SetParent(masterObjects.transform, false);
            newLine1.GetPoint1OfLine();
            newLine1.GetPoint2OfLine();
            newLine1.LineUpdate();
            newLine1.Isometry = -1;
        }
        AppMgr.ExecuteAllModules();
    }



}

