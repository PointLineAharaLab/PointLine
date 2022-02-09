using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using OpenCvSharp.Demo;
using OpenCvSharp;
using OpenCvSharp.Aruco;


//同一直線とみなす点間隔の長さ（2点の間隔がこれより狭いとき直線とみなす）
public class SliderGapUse : MonoBehaviour, IPointerUpHandler
{
    public Slider mySlider3;
    public GameObject thisSlider3;

    public Texture2D texture3;

    public GameObject raw3;

    public void Start(){
        mySlider3 = thisSlider3.GetComponent<Slider>();
        //mySlider = GameObject.Find("SliderCircle").GetComponent<Slider>();
        mySlider3.value = 10;
        
        texture3=new Texture2D(1,1,TextureFormat.ARGB32, false);
    }

    public void OnPointerUp(PointerEventData eventData){

        Debug.Log("SliderGap 現在値 : " + mySlider3.value);


         Mat mat = OpenCvSharp.Unity.TextureToMat(this.texture3);

        // グレースケール
        Mat gray = new Mat();
        Cv2.CvtColor(mat, gray, ColorConversionCodes.BGR2GRAY);


        raw3 = GameObject.Find("BackGroundScreen");
        raw3.GetComponent<BackGroundScreen>().HoughCircle(mat,gray);
        raw3.GetComponent<BackGroundScreen>().HoughLine(mat,gray);

        // 画像書き出し
        Texture2D outTexture3 = new Texture2D(mat.Width, mat.Height, TextureFormat.ARGB32, false);
        OpenCvSharp.Unity.MatToTexture(mat, outTexture3);

        // 表示
        raw3.GetComponent<BackGroundScreen>().Tex2D = outTexture3;

    }

   /* public void Method(){
        Debug.Log("現在値: " + mySlider.value);
    }
    */
}