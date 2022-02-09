using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using OpenCvSharp.Demo;
using OpenCvSharp;
using OpenCvSharp.Aruco;


//半径の下限
public class SliderCircleUse : MonoBehaviour, IPointerUpHandler
{
    public Slider mySlider;
    public GameObject thisSlider;

    public Texture2D texture;

    public GameObject raw;

    void Start(){
        mySlider = thisSlider.GetComponent<Slider>();
        //mySlider = GameObject.Find("SliderCircle").GetComponent<Slider>();
        mySlider.value = 50;
        
        texture=new Texture2D(1,1,TextureFormat.ARGB32, false);
    }

    public void OnPointerUp(PointerEventData eventData){

        Debug.Log("SliderCircle 現在値 : " + mySlider.value);

         Mat mat = OpenCvSharp.Unity.TextureToMat(this.texture);

        // グレースケール
        Mat gray = new Mat();
        Cv2.CvtColor(mat, gray, ColorConversionCodes.BGR2GRAY);


        raw = GameObject.Find("BackGroundScreen");
        raw.GetComponent<BackGroundScreen>().HoughLine(mat,gray);
        raw.GetComponent<BackGroundScreen>().HoughCircle(mat,gray);

        // 画像書き出し
        Texture2D outTexture = new Texture2D(mat.Width, mat.Height, TextureFormat.ARGB32, false);
        OpenCvSharp.Unity.MatToTexture(mat, outTexture);

        // 表示
        raw.GetComponent<BackGroundScreen>().Tex2D = outTexture;

    }

   /* public void Method(){
        Debug.Log("現在値: " + mySlider.value);
    }
    */
}
