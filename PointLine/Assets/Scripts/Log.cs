using System;
using UnityEngine;
using UnityEngine.UI; //

public class Log : MonoBehaviour
{
    public string ObjectType;
    public int Id;
    public bool Active;
    public bool Fixed;
    public bool Selected;
    public bool Show;
    public GameObject parent = null;
    public GameObject child = null;

    public GameObject Folder = null;

    public Material PointLogMaterial, LineLogMaterial, CircleLogMaterial, ModuleLogMaterial;
    public Material PointSLogMaterial, LineSLogMaterial, CircleSLogMaterial;

    public Vector3 Position;
    public string Text1 = "";
    public string Text2 = "";
    public GameObject TextObj1, TextObj2;

    //Point
    public Vector3 Vec = Vector3.zero;
    public string PName;
    //Line
    //public int Object1Id = -1;//    Line.Point1Id
    //public int Object2Id = -1;//    Line.Point2Id
    //Circle
    //public static int Object1Id = -1;// Circle.CenterPointId
    public float Radius=1f;
    //Module
    public int ModuleType = 0;
    public int Object1Id = -1;
    public int Object2Id = -1;
    public int Object3Id = -1;
    public GameObject Object1, Object2, Object3;
    public float Ratio1, Ratio2, Constant;
    //AngleMark
    //public int Object1Id = -1;
    //public int Object2Id = -1;

    private void Start()
    {
        if (ObjectType == "Point")
        {
            GetComponent<MeshRenderer>().material = PointLogMaterial;
        }
        else if (ObjectType == "Line")
        {
            GetComponent<MeshRenderer>().material = LineLogMaterial;
        }
        else if (ObjectType == "Circle")
        {
            GetComponent<MeshRenderer>().material = CircleLogMaterial;
        }
        else if (ObjectType == "Module")
        {
            GetComponent<MeshRenderer>().material = ModuleLogMaterial;
        }
        Position.x = Util.LogLeft;
        Selected = false;
    }

