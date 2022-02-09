using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using OpenCvSharp.Demo;
using OpenCvSharp;
using OpenCvSharp.Aruco;

public class ButtonScript : MonoBehaviour
{
    public GameObject button;
    public GameObject canvas;
    BackGroundScreen Back;

    // Start is called before the first frame update
    void Start()
    {
        Back = GameObject.Find("BackGroundScreen").GetComponent<BackGroundScreen>();

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addButton(){
        GameObject prefab = (GameObject)Instantiate(button);
        prefab.transform.SetParent(canvas.transform, false);
    }

    Vector2 GetPointPosition(float ax, float ay, float bx, float by, float cx, float cy){
        Vector2 a = new Vector2(ax,ay);
        Vector2 b = new Vector2(bx,by);
        Vector2 c = new Vector2(cx,cy);
        float dist = Vector2.Distance(a,b);
        Vector2 x = (b-a)/dist;
        Vector2 y = new Vector2(-x.y, x.x);
        float s = Vector2.Dot(c-a, x);
        float t = Vector2.Dot(c-a, y);
        return new Vector2(s,t);
    }

            Vector3 ConvertTexture2World(float x, float y){
            float height = AppMgr.BackgroundTexture.height;
            float width = AppMgr.BackgroundTexture.width;
            float rate = 9f/height;
            float gx = (x-width*0.5f)*rate;
            float gy = (-y+height*0.5f)*rate;
            return new Vector3(gx,gy,0);
        }

    public class Line{
        public float x1,y1,x2,y2;
        public Line(float _x1,float _y1,float _x2,float _y2){
            x1 = _x1;
            y1 = _y1;
            x2 = _x2;
            y2 = _y2;
        }
        public float dist(){
            return Mathf.Sqrt((x1-x2)*(x1-x2)+(y1-y2)*(y1-y2));
        }
    }

    public void OnClick(){
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Slider");
        foreach(GameObject Sli in objects){
            Destroy(Sli);
        }
        Destroy(this.gameObject);
        Back.SliderDestory = true;

        Util.InitLog();
        int PId = -1, LId = -1, CId = -1, MId = -1;

        List<Line> line = new List<Line>();

        for(int i=0; i < Back.lines.Length; i++){
            if(i==0){
                line.Add(new Line(Back.lines[0].P1.X,Back.lines[0].P1.Y,Back.lines[0].P2.X,Back.lines[0].P2.Y));
            }
            else{ 
                bool AddNew = true;
                for(int j = 0; j<line.Count; j++){
                    Vector2 p1 = GetPointPosition(line[j].x1,line[j].y1,line[j].x2,line[j].y2,Back.lines[i].P1.X, Back.lines[i].P1.Y);
                    Vector2 p2 = GetPointPosition(line[j].x1,line[j].y1,line[j].x2,line[j].y2,Back.lines[i].P2.X, Back.lines[i].P2.Y);
                    Debug.Log("p1=" + p1 + "p2= " + p2);


                    if(Mathf.Abs(p1.y)<10f && Mathf.Abs(p2.y)<10f){//合流を検討
                        float d = line[j].dist();

                        if(Mathf.Max(0,p1.x)<Mathf.Min(d,p2.x)){//区間が交差
                          if(p1.x<0){
                              line[j].x1 = Back.lines[i].P1.X;
                              line[j].y1 = Back.lines[i].P1.Y;
                          }
                          if(d<p2.x){
                              line[j].x2 = Back.lines[i].P2.X;
                              line[j].y2 = Back.lines[i].P2.Y;
                          }
                          AddNew = false;
                        }
                    }
                }
                if(AddNew == true){
                            line.Add(new Line(Back.lines[i].P1.X,Back.lines[i].P1.Y,Back.lines[i].P2.X,Back.lines[i].P2.Y));
                        }
            }

        }


        for(int i=0; i < line.Count; i++){
            Vector3 v1 = ConvertTexture2World(line[i].x1 , line[i].y1);
            Vector3 v2 = ConvertTexture2World(line[i].x2 , line[i].y2);
            Util.AddPoint(v1,1);
            Util.AddPoint(v2,2);
            }
            

        for(int i=0; i < Back.circles.Length; i++){
            Debug.Log(Back.circles[i]);
            Vector3 v3 = ConvertTexture2World(Back.circles[i].Center.X , Back.circles[i].Center.Y);
            Util.AddPoint(v3,3);

        }

        Debug.Log(line.Count);
        
        /*
                    string str;
                    int PId = -1, LId = -1, CId = -1, MId = -1;
                    do
                    {
                        str = reader.ReadLine();
                        if (str == null) break;//多分要らない。
                        else
                        {
                            Log lg = GetLogFromString(str);
                            //AddLog(lg);
                            if (lg.ObjectType == "Point")
                            {
                                if (PId < lg.Id) PId = lg.Id;
                            }
                            else if (lg.ObjectType == "Line")
                            {
                                if (LId < lg.Id) LId = lg.Id;
                            }
                            else if (lg.ObjectType == "Circle")
                            {
                                if (CId < lg.Id) CId = lg.Id;
                            }
                            else if (lg.ObjectType == "Module")
                            {
                                if (MId < lg.Id) MId = lg.Id;
                            }
                        }
                    }
                    while (str != null);
                    reader.Close();
                    ClickOnPanel.SetId(PId + 1, LId + 1, CId + 1, MId + 1);
                    AppMgr.pts = MonoBehaviour.FindObjectsOfType<Point>();
                    AppMgr.lns = MonoBehaviour.FindObjectsOfType<Line>();
                    AppMgr.cis = MonoBehaviour.FindObjectsOfType<Circle>();
                    AppMgr.mds = MonoBehaviour.FindObjectsOfType<Module>();
    */
    }
}
