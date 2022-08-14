using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using OpenCvSharp.Demo;
using OpenCvSharp;
using OpenCvSharp.Aruco;
using System.Linq;


public class BackGroundScreen : MonoBehaviour
{
    bool Show = true;
    public Material ScreenPictureMaterial;
    public Material ScreencolorMaterial;
    public float PictureWidth;
    public float PictureHeight;
    Slider slider1;
    Slider slider2;
    Slider slider3;
    public Vector3 PictureScale;
    bool showSlider;
    public bool SliderDestory;
    public OpenCvSharp.LineSegmentPoint[] lines;
    public OpenCvSharp.CircleSegment[] circles;
    OpenCvSharp.Point[][] contours;
    OpenCvSharp.HierarchyIndex[] hierarchyIndexes;

    // Start is called before the first frame update
    void Start()
    {
        showSlider = false;
        SliderDestory = false;

        PictureWidth = 16f;
        PictureHeight = 9f;
        PictureScale = new Vector3(PictureWidth, PictureHeight, 0.01f);

        
    }

    // Update is called once per frame
    void Update()
    {
		if (Show)
		{
            if(AppMgr.BackgroundTexture != null){
            PictureWidth = PictureHeight*AppMgr.BackgroundTexture.width/AppMgr.BackgroundTexture.height;
            }
            PictureScale.x = PictureWidth;
            PictureScale.y = PictureHeight;
            transform.localScale = PictureScale;
            if(AppMgr.BackgroundTexture != null)
            {

                if(showSlider == false){
                    RawImage RawImage = FindObjectOfType<RawImage>();
                    RawImage.GetComponent<AddSlider>().AddSliders();
                    RawImage.GetComponent<ButtonScript>().addButton();
                    showSlider = true;
                }
                //Tex2D = AppMgr.BackgroundTexture;
                Mat mat = OpenCvSharp.Unity.TextureToMat(AppMgr.BackgroundTexture);

                Mat gray = new Mat();
                Cv2.CvtColor(mat, gray, ColorConversionCodes.BGR2GRAY);

                Mat bin = new Mat();
                Contours(gray, bin);

                Hough(bin,bin);

                //HoughCircle(bin, bin);
                //HoughLine(bin, bin);
                

                Tex2D = OpenCvSharp.Unity.MatToTexture(bin, Tex2D);

                ScreenPictureMaterial.mainTexture = Tex2D;
               /* Debug.Log("height = " + AppMgr.BackgroundTexture.height);
                Debug.Log("width = " + AppMgr.BackgroundTexture.width);*/
                GetComponent<MeshRenderer>().material = ScreenPictureMaterial;
            }
            else{
                GetComponent<MeshRenderer>().material = ScreencolorMaterial;

            }
           
        }
        else
		{
            
            
		}
    }

        public void Hough(Mat gray, Mat mat2){

            if(SliderDestory == false){
                slider1 = GameObject.Find("SliderCircle(Clone)").GetComponent<Slider>();
                
                slider2 = GameObject.Find("SliderLine(Clone)").GetComponent<Slider>();
                
                slider3 = GameObject.Find("SliderLineGap(Clone)").GetComponent<Slider>();
                }

                circles = Cv2.HoughCircles(gray, HoughMethods.Gradient, 1, 80, 50, slider1.value, 0, 0);
                lines = Cv2.HoughLinesP(gray, 1, Mathf.PI/180, 100, slider2.value, slider3.value);
                
                Cv2.CvtColor(gray, gray, ColorConversionCodes.GRAY2RGB);

                for(int i=0; i < circles.Length; i++){
                    
                    mat2.Circle(circles[i].Center, (int)circles[i].Radius, new OpenCvSharp.Scalar(255,0,0), 3, LineTypes.AntiAlias);
                    
                    }

                for(int i=0; i < lines.Length; i++){
                    
                    mat2.Line(lines[i].P1, lines[i].P2, new OpenCvSharp.Scalar(0,0,255), 1, LineTypes.AntiAlias);
                    
                    }

        }

        public void HoughLine(Mat gray, Mat mat){
            
        Mat bin2 = new Mat();               

        if(SliderDestory == false){
        
        slider2 = GameObject.Find("SliderLine(Clone)").GetComponent<Slider>();

        slider3 = GameObject.Find("SliderLineGap(Clone)").GetComponent<Slider>();

        }

        lines = Cv2.HoughLinesP(gray, 1, Mathf.PI/180, 100, slider2.value, slider3.value);
        //Cv2.CvtColor(gray, bin2, ColorConversionCodes.GRAY2RGB);
        Cv2.CvtColor(gray, gray, ColorConversionCodes.GRAY2RGB);
        for(int i=0; i < lines.Length; i++){
            
            mat.Line(lines[i].P1, lines[i].P2, new OpenCvSharp.Scalar(0,0,255), 1, LineTypes.AntiAlias);
            
        }
        //Cv2.CvtColor(gray, gray, ColorConversionCodes.RGB2GRAY);

    }