    private void Update()
    {
        //Debug.Log("Log of " + ObjectType+":"+ Screen.height);
        if (ObjectType == "Point")
        {
            PName = parent.GetComponent<Point>().PointName;
            Fixed = parent.GetComponent<Point>().Fixed;
            if (AppMgr.Japanese == 1)
            {
                Text1 = "点 " + PName;
                if (Fixed)
                    Text1 += "(固定)";
            }
            else
            {
                Text1 = "Point " + PName;
                if (Fixed)
                    Text1 += "(Fixed)";
            }
            Text2 = "(" + Mathf.Round(Vec.x * 1000f) / 1000f + "," + Mathf.Round(Vec.y * 1000f) / 1000f + ")";
            Selected = parent.GetComponent<Point>().Selected;
            if (Selected)
            {
                GetComponent<MeshRenderer>().material = PointSLogMaterial;
            }
            else
            {
                GetComponent<MeshRenderer>().material = PointLogMaterial;
            }
        }
        else if (ObjectType == "Line")
        {
            if (Object1 == null || Object2 == null)
            {
                GameObject[] OBJs = FindObjectsOfType<GameObject>();
                for (int i = 0; i < OBJs.Length; i++)
                {
                    Point PT = OBJs[i].GetComponent<Point>();
                    if (PT != null)
                    {
                        if (PT.Id == parent.GetComponent<Line>().Point1Id)
                        {
                            Object1 = OBJs[i];
                        }
                        if (PT.Id == parent.GetComponent<Line>().Point2Id)
                        {
                            Object2 = OBJs[i];
                        }
                    }
                }
            }
            if (AppMgr.Japanese == 1)
                Text1 = "直線 " + PName;
            else
                Text1 = "Line " + PName;
            Text2 = Object1.GetComponent<Point>().PointName + "-" + Object2.GetComponent<Point>().PointName;
            Selected = parent.GetComponent<Line>().Selected;
            if (Selected)
            {
                GetComponent<MeshRenderer>().material = LineSLogMaterial;
            }
            else
            {
                GetComponent<MeshRenderer>().material = LineLogMaterial;
            }
        }
        else if (ObjectType == "Circle")
        {
            if (Object1 == null)
            {
                GameObject[] OBJs = FindObjectsOfType<GameObject>();
                for (int i = 0; i < OBJs.Length; i++)
                {
                    Point PT = OBJs[i].GetComponent<Point>();
                    if (PT != null)
                    {
                        if (PT.Id == parent.GetComponent<Circle>().CenterPointId)
                        {
                            Object1 = OBJs[i];
                        }
                    }
                }
            }
            Radius = parent.GetComponent<Circle>().Radius;// Circle.csで実施
            if (AppMgr.Japanese == 1)
            {
                Text1 = "円 " + PName;
                Text2 = "中心" + Object1.GetComponent<Point>().PointName + ":半径" + Mathf.Round(Radius * 1000f) / 1000f;
            }
            else
            {
                Text1 = "Circle " + PName;
                Text2 = "Center " + Object1.GetComponent<Point>().PointName + ":Radius " + Mathf.Round(Radius * 1000f) / 1000f;
            }
            Selected = parent.GetComponent<Circle>().Selected;
            if (Selected)
            {
                GetComponent<MeshRenderer>().material = CircleSLogMaterial;
            }
            else
            {
                GetComponent<MeshRenderer>().material = CircleLogMaterial;
            }
        }
        else if (ObjectType == "Module")
        {
            if(Object1 == null || Object2 == null)
            {
                FindObjects4Module();
            }
            if (PName == "点 - 点")
            {
                if (AppMgr.Japanese == 1)
                {
                    Text1 = "" + PName;
                    Text2 = GetPNameByParentObject(Object1) + "と" + GetPNameByParentObject(Object2) + "を重ねる";
                }
                else
                {
                    Text1 = "Point on point";
                    Text2 = "" + GetPNameByParentObject(Object1) + " = " + GetPNameByParentObject(Object2) + "";
                }
            }
            else if (PName == "点 - 直線")
            {
                if (AppMgr.Japanese == 1)
                {
                    Text1 = "" + PName;
                    Text2 = GetPNameByParentObject(Object1) + "は直線" + GetPNameByParentObject(Object2) + "上";
                }
                else
                {
                    Text1 = "Point on Line";
                    Text2 = GetPNameByParentObject(Object1) + " is on " + GetPNameByParentObject(Object2) + "";
                }
            }
            else if (PName == "点 - 円")
            {
                if (AppMgr.Japanese == 1)
                {
                    Text1 = "" + PName;
                    Text2 = GetPNameByParentObject(Object1) + "は円" + GetPNameByParentObject(Object2) + "上";
                }
                else
                {
                    Text1 = "Point on CIrcle";
                    Text2 = GetPNameByParentObject(Object1) + " is on " + GetPNameByParentObject(Object2) + "";
                }
            }
            else if (PName == "等長")
            {
                if (AppMgr.Japanese == 1)
                {
                    if (Ratio1 == Ratio2)
                    {
                        Text1 = "" + PName;
                        Text2 = GetPNameByParentObject(Object1) + "と" + GetPNameByParentObject(Object2) + "は等長";
                    }
                    else
                    {
                        Text1 = "作図 : 線分比(" + Mathf.RoundToInt(Ratio1) + "," + Mathf.RoundToInt(Ratio2) + ")";
                        Text2 = GetPNameByParentObject(Object1) + "と" + GetPNameByParentObject(Object2);
                    }
                    
                }
                else
                {
                    if (Ratio1 == Ratio2)
                    {
                        Text1 = "Isometry";
                        Text2 = GetPNameByParentObject(Object1) + " , " + GetPNameByParentObject(Object2) + " are isometry";
                    }
                    else
                    {
                        Text1 = "Segment Ratio(" + Mathf.RoundToInt(Ratio1) + ":" + Mathf.RoundToInt(Ratio2) + ")";
                        Text2 = GetPNameByParentObject(Object1) + " , " + GetPNameByParentObject(Object2);
                    }
                    
                }
            }
            else if (PName == "垂直")
            {
                if (AppMgr.Japanese == 1)
                {
                    Text1 = "" + PName;
                    Text2 = GetPNameByParentObject(Object1) + "と" + GetPNameByParentObject(Object2) + "は垂直";
                }
                else
                {
                    Text1 = "Perpendicular";
                    Text2 = GetPNameByParentObject(Object1) + " perp. " + GetPNameByParentObject(Object2) + "";
                }
            }
            else if (PName == "平行")
            {
                if (AppMgr.Japanese == 1)
                {
                    Text1 = "" + PName;
                    Text2 = GetPNameByParentObject(Object1) + "と" + GetPNameByParentObject(Object2) + "は平行";
                }
                else
                {
                    Text1 = "Parallel";
                    Text2 = GetPNameByParentObject(Object1) + " || " + GetPNameByParentObject(Object2) + "";
                }
            }
            else if (PName == "円 - 直線")
            {
                if (AppMgr.Japanese == 1)
                {
                    Text1 = "" + PName;
                    Text2 = "" + GetPNameByParentObject(Object1) + "は" + GetPNameByParentObject(Object2) + "と接する";
                }
                else
                {
                    Text1 = "Tangent";
                    Text2 = "" + GetPNameByParentObject(Object1) + " tangents to " + GetPNameByParentObject(Object2) + "";
                }
            }
            else if (PName == "円 - 円")
            {
                if (AppMgr.Japanese == 1)
                {
                    Text1 = "" + PName;
                    Text2 = "" + GetPNameByParentObject(Object1) + "は" + GetPNameByParentObject(Object2) + "と接する";
                }
                else
                {
                    Text1 = "Tangent";
                    Text2 = "" + GetPNameByParentObject(Object1) + " tangents to " + GetPNameByParentObject(Object2) + "";
                }
            }
            else if (PName == "中点")
            {
                if (AppMgr.Japanese == 1)
                {
                    if (Ratio1 == Ratio2)
                    {
                        Text1 = "" + PName;
                        Text2 = GetPNameByParentObject(Object3) + "は" + GetPNameByParentObject(Object1) + "と" + GetPNameByParentObject(Object2) + "の中点";
                    }
                    else
                    {
                        Text1 = "作図 : 内分(" + Mathf.RoundToInt(Ratio1) + "," + Mathf.RoundToInt(Ratio2) + ")";
                        Text2 = GetPNameByParentObject(Object3) + "は" + GetPNameByParentObject(Object1) + "と" + GetPNameByParentObject(Object2) + "の内分点";
                    }
                }
                else
                {
                    if (Ratio1 == Ratio2)
                    {
                        Text1 = "Midpoint";
                        Text2 = GetPNameByParentObject(Object3) + "= midpoint " + GetPNameByParentObject(Object1) + " , " + GetPNameByParentObject(Object2) + "";
                    }
                    else
                    {
                        Text1 = "Dividing(" + Mathf.RoundToInt(Ratio1) + ":" + Mathf.RoundToInt(Ratio2) + ")";
                        Text2 = GetPNameByParentObject(Object3) + "= divding pt " + GetPNameByParentObject(Object1) + " , " + GetPNameByParentObject(Object2) + "";
                    }
                }
            }
            else if (PName == "角度" && Object3 != null)
            {
                if (AppMgr.Japanese == 1)
                {
                    if (parent.GetComponent<Module>().FixAngle)
                    {
                        Text1 = "固定角 ：";
                    }
                    else
                    {
                        Text1 = "角 ：";
                    }
                    float angle = Mathf.FloorToInt(Constant * 180f / Mathf.PI * 10) * 0.1f;
                    Text1 += (angle + "度");
                    Text2 = "角" + GetPNameByParentObject(Object1) + "" + GetPNameByParentObject(Object2) + "" + GetPNameByParentObject(Object3);
                }
                else
                {
                    if (parent.GetComponent<Module>().FixAngle)
                    {
                        Text1 = "Fixed angle ";
                    }
                    else
                    {
                        Text1 = "Angle ";
                    }
                    float angle = Mathf.FloorToInt(Constant * 180f / Mathf.PI * 10) * 0.1f;
                    Text1 += (angle + " degree");
                    Text2 = "Angle " + GetPNameByParentObject(Object1) + "" + GetPNameByParentObject(Object2) + "" + GetPNameByParentObject(Object3);
                }
            }
            else if (PName == "角度 - 角度" && Object1 != null && Object2 != null)
            {
                if (AppMgr.Japanese == 1)
                {
                    Text1 = "等角 ：";
                    Module md1 = Object1.GetComponent<Module>();
                    Text2 = "角" + GetPNameByParentObject(md1.Object1) + "" + GetPNameByParentObject(md1.Object2) + "" + GetPNameByParentObject(md1.Object3);
                    Module md2 = Object2.GetComponent<Module>();
                    Text2 += " = 角" + GetPNameByParentObject(md2.Object1) + "" + GetPNameByParentObject(md2.Object2) + "" + GetPNameByParentObject(md2.Object3);
                }
                else
                {
                    Text1 = "Equiangular ：";
                    Module md1 = Object1.GetComponent<Module>();
                    Text2 = "Angle " + GetPNameByParentObject(md1.Object1) + "" + GetPNameByParentObject(md1.Object2) + "" + GetPNameByParentObject(md1.Object3);
                    Module md2 = Object2.GetComponent<Module>();
                    Text2 += " = Angle" + GetPNameByParentObject(md2.Object1) + "" + GetPNameByParentObject(md2.Object2) + "" + GetPNameByParentObject(md2.Object3);
                }
            }
            else
            {
                if (AppMgr.Japanese == 1)
                {
                    Text1 = "" + PName;
                    Text2 = GetPNameByParentObject(Object1) + "-" + GetPNameByParentObject(Object2);
                }
                else
                {
                    Text1 = PName;
                    Text2 = GetPNameByParentObject(Object1) + "-" + GetPNameByParentObject(Object2);
                }
            }
        }
        if (Text1.Length > 16)
        {
            int textSize = TextObj1.GetComponent<TextMesh>().fontSize;
            textSize = Mathf.FloorToInt(textSize * 16 / Text1.Length);
            TextObj1.GetComponent<TextMesh>().fontSize = textSize;
        }
        if (Text2.Length > 16)
        {
            int textSize = TextObj2.GetComponent<TextMesh>().fontSize;
            textSize = Mathf.FloorToInt(textSize * 16 / Text2.Length);
            TextObj2.GetComponent<TextMesh>().fontSize = textSize;
        }
        TextObj1.GetComponent<TextMesh>().text = Text1;
        TextObj2.GetComponent<TextMesh>().text = Text2;
        if (Position.y < -4f || Position.y > 4 || !Active || !Show)
        {
            Position.y = 100f;
        }
        transform.position = Position;
    }

