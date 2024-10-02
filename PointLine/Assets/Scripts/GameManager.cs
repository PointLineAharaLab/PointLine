using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int StageNumber=0;// AppMgr.GameStageNumber‚Æ˜AŒg
    public GameStageObjects thisStageObjects = null;

    // Start is called before the first frame update
    void Start()
    {
        
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

    private void InitilizeStage()
    {

    }




}

/// <summary>
/// 
/// </summary>
public class GameStageObjects
{
    public Module[] UserModules = null;
    public Module[] MasterModules = null;


    /// <summary>
    /// constructor
    /// </summary>
    GameStageObjects()
    {

    }
}


