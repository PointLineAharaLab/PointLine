
using System.Collections;
using System.Collections.Generic;
using System.Web;
using UnityEngine;
using UnityEngine.Networking;
using static SimpleFileBrowser.FileBrowser;


public class WebAcess : MonoBehaviour
{
    // �f�[�^�x�[�X�̏����擾���邽�߂�URL
    private string URL = "https://aharalab.sakura.ne.jp/PointLine/postForm.php";

    public string ReturnData="";
    private string Action = "None";
    private string Id = "";
    private string Title = "";
    private string WorkId = "00000";
    public bool Success = false;
    public void SetAction(string action, string id="aharalab", string workTitle = "default", string workId="00000")
    {
        Action = action;
        Id=id; 
        Title=workTitle;
        WorkId=workId;
    }

    // �R���[�`���̊J�n
    IEnumerator Start()
    {
        // Post�̂��߂�form
        WWWForm form = new WWWForm();
        if (Action == "ReadWorkContent")
        {
            form.AddField("Action", "ReadWorkContent");// ReadWorkList, ReadWorkContent, ReadListFromTag, WriteWork
            form.AddField("Id", Id);
            form.AddField("WorkTitle", Title);
        }

        // UnityWebRequest���쐬����URL_SELECT�̃y�[�W�ɃA�N�Z�X
        UnityWebRequest request = UnityWebRequest.Post(URL, form);

        yield return request.SendWebRequest();  // ���N�G�X�g�𑗐M���A���X�|���X��҂�



        if (request.result == UnityWebRequest.Result.Success) // ���X�|���X�̌��ʂ��`�F�b�N
        {
            // ���X�|���X�f�[�^���擾
            ReturnData = request.downloadHandler.text;

            // HTML�G���R�[�h���ꂽ��������f�R�[�h
            //string decodedData = System.Web.HttpUtility.HtmlDecode(data);

            // ���s�^�O�����ۂ̉��s�����ɕϊ�
            //string NewData = decodedData.Replace("<br>", "\n");
            Success = true;
            Debug.Log("Success");
            Debug.Log(ReturnData);
        }
        else
        {
            Debug.LogError("WebAPI Error: " + request.error);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