    public Log()
    {
        ObjectType = "";
        PName = "";
        Id = -1;
        Active = true;
        Fixed = false;
        Show = Util.ShowLog;
        parent = null;
        child = null;
    }

    public Log(string _t, int _id) {
        ObjectType = _t;
        Id = _id;
        Active = true;
        Fixed = false;
        Show = Util.ShowLog;
        PName = "";
    }


    public void MakePointLog(int _id, Vector3 _vec, GameObject _pa, bool _fixed, bool _active, string _pname)
    {
        ObjectType = "Point";
        Id = _id;
        Vec = _vec;
        parent = _pa;
        Active = _active;
        Fixed = _fixed;
        Show = Util.ShowLog;
        PName = _pname;
        Position.x = Util.LogLeft;
        Text1 = "点 " + PName;
        Text2 = "(" + Mathf.Round(Vec.x*1000f)/1000f + "," + Mathf.Round(Vec.y * 1000f) / 1000f + ")";
    }

    public void  MakePointLog(Point pt)
    {
        MakePointLog(pt.Id, pt.Vec, pt.parent.gameObject, pt.Fixed, pt.Active, pt.PointName);
    }//  pt.parent.gameObject は pt.gameObject でも同じ意味のハズ  

    public void MakeLineLog(int _id, int _o1, int _o2, GameObject _pa, bool _active, string _pname)
    {
        ObjectType = "Line";
        Id = _id;
        Object1Id = _o1;
        Object2Id = _o2;
        Point[] OBJs = FindObjectsOfType<Point>();
        for(int i=0; i<OBJs.Length; i++)
        {
            if(OBJs[i].Id == _o1)
            {
                Object1 = OBJs[i].gameObject;
            }
            if (OBJs[i].Id == _o2)
            {
                Object2 = OBJs[i].gameObject;
            }
        }
        parent = _pa;
        Active = _active;
        Show = Util.ShowLog;
        PName = _pname;
        Position.x = Util.LogLeft;
        Text1 = "直線 " + PName;
        Text2 = Object1.GetComponent<Point>().PointName+"-"+Object2.GetComponent<Point>().PointName;
    }

