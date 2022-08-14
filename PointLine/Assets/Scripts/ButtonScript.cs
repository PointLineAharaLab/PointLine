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

    public class Line2{
        public float x1,y1,x2,y2;
        public Line2(float _x1,float _y1,float _x2,float _y2){
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

        List<Line2> line = new List<Line2>();

        for(int i=0; i < Back.lines.Length; i++){
            if(i==0){
                line.Add(new Line2(Back.lines[0].P1.X,Back.lines[0].P1.Y,Back.lines[0].P2.X,Back.lines[0].P2.Y));
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
                            line.Add(new Line2(Back.lines[i].P1.X,Back.lines[i].P1.Y,Back.lines[i].P2.X,Back.lines[i].P2.Y));
                        }
            }

        }

        int pointid = 0;
        int lineid=1000;
        int circleid=2000;
        int moduleid = 3000;

        for(int i=0; i < line.Count; i++){
            Vector3 v1 = ConvertTexture2World(line[i].x1 , line[i].y1);
            Vector3 v2 = ConvertTexture2World(line[i].x2 , line[i].y2);
            Util.AddPoint(v1,pointid++);
            Util.AddPoint(v2,pointid++);
            Util.AddLine(pointid-2, pointid-1, lineid++);
            }
            

        for(int i=0; i < Back.circles.Length; i++){
            Debug.Log(Back.circles[i]);
            Vector3 v3 = ConvertTexture2World(Back.circles[i].Center.X , Back.circles[i].Center.Y);
            Util.AddPoint(v3,pointid++);
            float rate = 9f/AppMgr.BackgroundTexture.height;
            float rad = Back.circles[i].Radius*rate;
            Util.AddCircle(pointid-1, rad, circleid++);
        }


        AppMgr.pts = MonoBehaviour.FindObjectsOfType<Point>();
        int cou = AppMgr.pts.Length;
        for(int i=0; i<cou; i++){
            for(int j=i+1; j<cou; j++){
                Point pt1 = AppMgr.pts[i];
                Point pt2 = AppMgr.pts[j];
                if((pt1.Vec-pt2.Vec).magnitude < 0.5f ){
                    Util.AddModule(MENU.POINT_ON_POINT, pt1.Id, pt2.Id, -1, moduleid++);
                }
            }
        }

        //垂直判定
        AppMgr.lns = MonoBehaviour.FindObjectsOfType<Line>();
        Line[] lns = AppMgr.lns;
        for(int i=0; i<lns.Length; i++){
            for(int j=i+1; j<lns.Length; j++){
                Point p11 = null, p12 = null, p21 = null, p22 = null;
                Line ln1 = lns[i];
                Line ln2 = lns[j];
                for(int k=0; k<AppMgr.pts.Length; k++){
                    if(ln1.Point1Id == AppMgr.pts[k].Id){
                        p11 = AppMgr.pts[k];
                    }else if(ln1.Point2Id == AppMgr.pts[k].Id){
                        p12 = AppMgr.pts[k];
                    }else if(ln2.Point1Id == AppMgr.pts[k].Id){
                        p21 = AppMgr.pts[k];
                    }else if(ln2.Point2Id == AppMgr.pts[k].Id){
                        p22 = AppMgr.pts[k];
                    }
                }
                Vector3 vec1 = Vector3.Normalize(p11.Vec - p12.Vec);
                Vector3 vec2 = Vector3.Normalize(p21.Vec - p22.Vec);
               /* Vector3 vec1 = ln1.Point1.GetComponent<Point>().Vec - ln1.Point2.GetComponent<Point>().Vec;
                Vector3 vec2 = ln2.Point1.GetComponent<Point>().Vec - ln2.Point2.GetComponent<Point>().Vec;
                */
                float inner = Vector3.Dot(vec1, vec2);
                Debug.Log(inner);
                if(Mathf.Abs(inner)<0.017f){
                    Util.AddModule(MENU.LINES_PERPENDICULAR, ln1.Id, ln2.Id, -1, moduleid++);
                }
            }
        }

        //円の接触判定
        for(int i=0; i<AppMgr.lns.Length; i++){
            for(int j=0; j<AppMgr.cis.Length; j++){
                Line ln = AppMgr.lns[i];
                Point pt1 = null, pt2 = null;
                Circle ci = AppMgr.cis[j];
                Point pt = null;
                for(int k=0; k<AppMgr.pts.Length; k++){
                    if(ci.CenterPointId == AppMgr.pts[k].Id){
                        pt = AppMgr.pts[k];
                    }
                    if(ln.Point1Id == AppMgr.pts[k].Id){
                        pt1 = AppMgr.pts[k];
                    }else if(ln.Point2Id == AppMgr.pts[k].Id){
                        pt2 = AppMgr.pts[k];
                    }
                }
                float dist = Mathf.Abs(GetPointPosition(pt1.Vec.x, pt1.Vec.y, pt2.Vec.x, pt2.Vec.y, pt.Vec.x, pt.Vec.y).y);

                if(Mathf.Abs(dist-ci.Radius)<0.1f){
                    Util.AddModule(MENU.CIRCLE_TANGENT_LINE, ci.Id, ln.Id, -1, moduleid++);
                }
            }
        }



        //Debug.Log(line.Count);
        
    AppMgr.BackgroundTexture=null;

    }
}
