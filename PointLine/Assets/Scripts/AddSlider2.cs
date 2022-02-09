using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using OpenCvSharp.Demo;
using OpenCvSharp;
using OpenCvSharp.Aruco;

public class AddSlider2 : MonoBehaviour
{
    public GameObject slider;
    public GameObject canvas;
    // Start is called before the first frame update
    void Start()
    {
        GameObject prefab = (GameObject)Instantiate(slider);
        prefab.transform.SetParent(canvas.transform, false);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