    public void MakeLineLog(Line ln)
    {
        MakeLineLog(ln.Id, ln.Point1Id, ln.Point2Id, ln.parent, ln.Active, ln.LineName);
    }

    public void MakeCircleLog(int _id, int _o1, float _rad, GameObject _pa, bool _active, string _pname)
    {
        ObjectType = "Circle";
        Id = _id;
        Object1Id = _o1;
        Point[] OBJs = FindObjectsOfType<Point>();
        for (int i = 0; i < OBJs.Length; i++)
        {
            if (OBJs[i].Id == _o1)
            {
                Object1 = OBJs[i].gameObject;
            }
        }
        Radius = _rad;
        parent = _pa;
        Active = _active;
        Show = Util.ShowLog;
        PName = _pname;
        Text1 = "円 " + PName;
        Text2 = Object1.GetComponent<Point>().PointName + ":" + Mathf.Round(Radius*1000f)/1000f;
    }

    public void MakeCircleLog(Circle ci)
    {
        MakeCircleLog(ci.Id, ci.CenterPointId, ci.Radius, ci.parent, ci.Active, ci.CircleName);
    }

    void FindObjects4Module()
    {
        Object1 = Object2 = Object3 = null;
        GameObject[] OBJs = FindObjectsOfType<GameObject>();
        for (int i = 0; i < OBJs.Length; i++)
        {
            Point pt = OBJs[i].GetComponent<Point>();
            if (pt != null)
            {
                if (pt.Id == Object1Id)
                {
                    Object1 = OBJs[i];
                }
                if (pt.Id == Object2Id)
                {
                    Object2 = OBJs[i];
                }
                if (pt.Id == Object3Id)
                {
                    Object3 = OBJs[i];
                }
            }
            Line ln = OBJs[i].GetComponent<Line>();
            if (ln != null)
            {
                if (ln.Id == Object1Id)
                {
                    Object1 = OBJs[i];
                }
                if (ln.Id == Object2Id)
                {
                    Object2 = OBJs[i];
                }
                if (ln.Id == Object3Id)
                {
                    Object3 = OBJs[i];
                }
            }
            Circle ci = OBJs[i].GetComponent<Circle>();
            if (ci != null)
            {
                if (ci.Id == Object1Id)
                {
                    Object1 = OBJs[i];
                }
                if (ci.Id == Object2Id)
                {
                    Object2 = OBJs[i];
                }
                if (ci.Id == Object3Id)
                {
                    Object3 = OBJs[i];
                }
            }
            Module md = OBJs[i].GetComponent<Module>();
            if (md != null)
            {
                if (md.Id == Object1Id)
                {
                    Object1 = OBJs[i];
                }
                if (md.Id == Object2Id)
                {
                    Object2 = OBJs[i];
                }
                if (md.Id == Object3Id)
                {
                    Object3 = OBJs[i];
                }
            }
        }
    }

