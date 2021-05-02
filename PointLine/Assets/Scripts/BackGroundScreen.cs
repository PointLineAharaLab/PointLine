using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        PictureWidth = 8f;
        PictureHeight = 6f;
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
            GetComponent<MeshRenderer>().material = ScreenPictureMaterial;
        }
        else
		{
            
		}
    }

    //PNGファイルを読み込んでTextureとして使う方法
    // https://ugcj.com/png%E3%83%95%E3%82%A1%E3%82%A4%E3%83%AB%E3%82%92%E8%AA%AD%E3%81%BF%E8%BE%BC%E3%82%93%E3%81%A7texture%E3%81%A8%E3%81%97%E3%81%A6%E4%BD%BF%E3%81%86%E6%96%B9%E6%B3%95/

    public Texture2D ReadPng(string path)
    {
        //byte[] bytes = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(2, 2);
        //texture.LoadImage(bytes);
        return texture;
    }
}