    public void HoughCircle(Mat gray, Mat mat2){

        //二値化
        //Mat bin = new Mat();
        //Cv2.Threshold(gray, bin, 0, 255, ThresholdTypes.Otsu);
        
        if(SliderDestory == false){
        slider1 = GameObject.Find("SliderCircle(Clone)").GetComponent<Slider>();
        }

        circles = Cv2.HoughCircles(gray, HoughMethods.Gradient, 1, 80, 50, slider1.value, 0, 0);
        Cv2.CvtColor(gray, gray, ColorConversionCodes.GRAY2RGB);
        for(int i=0; i < circles.Length; i++){
 
            mat2.Circle(circles[i].Center, (int)circles[i].Radius, new OpenCvSharp.Scalar(255,0,0), 3, LineTypes.AntiAlias);        

        }
        //Cv2.CvtColor(gray, gray, ColorConversionCodes.RGB2GRAY);

    }

    //輪郭
    public void Contours(Mat gray, Mat mat){

        //二値化
        Mat bin = new Mat();
        Cv2.Threshold(gray, mat, 0, 255, ThresholdTypes.Otsu);

        //反転
        Cv2.BitwiseNot(mat, bin);

        Cv2.FindContours(bin, out contours, out hierarchyIndexes, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);

        for(int i=0; i<contours.Length; i++){
            var Area = Cv2.ContourArea(contours[i]);
            if(contours[i].Length > 15){
                if(Area > 50 && Area < 1500){
                Cv2.DrawContours(mat, contours, i, new OpenCvSharp.Scalar(255,255,255), -1);
                }else{
                    Cv2.DrawContours(mat, contours, i, new OpenCvSharp.Scalar(0,0,0), 1);
                }
            }
        }
    }


    //PNG�t�@�C����ǂݍ����Texture�Ƃ��Ďg�����@
    // https://ugcj.com/png%E3%83%95%E3%82%A1%E3%82%A4%E3%83%AB%E3%82%92%E8%AA%AD%E3%81%BF%E8%BE%BC%E3%82%93%E3%81%A7texture%E3%81%A8%E3%81%97%E3%81%A6%E4%BD%BF%E3%81%86%E6%96%B9%E6%B3%95/

    public Texture2D ReadPng(string path)
    {
        byte[] bytes = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(bytes);
        return texture;
    }

    public GameObject RenderCameraObject;
    public Texture2D Tex2D;
    public RenderTexture RenderTex;
    /// <summary>
    /// CaptureMain
    /// </summary>
    public void CaptureFromCamera(string path)
    {
        Debug.Log("CaptureFromCamera");
        // �����_�[�p�̃J�������擾�B
        Camera RenderCamera = RenderCameraObject.GetComponent<Camera>();
        var width = Screen.width;
        var height = Screen.height;
        // �����_�[�p�̃e�N�X�`�����擾����B
        RenderTex = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        RenderCamera.targetTexture = RenderTex;

        // �J�[�\�����f�荞�܂Ȃ��悤�ɔ�A�N�e�B�u�ɂ���B
        GameObject Cursor1Object=null, Cursor2Object=null;
        GameObject[] Objects = FindObjectsOfType<GameObject>();
        for (int i = 0; i < Objects.Length ; i++)
		{
            GameObject Obj = Objects[i];
			if (Obj.name.Contains("Cursor1"))
			{
                Cursor1Object = Obj;
                Obj.SetActive(false);
			}
            if (Obj.name.Contains("Cursor2"))
            {
                Cursor2Object = Obj;
                Obj.SetActive(false);
            }
        }
        RenderCamera.Render();
        //2�����e�N�X�`���ɃR�s�[����B
        RenderTexture.active = RenderTex;
        Tex2D = new Texture2D(width, height, TextureFormat.ARGB32, false);
        Tex2D.ReadPixels(new UnityEngine.Rect(0, 0, width, height), 0, 0);
        Tex2D.Apply();
        // �t�@�C���o�͂���B
        byte[] bytes = Tex2D.EncodeToPNG();
        File.WriteAllBytes(path, bytes);

        // �J�[�\�����A�N�e�B�u�ɂ���B        
        Cursor1Object.SetActive(true);
        Cursor2Object.SetActive(true);
        //Destroy(tex);


    }

}