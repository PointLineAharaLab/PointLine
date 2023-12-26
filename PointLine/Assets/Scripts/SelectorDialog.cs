using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorDialog : MonoBehaviour
{
    public string Text1="";
    public bool Selected = false;
    public GameObject Text1obj;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Text1obj.GetComponent<TextMesh>().text = Text1;
    }
}
