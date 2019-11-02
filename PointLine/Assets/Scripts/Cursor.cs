using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour {

    public Vector3 LeftBottom=Vector3.zero, RightUp=Vector3.zero;
    
	// Use this for initialization
	void Start () {
        LeftBottom = Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, 0f));
        RightUp = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
        //Debug.Log(LeftBottom.ToString() + RightUp.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        if (name == "Cursor1")
        {
            Vector3 v = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            LineRenderer lr = GetComponent<LineRenderer>();
            Vector3 Vec0 = v;
            Vector3 Vec1 = v;
            Vec0.x = LeftBottom.x;
            Vec1.x = RightUp.x;
            Vec0.z = 0f;
            Vec1.z = 0f;

            lr.SetPosition(0, Vec0);
            lr.SetPosition(1, Vec1);
        }
        if (name == "Cursor2")
        {
            Vector3 v = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            LineRenderer lr = GetComponent<LineRenderer>();
            Vector3 Vec0 = v;
            Vector3 Vec1 = v;
            Vec0.y = LeftBottom.y;
            Vec1.y = RightUp.y;
            Vec0.z = 0f;
            Vec1.z = 0f;

            lr.SetPosition(0, Vec0);
            lr.SetPosition(1, Vec1);
        }
    }
}
