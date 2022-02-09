using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using OpenCvSharp.Demo;
using OpenCvSharp;
using OpenCvSharp.Aruco;


//直線とみなす最小の長さ
public class SliderLineUse : MonoBehaviour, IPointerUpHandler
{
    public Slider mySlider2;
    public GameObject thisSlider2;

    public Texture2D texture2;

    public GameObject raw2;

    void Start(){
        
        mySlider2 = thisSlider2.GetComponent<Slider>();
        //mySlider = GameObject.Find("SliderCircle").GetComponent<Slider>();
        mySlider2.value = 100;


        texture2=new Texture2D(1,1,TextureFormat.ARGB32, false);
        
    }

    public void OnPointerUp(PointerEventData eventData){

        Debug.Log("SliderLine 現在値 : " + mySlider2.value);

         Mat mat = OpenCvSharp.Unity.TextureToMat(this.texture2);

        // グレースケール
        Mat gray = new Mat();
        Cv2.CvtColor(mat, gray, ColorConversionCodes.BGR2GRAY);


        raw2 = GameObject.Find("BackGroundScreen");
        raw2.GetComponent<BackGroundScreen>().HoughCircle(mat,gray);
        raw2.GetComponent<BackGroundScreen>().HoughLine(mat,gray);

        // 画像書き出し
        Texture2D outTexture2 = new Texture2D(mat.Width, mat.Height, TextureFormat.ARGB32, false);
        OpenCvSharp.Unity.MatToTexture(mat, outTexture2);

        // 表示
        raw2.GetComponent<BackGroundScreen>().Tex2D = outTexture2;

    }

   /* public void Method(){
        Debug.Log("現在値: " + mySlider.value);
    }
    */
}
