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
}