    public void MakeModuleLog(int _id, int _mt, int _o1, int _o2, int _o3, GameObject _pa, bool _active, string _pname)
    {
        ObjectType = "Module";
        Id = _id;
        Object1Id = _o1;
        Object2Id = _o2;
        Object3Id = _o3;
        FindObjects4Module();
        ModuleType = _mt;
        parent = _pa;
        Active = _active;
        Show = Util.ShowLog;
        PName = _pname;
        Text1 = "作図 ：" + PName;
        Text2 = GetPNameByParentObject(Object1) + "-" + GetPNameByParentObject(Object2) ;
        if(Object3 != null)
        {
            Text2 += ("-" + GetPNameByParentObject(Object3));
        }
    }

    string GetPNameByParentObject(GameObject obj)
    {
        if (obj == null) return "";
        Point pt = obj.GetComponent<Point>();
        if(pt != null)
        {
            return pt.PointName;
        }
        Line ln = obj.GetComponent<Line>();
        if (ln != null)
        {
            return ln.LineName;
        }
        Circle ci = obj.GetComponent<Circle>();
        if (ci != null)
        {
            return ci.CircleName;
        }
        return "";
    }
    public void MakeModuleLog(Module md)
    {
        MakeModuleLog(md.Id, md.Type, md.Object1Id, md.Object2Id, md.Object3Id, md.parent, md.Active, md.ModuleName);
    }

