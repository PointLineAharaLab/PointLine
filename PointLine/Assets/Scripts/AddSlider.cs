using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using OpenCvSharp.Demo;
using OpenCvSharp;
using OpenCvSharp.Aruco;

public class AddSlider : MonoBehaviour
{
    public GameObject slider;
    public GameObject slider2;
    public GameObject slider3;
    public GameObject canvas;
    
    // Start is called before the first frame update
    void Start()
    {
 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddSliders(){
        GameObject prefab = (GameObject)Instantiate(slider);
        prefab.transform.SetParent(canvas.transform, false);
        GameObject prefab2 = (GameObject)Instantiate(slider2);
        prefab2.transform.SetParent(canvas.transform, false);
        GameObject prefab3 = (GameObject)Instantiate(slider3);
        prefab3.transform.SetParent(canvas.transform, false);
    }
}
