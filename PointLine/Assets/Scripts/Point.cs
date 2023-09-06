using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //

public class Point : Object
{

    public bool Active;
    public int Id = 0;
    public bool Selected = false;
    public bool Fixed = false;
    public GameObject parent;

    public GameObject GameLog=null;// ログ
    public GameObject PTobject = null;//テキスト

    public string PointName;
    public bool ShowPointName = true;

    private Vector3 vec;
    public Vector3 Vec
    {
        get { return vec; }
        set {
            vec = value;
            vec.z = 0f;
        }
    }

    Color StandardColor = new Color(0.5f, 0.5f, 0.9f); 
    Color SelectedColor = new Color(0.0f, 0.0f, 0.9f);
    Color FixedColor = new Color(0.8f, 0.0f, 0.0f);
    Color FixedSelectedColor = new Color(1.0f, 0.0f, 0.0f); 


    // Use this for initialization
    void Start() {
        this.thisis = "Point";
        Vec = gameObject.transform.position;
        Active = true;

        if (PTobject != null)
        {
            PointName = PTobject.GetComponent<TextMesh>().text;
        }
        else
        {
            PointName = "P" + Id;
        }
    }

    // Update is called once per frame
    void Update() {
        gameObject.transform.position = Vec;// 点を動かすときはVecを更新する。
        //position.x = Vec.x+40;
        //position.y = Vec.y+40;
        //GUI.Label(position, PointName, Menu.gst);
        if (parent != null) parent.SetActive(Active);
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (Selected) {
            if (Fixed) {
                sr.color = FixedSelectedColor;
            }
            else {
                sr.color = SelectedColor;
            }
        }
        else {
            if (Fixed) {
                sr.color = FixedColor;
            }
            else {
                sr.color = StandardColor;
            }
        }
        if (PTobject != null)
        {// 文字の位置の更新
            Vector3 TextVec = Vec;
            TextVec.Normalize();
            if (TextVec == Vector3.zero) TextVec = Vector3.right;
            TextVec *= 2f;
            PTobject.transform.localPosition = TextVec;
            PTobject.GetComponent<TextMesh>().text = PointName;
            // 文字を表示しないときにはオブジェクトを非アクティブにする
            PTobject.SetActive(ShowPointName);
        }
        if (GameLog != null) 
        {
            GameLog.GetComponent<Log>().Vec = Vec;
        }

    }

    public static void AllPointsUnselected()
    {
        for (int i = 0; i < AppMgr.pts.Length; i++)
        {
            AppMgr.pts[i].Selected = false;
        }
    }

    public static void MakeOnePointSelected(int MOP)
    {
        for (int i = 0; i < AppMgr.pts.Length; i++)
        {
            if (AppMgr.pts[i].Id == MOP)
            {
                AppMgr.pts[i].Selected = true;
            }
            else
            {
                AppMgr.pts[i].Selected = false;
            }
        }
    }

    public static void AddOnePointSelected(int MOP)
    {
        for (int i = 0; i < AppMgr.pts.Length; i++)
        {
            if (AppMgr.pts[i].Id == MOP)
            {
                AppMgr.pts[i].Selected = true;
            }
        }
    }

}