    public void MakeObjectFromLog(Log _l)
    {
        if(_l.ObjectType == "Point")
        {
            Point pt = Util.AddPoint(_l.Vec, _l.Id);//AddPointの中でLogを作り直しているので、そもそもこれがどういう行為かはっきりしない。
            pt.PointName = _l.PName;
            pt.PTobject.GetComponent<TextMesh>().text = pt.PointName;

        }
        else if(_l.ObjectType == "Line")
        {
            Util.AddLine(_l.Object1Id, _l.Object2Id, _l.Id);
        }
        else if(_l.ObjectType == "Circle")
        {
            Util.AddCircle(_l.Object1Id, _l.Radius, _l.Id);
        }
        else if(_l.ObjectType == "Module")
        {
            Module MD = Util.AddModule(_l.ModuleType, _l.Object1Id, _l.Object2Id, _l.Object3Id, _l.Id);
            if(_l.ModuleType == MENU.LINES_PERPENDICULAR)
            {
                Util.AddAngleMark(_l.Object1Id, _l.Object2Id, MD.gameObject);
            }
            else if (_l.ModuleType == MENU.ANGLE)
            {
                Util.AddAngleMark(_l.Object1Id, _l.Object2Id, _l.Object3Id, MD.gameObject);
            }
        }
    }

    override public string ToString()
    {
        if(ObjectType == "Point")
        {
            return "Point," + Vec.x + "," + Vec.y + "," + Vec.z + "," + Id + "," + Fixed + "," + Active + "," + PName;
        }
        else if(ObjectType == "Line")
        {
            return "Line," + Object1Id + "," + Object2Id + "," + Id + "," + Active;
        }
        else if (ObjectType == "Circle")
        {
            return "Circle," + Object1Id + "," + Radius + "," + Id + "," + Active;
        }
        else if (ObjectType == "Module")
        {
            Module md = parent.GetComponent<Module>();
            return "Module," + ModuleType + "," + Object1Id + "," + Object2Id + ","
                + Object3Id + "," + Id + "," + Active + ","
                + md.Ratio1 + "," + md.Ratio2 + "," + md.Constant ;
        }
        return "";
    }

 

};