using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using OpenCvSharp.Demo;
using OpenCvSharp;
using OpenCvSharp.Aruco;

public class BackGroundScreen : MonoBehaviour
{
    bool Show = true;
    public Material ScreenPictureMaterial;
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

                HoughLine(mat,gray);
                HoughCircle(mat,gray);

                Tex2D = OpenCvSharp.Unity.MatToTexture(mat, Tex2D);

                ScreenPictureMaterial.mainTexture = Tex2D;
               /* Debug.Log("height = " + AppMgr.BackgroundTexture.height);
                Debug.Log("width = " + AppMgr.BackgroundTexture.width);*/
            }
            GetComponent<MeshRenderer>().material = ScreenPictureMaterial;
        }
        else
		{
            
		}
    }

        public void HoughLine(Mat mat, Mat gray){
        /*
        // 画像読み込み
        Mat mat = OpenCvSharp.Unity.TextureToMat(this.texture);

        // グレースケール
        Mat gray = new Mat();
        Cv2.CvtColor(mat, gray, ColorConversionCodes.BGR2GRAY);

        */


        //２値化
        Mat bin = new Mat();
        Cv2.Canny(gray, bin, 150, 255);

        //直線検出
        //OpenCvSharp.LineSegmentPoint[] lines;

        if(SliderDestory == false){
        
        slider2 = GameObject.Find("SliderLine(Clone)").GetComponent<Slider>();

        
        slider3 = GameObject.Find("SliderLineGap(Clone)").GetComponent<Slider>();

        }

        lines = Cv2.HoughLinesP(bin, 1, Mathf.PI/180, 100, slider2.value, slider3.value);
        for(int i=0; i < lines.Length; i++){
            
            mat.Line(lines[i].P1, lines[i].P2, Scalar.Red, 1, LineTypes.AntiAlias);
            
            //Debug.Log(lines[i]);
        }

    }

    public void HoughCircle(Mat mat, Mat gray){
        /*

        // 画像読み込み
        Mat mat = OpenCvSharp.Unity.TextureToMat(this.texture);

        // グレースケール
        Mat gray = new Mat();
        Cv2.CvtColor(mat, gray, ColorConversionCodes.BGR2GRAY);

        //２値化
        Mat bin = new Mat();
        Cv2.Canny(gray, bin, 50, 255);

        */

        //円検出
        
        if(SliderDestory == false){
        slider1 = GameObject.Find("SliderCircle(Clone)").GetComponent<Slider>();
        }

        circles = Cv2.HoughCircles(gray, HoughMethods.Gradient, 1, 80, 50, slider1.value, 0, 0);
        for(int i=0; i < circles.Length; i++){
 
            mat.Circle(circles[i].Center, (int)circles[i].Radius, Scalar.Blue, 1, LineTypes.AntiAlias);        

            //Debug.Log(circles[i]);
        }


        /*
        
        // 画像書き出し
        Texture2D outTexture = new Texture2D(mat.Width, mat.Height, TextureFormat.ARGB32, false);
        OpenCvSharp.Unity.MatToTexture(mat, outTexture);

        // 表示
        GetComponent<RawImage>().texture = outTexture;
*/

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