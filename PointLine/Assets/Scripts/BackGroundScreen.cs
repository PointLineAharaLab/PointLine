using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class BackGroundScreen : MonoBehaviour
{
    bool Show = true;
    public Material ScreenPictureMaterial;
    public float PictureWidth;
    public float PictureHeight;
    Vector3 PictureScale;
    // Start is called before the first frame update
    void Start()
    {
        PictureWidth = 16f;
        PictureHeight = 12f;
        PictureScale = new Vector3(PictureWidth, PictureHeight, 0.01f);
        
    }

    // Update is called once per frame
    void Update()
    {
		if (Show)
		{
            PictureScale.x = PictureWidth;
            PictureScale.y = PictureHeight;
            transform.localScale = PictureScale;
            if(AppMgr.BackgroundTexture != null)
            {
                Tex2D = AppMgr.BackgroundTexture;
                ScreenPictureMaterial.mainTexture = Tex2D;
            }
            GetComponent<MeshRenderer>().material = ScreenPictureMaterial;
        }
        else
		{
            
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
        Tex2D.ReadPixels(new Rect(0, 0, width, height), 0, 0);
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
