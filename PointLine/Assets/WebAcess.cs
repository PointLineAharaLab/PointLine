
using System.Collections;
using System.Collections.Generic;
using System.Web;
using UnityEngine;
using UnityEngine.Networking;
using static SimpleFileBrowser.FileBrowser;


public class WebAcess : MonoBehaviour
{
    // データベースの情報を取得するためのURL
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

    // コルーチンの開始
    IEnumerator Start()
    {
        // Postのためのform
        WWWForm form = new WWWForm();
        if (Action == "ReadWorkContent")
        {
            form.AddField("Action", "ReadWorkContent");// ReadWorkList, ReadWorkContent, ReadListFromTag, WriteWork
            form.AddField("Id", Id);
            form.AddField("WorkTitle", Title);
        }

        // UnityWebRequestを作成してURL_SELECTのページにアクセス
        UnityWebRequest request = UnityWebRequest.Post(URL, form);

        yield return request.SendWebRequest();  // リクエストを送信し、レスポンスを待つ



        if (request.result == UnityWebRequest.Result.Success) // レスポンスの結果をチェック
        {
            // レスポンスデータを取得
            ReturnData = request.downloadHandler.text;

            // HTMLエンコードされた文字列をデコード
            //string decodedData = System.Web.HttpUtility.HtmlDecode(data);

            // 改行タグを実際の改行文字に変換
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
